using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;
public enum mouseState
{
    None,
    Click,
    Hold
};

public enum SkillType
{
    QSkill,
    WSkill,
    ESkill,
    RSkill,
    Tag
};

public class PlayerbleCharacter : Character
{
    [Header("Character Info")]
    public float fMoveSpeed = 3f;

    [Header("Attack Type Info")]
    public GameObject objAttackEffect;
    public GameObject objRotationAttackEffect;
    public Skill srtQSkill;
    public Skill srtWSkill;
    public Skill srtESkill;
    public Skill srtRSkill;

    protected Skill srtCurrentSkill;
    private Queue<GameObject> queAddSkillEffectList = new Queue<GameObject>();

    [Header("Move Info")]
    public GameObject objWGSMouseIndicator;
    public GameObject objWTDMouseIndicator;
    protected virtual void Awake()
    {
        Initialize();
        StateInitalizeOnEnter();
        StateInitalizeOnExit();
        InitializeRightMouseState();
        InitializeLeftMouseState();
        fQSkillTimer = srtQSkill.fSkillCoolDown;
        fWSkillTimer = srtWSkill.fSkillCoolDown;
        fESkillTimer = srtESkill.fSkillCoolDown;
    }
    protected virtual void FixedUpdate()
    {
        //print(cStateMachine.GetCurrentState().strStateName);
        //print("mouse : " + eMouseState);
        Move();
    }

    protected void Update()
    {
        fMaxHealthPoint = Player.instance.GetPlayerMaxHp();
        fHealthPoint = Player.instance.GetPlayerHp();
    }

    #region State Var
    protected State cIdleState = new State("idleState");
    protected State cMoveState = new State("moveState");
    protected State cDodgeState = new State("dodgeState");
    protected State cToStandState = new State("toStand");
    protected State cQSkillState = new State("qSkill");
    protected State cWSkillState = new State("wSkill");
    protected State cESkillState = new State("eSkill");
    protected State cRSkillState = new State("rSkill");
    protected State cTagState = new State("tagState");
    protected State cDeadState = new State("deadState");
    protected State[] cNormalAttack = new State[5] { new State("normalAttack1"), new State("normalAttack2"), new State("normalAttack3"), new State("normalAttack4"), new State("normalAttack5") };
    #endregion

    #region Move Var
    protected bool isMoving = false;
    private bool isAttackDuringHoldMove = false;

    Coroutine moveCoroutine;
    InputAction moveAction;
    [HideInInspector]
    public mouseState eMouseState { get; set; }

    protected Vector3 mousePosOnVirtualGround;
    protected Vector3 mousePosOnGround;
    #endregion

    #region Skill Var
    [Serializable]
    public struct Skill
    {
        public GameObject objSkillEffect;
        public GameObject[] objAddSkillEffect;
        public GameObject[] objSkillArea;
        public float fSkillCoolDown;
        public float[] fSkillDamage;
        public float fMoveTimeOnBySkill;
    }
    protected float fMoveOnBySkillTimer;
    protected int fSkillDamageLinearCount = 0;
    protected int fSkillAreaCount = 0;
    protected int fAddEffectCount = 0;
    [HideInInspector] public bool isSkillEffectFollowingPlayer = false;
    #endregion

    #region QSkill Var
    protected float fQSkillTimer = 99f;
    #endregion

    #region WSkill Var
    protected float fWSkillTimer = 99f;
    #endregion

    #region ESkill Var
    protected float fESkillTimer = 99f;
    #endregion

    #region RSkill Var
    private bool isRSkillTime = false;
    #endregion

    #region WTD ESKill Var
    protected float fParabolaTimer = 0f;
    protected float fFreeFallTimer = 0f;
    #endregion

    #region WGS ESkill Var
    protected float fESkillHoldTimer = 0f;
    #endregion

