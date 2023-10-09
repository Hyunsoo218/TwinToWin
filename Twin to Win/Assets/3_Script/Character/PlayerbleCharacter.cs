using JetBrains.Annotations;
using System;
using System.Collections;
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
    }
    protected virtual void FixedUpdate()
    {
        //print(cStateMachine.GetCurrentState().strStateName);
        //print("mouse : " + eMouseState);
        Move();
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
        public float fSkillCoolDown;
        public float fMoveTimeOnBySkill;
    }
    protected float fMoveOnBySkillTimer;
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
        cQSkillState.onExit += () => { Player.instance.canTag = true; };
        cWSkillState.onExit += () => { Player.instance.canTag = true; };
        cESkillState.onExit += () => { Player.instance.canTag = true; };
        cRSkillState.onExit += () => { Player.instance.canTag = true; };
        cNormalAttack[0].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false;  };
        cNormalAttack[1].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false;  };
        cNormalAttack[2].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false;  };
        cNormalAttack[3].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false;  };
        cNormalAttack[4].onExit += () => { if (isMoving == true) { eMouseState = mouseState.Hold; isAttackDuringHoldMove = false; } isNormalAttackState = false;  };
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
                if (gameObject.activeSelf == true)
                {
                    cStateMachine.ChangeState(cIdleState);
                }

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
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
            }   // 마우스 왼쪽 클릭할 때
            else if ((eMouseState == mouseState.Click || eMouseState == mouseState.None) && isNormalAttackState == false && isAttackDuringHoldMove == false && canAttack == true)
            {
                isMoving = false;
                transform.localRotation = GetMouseAngle();
                RSkillGauge.Instance.IncreaseRSkillGaugeUsingAttack();
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
            }   // 마우스 오른쪽 클릭하고 이동하는 도중 공격할 때

            if (eMouseState == mouseState.Hold && cStateMachine.GetCurrentState() == cMoveState && isMoving == true && canAttack == true)
            {
                isAttackDuringHoldMove = true;
                eMouseState = mouseState.None;
                transform.localRotation = GetMouseAngle();
                RSkillGauge.Instance.IncreaseRSkillGaugeUsingAttack();
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
                GameManager.instance.AsynchronousExecution(StartAttackCancelTimerWhenHoldMove());
            }   // 마우스 오른쪽 꾹 누른 상태에서 공격할 때
            else if (eMouseState == mouseState.None && cStateMachine.GetCurrentState() != cMoveState && isAttackDuringHoldMove == true && canAttack == true)
            {
                isMoving = false;
                transform.localRotation = GetMouseAngle();
                RSkillGauge.Instance.IncreaseRSkillGaugeUsingAttack();
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
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
                cStateMachine.ChangeState(cMoveState);
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
                        cStateMachine.ChangeState(cMoveState);
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

                cStateMachine.ChangeState(cIdleState);
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
            cStateMachine.ChangeState(cDodgeState);
            Player.instance.isDodging = true;
            Dodge();
        }
    }

    #endregion

    #region Skill Part

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

    protected void EnableAttackEffect()
    {
        GameObject obj = EffectManager.instance.GetEffect(objAttackEffect);
        obj.GetComponent<Effect>().OnAction(transform, fPower, 1 << 7);
    }

    protected void EnableRotationAttackEffect()
    {
        GameObject obj = EffectManager.instance.GetEffect(objRotationAttackEffect);
        obj.GetComponent<Effect>().OnAction(transform, fPower, 1 << 7);
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
            Player.instance.canTag = false;
            Player.instance.ConvertCharacter();
            cStateMachine.ChangeState(cTagState);
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
        cStateMachine.ChangeState(cTagState);
        CameraManager.instance.ResetCamera();
    }

    #endregion

    //public virtual void OnRSkill(InputAction.CallbackContext ctx)
    //{
    //    //if (ctx.started && FeverGauge.Instance.IsDoubleFeverGaugeFull() == true)
    //    //{
    //    //    Constants.fSpeedConstant = 2f;
    //    //    Player.instance.GetTwinSword().cAnimator.speed = Constants.fSpeedConstant;
    //    //    Player.instance.GetGreatSword().cAnimator.speed = Constants.fSpeedConstant;
    //    //    Player.instance.GetTwinSword().CutCoolDown(fCoolDownCutAndRestoreTime);
    //    //    Player.instance.GetGreatSword().CutCoolDown(fCoolDownCutAndRestoreTime);
    //    //    Player.instance.GetTwinSword().SetIsFeverTime(true);
    //    //    Player.instance.GetGreatSword().SetIsFeverTime(true);
    //    //    GameManager.instance.AsynchronousExecution(FeverGauge.Instance.StartDoubleFeverTime());
    //    //}
    //}
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
            fQSkillTimer = srtQSkill.fSkillCoolDown;

            srtWSkill.fSkillCoolDown /= cutTime;
            fWSkillTimer = srtWSkill.fSkillCoolDown;

            srtESkill.fSkillCoolDown /= cutTime;
            fESkillTimer = srtESkill.fSkillCoolDown;
        }
    }
    public void RestoreCoolDown(float restoreTime)
    {
        if (isRSkillTime == true && Player.instance.cCurrentCharacter == Player.instance.GetTwinSword())
        {
            srtQSkill.fSkillCoolDown *= restoreTime;
            fQSkillTimer = srtQSkill.fSkillCoolDown;

            srtWSkill.fSkillCoolDown *= restoreTime;
            fWSkillTimer = srtWSkill.fSkillCoolDown;

            srtESkill.fSkillCoolDown *= restoreTime;
            fESkillTimer = srtESkill.fSkillCoolDown;
        }
    }

    public virtual float GetCoolDownCutAndRestoreTime() { print("You have not overrided function");  return 0f; }

    public string GetCurrentStateName()
    {
        return cStateMachine.GetCurrentState().strStateName;
    }

    public override void Damage(float fAmount)
    {
        
    }

    protected virtual void ReduceHP(float fAmount) {}

    public override void Die()
    {
        cStateMachine.ChangeState(cDeadState);
        Player.instance.EnableCurrentPlayerInput(false);
    }

    public override void ChangeState(State cNextState)
    {

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

    protected void ReturnToIdle()
    {
        cStateMachine.ChangeState(cIdleState);
    }
    protected void ReturnToIdleWithHold()
    {
        cStateMachine.ChangeState(cIdleState);
        KeepHoldMove();
    }
    protected void ChangeToStand()
    {
        cStateMachine.ChangeState(cToStandState);
    }

    protected void KeepHoldMove()
    {
        if (isMoving == true && eMouseState == mouseState.Hold
            || (isMoving == true && isNormalAttackState == true)
            || isAttackDuringHoldMove == true)
        {
            isMoving = true;
            cStateMachine.ChangeState(cMoveState);
            eMouseState = mouseState.Hold;
        }
    }

    public Animator GetAnimator()
    {
        return cAnimator;
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

    public float GetSkillTimer(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.QSkill:
                return fQSkillTimer / srtQSkill.fSkillCoolDown;
            case SkillType.WSkill:
                return fWSkillTimer / srtWSkill.fSkillCoolDown;
            case SkillType.ESkill:
                return fESkillTimer / srtESkill.fSkillCoolDown;
            case SkillType.Tag:
                return Player.instance.fTagTimer / Player.instance.fTagCoolDown;
        }
        return 0f;
    }
}