    #region Normal Attack Var
    protected int nNormalAttackCount = 0;
    protected float fNormalAttackCancelTimer = 0f;
    protected float fNormalAttackCancelTime = 2f;
    protected bool canNextAttack = false;
    protected bool isNormalAttackState;

    private Coroutine corouAttackCancelCoroutine;
    protected InputAction normalAttackAction;
    #endregion

    #region Init Part
    protected void Initialize()
    {
        moveAction = GetComponent<PlayerInput>().actions["Move"];
        normalAttackAction = GetComponent<PlayerInput>().actions["NormalAttack"];
        cStateMachine = GetComponent<StateMachine>();
        cAnimator = GetComponent<Animator>();
    }

    protected virtual void StateInitalizeOnEnter()
    {
        cIdleState.onEnter += () => { ChangeAnimation(cIdleState.strStateName); };
        cMoveState.onEnter += () => { ChangeAnimation(cMoveState.strStateName); };
        cDodgeState.onEnter += () => { ChangeAnimation(cDodgeState.strStateName); };
        cToStandState.onEnter += () => { ChangeAnimation(cToStandState.strStateName); };
        cQSkillState.onEnter += () => { ChangeAnimation(cQSkillState.strStateName); Player.instance.canTag = false; transform.localRotation = GetMouseAngle(); };
        cWSkillState.onEnter += () => { ChangeAnimation(cWSkillState.strStateName); Player.instance.canTag = false; transform.localRotation = GetMouseAngle(); };
        cESkillState.onEnter += () => { ChangeAnimation(cESkillState.strStateName); Player.instance.canTag = false; transform.localRotation = GetMouseAngle(); };
        cRSkillState.onEnter += () => { ChangeAnimation(cRSkillState.strStateName); Player.instance.canTag = false; transform.localRotation = GetMouseAngle(); };
        cDeadState.onEnter += () => { ChangeAnimation(cDeadState.strStateName); };
    }

    protected virtual void StateInitalizeOnExit()
    {
        cQSkillState.onExit += () => { Player.instance.canTag = true; queAddSkillEffectList.Clear(); fSkillDamageLinearCount = 0; fSkillAreaCount = 0; fAddEffectCount = 0; };
        cWSkillState.onExit += () => { Player.instance.canTag = true; queAddSkillEffectList.Clear(); fSkillDamageLinearCount = 0; fSkillAreaCount = 0; fAddEffectCount = 0; };
        cESkillState.onExit += () => { Player.instance.canTag = true; queAddSkillEffectList.Clear(); fSkillDamageLinearCount = 0; fSkillAreaCount = 0; fAddEffectCount = 0; };
        cRSkillState.onExit += () => { Player.instance.canTag = true; queAddSkillEffectList.Clear(); fSkillDamageLinearCount = 0; fSkillAreaCount = 0; fAddEffectCount = 0; };
        cNormalAttack[0].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false; };
        cNormalAttack[1].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false; };
        cNormalAttack[2].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false; };
        cNormalAttack[3].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false; };
        cNormalAttack[4].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false; };
    }

    private void InitializeRightMouseState()
    {

        moveAction.performed += ctx =>
        {
            ActiveMouseIndicator();
            if (cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                (cStateMachine.GetCurrentState() != cESkillState || fESkillHoldTimer > 0f) &&
                cStateMachine.GetCurrentState() != cRSkillState)
            {
                isMoving = true;
                eMouseState = mouseState.Hold;
            }   // 마우스 오른쪽 꾹 누를 때 이동
            else
            {
                eMouseState = mouseState.Hold;
            }   // 이동은 안하고 마우스 오른쪽 꾹 누른 상태 저장
        };
        moveAction.canceled += ctx =>
        {
            if (cStateMachine.GetCurrentState() == cQSkillState ||
                cStateMachine.GetCurrentState() == cWSkillState ||
                cStateMachine.GetCurrentState() == cESkillState ||
                cStateMachine.GetCurrentState() == cRSkillState ||
                isAttackDuringHoldMove == true)
            {
                isAttackDuringHoldMove = false;
                isMoving = false;
                eMouseState = mouseState.None;
            }   // 스킬 사용 도중 오른쪽 마우스를 땔 때

            if (eMouseState == mouseState.Hold &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                cStateMachine.GetCurrentState() != cESkillState &&
                cStateMachine.GetCurrentState() != cRSkillState)
            {
                isMoving = false;
                eMouseState = mouseState.None;
                ChangeState(cIdleState);

            }   // 오른쪽 마우스 꾹 누르다가 땔 때
            else if (eMouseState == mouseState.None &&
                isNormalAttackState == false &&
                isAttackDuringHoldMove == false &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                (cStateMachine.GetCurrentState() != cESkillState || fESkillHoldTimer > 0f) &&
                cStateMachine.GetCurrentState() != cRSkillState &&
                GetPositionOnGround() != Vector3.zero ||
                (isNormalAttackState == true && fNormalAttackCancelTimer >= 0.2f))
            {
                if (isMoving == false)
                {
                    ChangeState(cMoveState);
                }

                isMoving = true;
                eMouseState = mouseState.Click;
                ActiveMouseIndicator();
            }   // 오른쪽 마우스 클릭할 때
        };
    }

    private void InitializeLeftMouseState()
    {
        normalAttackAction.started += ctx =>
        {
            bool canAttack = Player.instance.cCurrentCharacter == this &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                (cStateMachine.GetCurrentState() != cESkillState || fESkillHoldTimer > 0f) &&
                cStateMachine.GetCurrentState() != cRSkillState &&
                cStateMachine.GetCurrentState() != cNormalAttack[nNormalAttackCount];

            if (canNextAttack == true && corouAttackCancelCoroutine != null)
            {
                StopCoroutine(corouAttackCancelCoroutine);
                fNormalAttackCancelTimer = 0f;
            }

            if (eMouseState == mouseState.None && isAttackDuringHoldMove == false && isMoving == false && canAttack == true)
            {
                transform.localRotation = GetMouseAngle();
                RSkillGauge.Instance.IncreaseRSkillGaugeUsingAttack();
                ChangeState(cNormalAttack[nNormalAttackCount]);
            }   // 마우스 왼쪽 클릭할 때
            else if ((eMouseState == mouseState.Click || eMouseState == mouseState.None) && isNormalAttackState == false && isAttackDuringHoldMove == false && canAttack == true)
            {
                isMoving = false;
                transform.localRotation = GetMouseAngle();
                RSkillGauge.Instance.IncreaseRSkillGaugeUsingAttack();
                ChangeState(cNormalAttack[nNormalAttackCount]);
            }   // 마우스 오른쪽 클릭하고 이동하는 도중 공격할 때

            if (eMouseState == mouseState.Hold && cStateMachine.GetCurrentState() == cMoveState && isMoving == true && canAttack == true)
            {
                isAttackDuringHoldMove = true;
                eMouseState = mouseState.None;
                transform.localRotation = GetMouseAngle();
                RSkillGauge.Instance.IncreaseRSkillGaugeUsingAttack();
                ChangeState(cNormalAttack[nNormalAttackCount]);
                GameManager.instance.AsynchronousExecution(StartAttackCancelTimerWhenHoldMove());
            }   // 마우스 오른쪽 꾹 누른 상태에서 공격할 때
            else if (eMouseState == mouseState.None && cStateMachine.GetCurrentState() != cMoveState && isAttackDuringHoldMove == true && canAttack == true)
            {
                isMoving = false;
                transform.localRotation = GetMouseAngle();
                RSkillGauge.Instance.IncreaseRSkillGaugeUsingAttack();
                ChangeState(cNormalAttack[nNormalAttackCount]);
                GameManager.instance.AsynchronousExecution(StartAttackCancelTimerWhenHoldMove());
            }   // 마우스 오른쪽 꾹 누른 상태에서 마우스 왼쪽 연타할 때
        };
    }


    #endregion

    #region Move Part
    public override void Move()
    {
        switch (eMouseState)
        {
            case mouseState.Click:
                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                }
                mousePosOnGround = GetPositionOnGround();

                transform.localRotation = GetMouseAngle();
                moveCoroutine = StartCoroutine(MoveCoroutine(mousePosOnGround));
                eMouseState = mouseState.None;
                break;
            case mouseState.Hold:
                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                }
                if (cStateMachine.GetCurrentState() != cQSkillState &&
                    cStateMachine.GetCurrentState() != cWSkillState &&
                    cStateMachine.GetCurrentState() != cESkillState &&
                    cStateMachine.GetCurrentState() != cRSkillState &&
                    cStateMachine.GetCurrentState() != cDodgeState)
                {
                    if (cStateMachine.GetCurrentState() != cMoveState)
                    {
                        ChangeState(cMoveState);
                    }
                    mousePosOnVirtualGround = GetPositionOnVirtualGround();
                    transform.localRotation = GetMouseAngle();
                    transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * fMoveSpeed);
                }
                break;
        }
    }

    private IEnumerator MoveCoroutine(Vector3 mousePosOnGround)
    {
        while (cStateMachine.GetCurrentState() == cMoveState)
        {
            if (Vector3.Distance(transform.position, mousePosOnGround) <= 0.1f)
            {
                isMoving = false;
                eMouseState = mouseState.None;

                ChangeState(cIdleState);
                yield break;
            }
            transform.position = Vector3.MoveTowards(transform.position, mousePosOnGround, Time.deltaTime * fMoveSpeed);

            yield return null;
        }
    }

    private void ActiveMouseIndicator()
    {
        if (Player.instance.cCurrentCharacter == Player.instance.GetTwinSword())
        {
            GameObject obj = EffectManager.instance.GetEffect(objWTDMouseIndicator);
            obj.transform.position = GetPositionOnGround();
            obj.transform.eulerAngles = Vector3.zero;
        }
        else if (Player.instance.cCurrentCharacter == Player.instance.GetGreatSword())
        {
            GameObject obj = EffectManager.instance.GetEffect(objWGSMouseIndicator);
            obj.transform.position = GetPositionOnGround();
            obj.transform.eulerAngles = Vector3.zero;
        }
    }
    #endregion

    #region Dodge Part
    public void Dodge()
    {
        Player.instance.fDodgePlayTimer = Player.instance.fDodgePlayTime;
        transform.localRotation = GetMouseAngle();
        GameManager.instance.AsynchronousExecution(DodgeCoroutine());
        Player.instance.isDodging = false;
    }

    private IEnumerator DodgeCoroutine()
    {
        yield return new WaitUntil(() => DoDodge() <= 0f);
        Player.instance.fDodgePlayTimer = Player.instance.fDodgePlayTime;
        ReturnToIdleWithHold();
    }

    private float DoDodge()
    {
        transform.position += transform.forward * Time.deltaTime * Player.instance.fDodgePower;
        return Player.instance.fDodgePlayTimer -= Time.deltaTime;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.started &&
            cStateMachine.GetCurrentState() != cDodgeState &&
            cStateMachine.GetCurrentState() != cTagState &&
            cStateMachine.GetCurrentState() != cQSkillState &&
            cStateMachine.GetCurrentState() != cWSkillState &&
            (cStateMachine.GetCurrentState() != cESkillState || fESkillHoldTimer > 0f) &&
            cStateMachine.GetCurrentState() != cRSkillState &&
            DodgeGauge.instance.IsUsedDodge() == true &&
            Player.instance.CanDodge())
        {
            ChangeState(cDodgeState);
            Player.instance.isDodging = true;
            Dodge();
        }
    }

    #endregion

    #region Skill Part
    /*
    * 스킬 데미지 주는 방법
    * 데미지 주는 방식은 총 3가지.
    * 1. 이펙트 켬과 동시에 순차적 데미지 : 플레이어 inspector 창에 있는 Damage 배열에 데미지 추가 후 애니메이션 이벤트에는 OnLinearDamage만 추가
    * 2. 이펙트가 플레이어를 따라다니면서 일정 간격 지속 딜 :  OnFollowPlayerSkillDamage를 애니메이션 이벤트에 넣는데 float에 스킬 총 플레이 타임, int에 총 타격 횟수, 데미지는 2와 동일 [문제점: 코루틴이라 독딜처럼 딜이 들어감]
    * 3. 이펙트와 데미지 따로 분리 : OnDamageWithoutEffect, OnEffectWithoutDamage 애니메이션 이벤트에 추가. [문제점 : 잠깐이지만 Clone 두 개 켜짐]  
    *                               OnDamageWithoutEffect는 inspector에서 스킬 범위를 나타내는 프리펩을 따로 넣어야함
    */

    public virtual void OnLinearDamage()
    {
        GameObject obj = EffectManager.instance.GetEffect(srtCurrentSkill.objSkillEffect);
        PlayerEffect playerEffect = obj.GetComponent<PlayerEffect>();
        float finalDamage = ChangeDamageToRandom(srtCurrentSkill.fSkillDamage[fSkillDamageLinearCount]);

        playerEffect.OnSkillEffect(transform);
        playerEffect.OnSkillDamage(transform, finalDamage, 1 << 7);

        if (fSkillDamageLinearCount != srtCurrentSkill.fSkillDamage.Length - 1)
        {
            fSkillDamageLinearCount++;
        }
    }

    public void OnFollowPlayerSkillDamage(AnimationEvent skillEvent)
    {
        float finalDamage = 0f;
        GameObject obj = EffectManager.instance.GetEffect(srtCurrentSkill.objSkillEffect);
        isSkillEffectFollowingPlayer = true;

        for (int i = 0; i < srtCurrentSkill.fSkillDamage.Length; i++)
        {
            finalDamage = ChangeDamageToRandom(srtCurrentSkill.fSkillDamage[i]);
        }
        obj.GetComponent<PlayerEffect>().OnSkillEffect(transform);
        obj.GetComponent<PlayerEffect>().OnSkillContinueDamage(transform, finalDamage, 1 << 7, skillEvent.floatParameter, skillEvent.intParameter);
    }

    public void OnDamageWithoutEffect(float damage)
    {
        GameObject obj = EffectManager.instance.GetEffect(srtCurrentSkill.objSkillArea[fSkillAreaCount]);
        PlayerEffect playerEffect = obj.GetComponent<PlayerEffect>();
        float finalDamage = ChangeDamageToRandom(damage);
        playerEffect.OnSkillDamage(transform, finalDamage, 1 << 7);


        if (fSkillAreaCount != srtCurrentSkill.objSkillArea.Length - 1)
        {
            fSkillAreaCount++;
        }
    }

    public void OnEffectWithoutDamage()
    {
        GameObject obj = EffectManager.instance.GetEffect(srtCurrentSkill.objSkillEffect);
        PlayerEffect playerEffect = obj.GetComponent<PlayerEffect>();

        playerEffect.OnSkillEffect(transform);
    }

    public void OnAddSkillEffect()
    {
        GameObject obj = EffectManager.instance.GetEffect(srtCurrentSkill.objAddSkillEffect[fAddEffectCount]);
        PlayerEffect playerEffect = obj.GetComponent<PlayerEffect>();

        queAddSkillEffectList.Enqueue(obj);
        playerEffect.OnAddSkillEffect(transform, obj);

        if (fAddEffectCount != srtCurrentSkill.objAddSkillEffect.Length - 1)
        {
            fAddEffectCount++;
        }
    }

    private void InPoolAddSkillEffect()
    {
        for (int i = 0; i < queAddSkillEffectList.Count; i++)
        {
            queAddSkillEffectList.TryDequeue(out GameObject obj);
            obj.gameObject.SetActive(false);
        }
    }

    protected void EnableRotationAttackEffect(float damage)
    {
        GameObject obj = EffectManager.instance.GetEffect(objRotationAttackEffect);
        float finalDamage = ChangeDamageToRandom(damage);
        obj.GetComponent<Effect>().OnAction(transform, finalDamage, 1 << 7);
    }


    private void OnMoveOnBySkill(AnimationEvent skillEvent)   // 애니메이션 이벤트에서 power 수정
    {
        GameManager.instance.AsynchronousExecution(StartMoveOnBySkill(skillEvent.floatParameter));
    }

    private IEnumerator StartMoveOnBySkill(float power)
    {
        fMoveOnBySkillTimer = srtCurrentSkill.fMoveTimeOnBySkill;
        yield return new WaitUntil(() => DoMoveOnBySkill(power) <= 0f);
    }

    private float DoMoveOnBySkill(float power)
    {
        transform.position += transform.forward * Time.deltaTime * power;
        return fMoveOnBySkillTimer -= Time.deltaTime;
    }

    public void UseSkillWithoutPressKey(SkillType skillType, Vector3 target)
    {
        switch (skillType)
        {
            case SkillType.QSkill:
                DoSkillWithoutPressKey(SkillType.QSkill, target);
                break;
            case SkillType.WSkill:
                DoSkillWithoutPressKey(SkillType.WSkill, target);
                break;
            case SkillType.ESkill:
                DoSkillWithoutPressKey(SkillType.ESkill, target);
                break;
            case SkillType.RSkill:
                DoSkillWithoutPressKey(SkillType.RSkill, target);
                break;
            default:
                Debug.Log($"Skill Type is Wrong Type!");
                break;
        }
    }

    public void UseSkillWithoutPressKey(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Tag:
                DoTagWithoutPressKey();
                break;
            default:
                Debug.Log($"Skill Type is Wrong Type!");
                break;
        }
    }

    protected virtual void DoSkillWithoutPressKey(SkillType skillType, Vector3 t) { Debug.Log("DoSkillWithoutPressKey isn't override! "); }
    #endregion

    #region Normal Attack Part
    protected IEnumerator StartAttackCancelTimerWhenHoldMove()
    {
        yield return new WaitUntil(() => fNormalAttackCancelTimer > 0.2f || cStateMachine.GetCurrentState() == cIdleState);
        fNormalAttackCancelTimer = 0f;
        KeepHoldMove();
    }
    protected virtual void IncreaseAttackCount() { print("You have not added anything"); }

    protected void ResetAttackCount()
    {
        if (isNormalAttackState == false)
        {
            canNextAttack = false;
            nNormalAttackCount = 0;
        }
    }

    protected void CheckExceededCancelTime()
    {
        if (fNormalAttackCancelTimer >= fNormalAttackCancelTime - 0.05f
            && cStateMachine.GetCurrentState() != cQSkillState
            && cStateMachine.GetCurrentState() != cWSkillState
            && cStateMachine.GetCurrentState() != cESkillState
            && cStateMachine.GetCurrentState() != cRSkillState)
        {
            ReturnToIdleWithHold();
        }
    }

    protected void DisableNextAttack()
    {
        canNextAttack = false;
    }

    protected void EnableNextAttack()
    {
        corouAttackCancelCoroutine = StartCoroutine(StartNormalAttackCancelTimer());
        canNextAttack = true;
    }

    private IEnumerator StartNormalAttackCancelTimer()
    {
        while (isNormalAttackState == true)
        {
            fNormalAttackCancelTimer += Time.deltaTime;
            yield return null;
        }
        fNormalAttackCancelTimer = 0f;
    }
    #endregion

    #region Tag Part
    public void OnTag(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            UIManager.instance.ConvertPlayer();
        }

        if (ctx.started && Player.instance.canTag == true && (Player.instance.fTagTimer >= Player.instance.fTagCoolDown || Player.instance.fTagTimer == 0f))
        {
            EnemyManager.instance.SlowEndAllEnemy();
            Player.instance.canTag = false;
            Player.instance.ConvertCharacter();
            ChangeState(cTagState);
            CameraManager.instance.ResetCamera();
        }
    }
    public IEnumerator StartTagCoolDown()
    {
        Player.instance.fTagTimer = 0f;
        while (Player.instance.fTagTimer <= Player.instance.fTagCoolDown)
        {
            Player.instance.fTagTimer += Time.deltaTime;
            yield return null;
        }
        Player.instance.canTag = true;
        Player.instance.fTagTimer = Player.instance.fTagCoolDown;
    }

    private void DoTagWithoutPressKey()
    {
        UIManager.instance.ConvertPlayer();
        Player.instance.ConvertCharacter();
        ChangeState(cTagState);
        CameraManager.instance.ResetCamera();
    }

    #endregion

    public bool IsRSkillTime()
    {
        return isRSkillTime;
    }
    public void SetIsRSkillTime(bool isRSkillTime)
    {
        this.isRSkillTime = isRSkillTime;
    }
    public void CutCoolDown(float cutTime)
    {
        if (isRSkillTime == false && Player.instance.cCurrentCharacter == Player.instance.GetTwinSword())
        {
            srtQSkill.fSkillCoolDown /= cutTime;
            srtWSkill.fSkillCoolDown /= cutTime;
            srtESkill.fSkillCoolDown /= cutTime;
            ResetAllCoolDown();
        }
    }
    public void RestoreCoolDown(float restoreTime)
    {
        if (isRSkillTime == true && Player.instance.cCurrentCharacter == Player.instance.GetTwinSword())
        {
            srtQSkill.fSkillCoolDown *= restoreTime;
            srtWSkill.fSkillCoolDown *= restoreTime;
            srtESkill.fSkillCoolDown *= restoreTime;
            
            fQSkillTimer *= restoreTime;
            fWSkillTimer *= restoreTime;
            fESkillTimer *= restoreTime;
        }

        if (fQSkillTimer > 0f && fQSkillTimer <= srtQSkill.fSkillCoolDown == false)
        {
            fQSkillTimer = srtQSkill.fSkillCoolDown;
        }
        if (fWSkillTimer > 0f && fWSkillTimer <= srtWSkill.fSkillCoolDown == false)
        {
            fWSkillTimer = srtWSkill.fSkillCoolDown;
        }
        if (fESkillTimer > 0f && fESkillTimer <= srtESkill.fSkillCoolDown == false)
        {
            fESkillTimer = srtESkill.fSkillCoolDown;
        }
    }

    private void ResetAllCoolDown()
    {
        fQSkillTimer = srtQSkill.fSkillCoolDown;
        fWSkillTimer = srtWSkill.fSkillCoolDown;
        fESkillTimer = srtESkill.fSkillCoolDown;
    }

    public virtual float GetCoolDownCutAndRestoreTime() { print("You have not overrided function"); return 0f; }

    public string GetCurrentStateName()
    {
        return cStateMachine.GetCurrentState().strStateName;
    }

    public override void Damage(float fAmount) { }

    protected int ChangeDamageToRandom(float originalDamage)
    {
        float minDamage = originalDamage * 0.9f;
        float maxDamage = originalDamage * 1.1f;

        return Mathf.FloorToInt(UnityEngine.Random.Range(minDamage, maxDamage));
    }

    protected virtual void ReduceHP(float fAmount) { }

    public override void Die()
    {
        if (cStateMachine.GetCurrentState() == cDeadState) return;
        ChangeState(cDeadState);
        Player.instance.EnablePlayerInput(false);
        GameManager.instance.GameLose();

    }

    public override void ChangeState(State cNextState)
    {
        if (cStateMachine.GetCurrentState() != cDeadState)
        {
            cStateMachine.ChangeState(cNextState);
        }
        
    }

    public override void Attack()
    {

    }

    public override void ChangeAnimation(string strTrigger)
    {
        if (cStateMachine.GetPrevState() != null)
            cAnimator.ResetTrigger(cStateMachine.GetPrevState().strStateName);
        cAnimator.SetTrigger(strTrigger);
    }

    public void ReturnToIdle()
    {
        ChangeState(cIdleState);
    }
    protected void ReturnToIdleWithHold()
    {
        ChangeState(cIdleState);
        KeepHoldMove();
    }
    protected void ChangeToStand()
    {
        ChangeState(cToStandState);
    }

    protected void KeepHoldMove()
    {
        if (isMoving == true && eMouseState == mouseState.Hold
            || (isMoving == true && isNormalAttackState == true)
            || isAttackDuringHoldMove == true)
        {
            isMoving = true;
            ChangeState(cMoveState);
            eMouseState = mouseState.Hold;
        }
    }

    protected Vector3 GetPositionOnVirtualGround()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 pos = Vector3.zero;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, 1 << 9))
            pos = hit.point;

        return pos;
    }
    protected Vector3 GetPositionOnGround()
    {
        Ray ray;
        NavMeshHit navMeshHit;
        RaycastHit hit;
        Vector3 pos = Vector3.zero;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1f, NavMesh.AllAreas))
            {
                pos = navMeshHit.position;
            }
        }
        return pos;
    }
    protected Quaternion GetMouseAngle()
    {
        double angle = Math.Atan2(GetMouseNormalizedXPosition(), GetMouseNormalizedYPosition()) * 180 / Math.PI;
        double targetAngle = angle < 0f ? angle + 360f : angle;

        return Quaternion.Euler(new Vector3(transform.rotation.x, (float)targetAngle + Camera.main.transform.localEulerAngles.y, transform.rotation.z));
    }

    protected float GetMouseNormalizedXPosition()
    {
        float xNormalized = 2 * (Input.mousePosition.x - 0f) / (Screen.width - 0f) - 1f;
        float mouseNormalizedXPosition;

        if (xNormalized > 1f)
            mouseNormalizedXPosition = 1f;
        else if (xNormalized < -1f)
            mouseNormalizedXPosition = -1f;
        else
            mouseNormalizedXPosition = xNormalized;

        return mouseNormalizedXPosition;
    }

    protected float GetMouseNormalizedYPosition()
    {
        float yNormalized = 2 * (Input.mousePosition.y - 0f) / (Screen.height - 0f) - 1f;
        float mouseNormalizedYPosition;

        if (yNormalized > 1f)
            mouseNormalizedYPosition = 1f;
        else if (yNormalized < -1f)
            mouseNormalizedYPosition = -1f;
        else
            mouseNormalizedYPosition = yNormalized;

        return mouseNormalizedYPosition;
    }

    public PlayerSkillTimeInfo GetSkillTimer(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.QSkill:
                return new PlayerSkillTimeInfo(srtQSkill.fSkillCoolDown, fQSkillTimer);
            case SkillType.WSkill:
                return new PlayerSkillTimeInfo(srtWSkill.fSkillCoolDown, fWSkillTimer);
            case SkillType.ESkill:
                return new PlayerSkillTimeInfo(srtESkill.fSkillCoolDown, fESkillTimer);
            case SkillType.Tag:
                return new PlayerSkillTimeInfo(Player.instance.fTagCoolDown, Player.instance.fTagTimer);
        }
        return new PlayerSkillTimeInfo(0,0);
    }
}