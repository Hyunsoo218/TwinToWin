using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;
public enum mouseState
{
    None,
    Click,
    Hold
};
/** 
    버그 리스트
    =마우스 클릭 Hold 이동 절묘하게 느림 : 하
    =우클릭 Click을 하면 가끔 멈춤 : 중
    =Hold를 아주 살짝 했다가 떼면 toStand가 유지됨 : 하
    =게임 시작 전 WGS, WTD 둘 다 켜놓으면 안 움직임 : 하
    해야할 거
    =마우스 이펙트를 쓸데없이 Inspector에서 둘 다 받음
**/

public class PlayerbleCharacter : Character
{
    [Header("Character Info")]
    [SerializeField] private float fMoveSpeed = 3f;
    [SerializeField] private float fDodgePower = 30f;
    [SerializeField] private float fDodgePlayTime = 0.1f;
    [SerializeField] private float fDodgeCoolTime = 3f;

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
        InitializeRightMouseState();
        InitializeLeftMouseState();
    }
    protected virtual void FixedUpdate()
    {
        //print(cStateMachine.GetCurrentState().strStateName);
        //print("mouse : " + eMouseState);
        Move();
        Dodge();
    }

    #region State
    protected State cIdleState = new State("idleState");
    protected State cMoveState = new State("moveState");
    protected State cDodgeState = new State("dodgeState");
    protected State cToStandState = new State("toStand");
    protected State cQSkillState = new State("qSkill");
    protected State cWSkillState = new State("wSkill");
    protected State cESkillState = new State("eSkill");
    protected State cRSkillState = new State("rSkill");
    protected State cTagState = new State("tagState");
    protected State[] cNormalAttack = new State[5] { new State("normalAttack1"), new State("normalAttack2"), new State("normalAttack3"), new State("normalAttack4"), new State("normalAttack5") };
    #endregion

    #region Move
    protected bool isMoving = false;

    Coroutine moveCoroutine;
    InputAction moveAction;
    [HideInInspector]
    public mouseState eMouseState { get; set; }

    protected Vector3 mousePosOnVirtualGround;
    protected Vector3 mousePosOnGround;
    #endregion

    #region Dodge
    private float fDodgeTimer;
    private float fDodgePlayTimer;
    private bool isDodging = false;
    protected bool canDodge = true;
    #endregion

    #region Skill
    [Serializable]
    public struct Skill
    {
        public GameObject objSkillEffect;
        public float fSkillCoolDown;
        public float fMoveTimeOnBySkill;
    }
    protected float fMoveOnBySkillTimer;
    #endregion

    #region QSkill
    protected float fQSkillTimer = 0f;
    #endregion

    #region WSkill
    protected float fWSkillTimer = 0f;
    #endregion

    #region ESkill
    protected float fESkillTimer = 0f;
    #endregion

    #region WTD ESKill
    protected float fParabolaTimer = 0f;
    protected float fFreeFallTimer = 0f;
    #endregion

    #region WGS ESkill
    protected float fESkillHoldTimer = 0f;
    #endregion

    #region Normal Attack
    protected int nNormalAttackCount = 0;
    protected float fNormalAttackCancelTimer = 0f;
    protected float fNormalAttackCancelTime = 2f;
    protected bool canNextAttack = true;
    protected bool isNotNormalAttackState;

    private Coroutine corouAttackCancelCoroutine;
    protected InputAction normalAttackAction;
    #endregion

    #region Tag
    protected static bool canTag = true;
    protected static float fTagTimer = 0f;
    protected static float fTagCoolTime = 2f;
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
        cQSkillState.onEnter += () => { ChangeAnimation(cQSkillState.strStateName); };
        cWSkillState.onEnter += () => { ChangeAnimation(cWSkillState.strStateName); };
        cESkillState.onEnter += () => { ChangeAnimation(cESkillState.strStateName); };
        cRSkillState.onEnter += () => { ChangeAnimation(cRSkillState.strStateName); };
    }

    private void InitializeRightMouseState()
    {
        moveAction.performed += ctx =>
        {
            ActiveMouseIndicator();
            if (cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                cStateMachine.GetCurrentState() != cESkillState)
            {
                isMoving = true;
                eMouseState = mouseState.Hold;

            }
            else
            {
                eMouseState = mouseState.Hold;
            }
        };
        moveAction.canceled += ctx =>
        {
            if (eMouseState == mouseState.None &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                cStateMachine.GetCurrentState() != cESkillState)
            {
                isMoving = true;
                eMouseState = mouseState.Click;
                ActiveMouseIndicator();
            }

            if (eMouseState == mouseState.Hold)
            {
                isMoving = false;
                eMouseState = mouseState.None;
                if (gameObject.activeSelf == true)
                {
                    cStateMachine.ChangeState(cToStandState);
                }
                
            }
        };
    }

    private void InitializeLeftMouseState()
    {
        normalAttackAction.started += ctx =>
        {
            if (canNextAttack == true && corouAttackCancelCoroutine != null)
            {
                StopCoroutine(corouAttackCancelCoroutine);
                fNormalAttackCancelTimer = 0f;
            }


            if (eMouseState != mouseState.Hold &&
                cStateMachine.GetCurrentState() != cNormalAttack[nNormalAttackCount] &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                cStateMachine.GetCurrentState() != cESkillState &&
                Player.instance.cCurrentCharacter == this &&
                canNextAttack)
            {
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
            }
            else if (eMouseState == mouseState.Hold &&
                cStateMachine.GetCurrentState() != cNormalAttack[nNormalAttackCount] &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                cStateMachine.GetCurrentState() != cESkillState &&
                Player.instance.cCurrentCharacter == this &&
                canNextAttack)
            {
                GameManager.instance.AsynchronousExecution(AttackDuringHoldMove());
            }
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
                    cStateMachine.GetCurrentState() != cDodgeState &&
                    cStateMachine.GetCurrentState() != cToStandState &&
                    cStateMachine.GetCurrentState() != cNormalAttack[0] &&
                    cStateMachine.GetCurrentState() != cNormalAttack[1] &&
                    cStateMachine.GetCurrentState() != cNormalAttack[2] &&
                    cStateMachine.GetCurrentState() != cNormalAttack?[3] &&
                    cStateMachine.GetCurrentState() != cNormalAttack?[4])
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

                cStateMachine.ChangeState(cToStandState);
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
            Instantiate(objWTDMouseIndicator, GetPositionOnGround(), Quaternion.identity);
        }
        else if (Player.instance.cCurrentCharacter == Player.instance.GetGreatSword())
        {
            Instantiate(objWGSMouseIndicator, GetPositionOnGround(), Quaternion.identity);
        }
    }
    #endregion

    #region Dodge Part
    public void Dodge()
    {
        if (isDodging && cStateMachine.GetCurrentState() == cDodgeState)
        {
            fDodgePlayTimer = fDodgePlayTime;
            GameManager.instance.AsynchronousExecution(DodgeCoroutine());
            isDodging = false;
        }
    }

    private IEnumerator DodgeCoroutine()
    {
        yield return new WaitUntil(() => DoDodge() <= 0f);
        fDodgePlayTimer = fDodgePlayTime;
        cStateMachine.ChangeState(cToStandState);
    }
    protected void CanDodge()
    {
        canDodge = true;
    }

    private float DoDodge()
    {
        transform.localRotation = GetMouseAngle();
        transform.position += transform.forward * Time.deltaTime * fDodgePower;
        return fDodgePlayTimer -= Time.deltaTime;
    }

    private IEnumerator StartDodgeCoolDown()
    {
        canDodge = false;
        while (fDodgeTimer <= fDodgeCoolTime)
        {
            fDodgeTimer += Time.deltaTime;
            yield return null;
        }
        fDodgeTimer = 0f;
        canDodge = true;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.started && fDodgeTimer <= 0f &&
            cStateMachine.GetCurrentState() != cDodgeState &&
            cStateMachine.GetCurrentState() != cTagState &&
            canDodge == true)
        {
            cStateMachine.ChangeState(cDodgeState);
            GameManager.instance.AsynchronousExecution(StartDodgeCoolDown());
            isDodging = true;
        }
    }

    #endregion

    #region Skill Part
    private void EnableSkillEffect()
    {
        GameObject obj = EffectManager.instance.GetEffect(srtCurrentSkill.objSkillEffect);
        obj.GetComponent<Effect>().OnAction(transform, fPower, 1 << 7);
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
    #endregion

    #region Normal Attack Part
    protected IEnumerator AttackDuringHoldMove()
    {
        cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
        yield return new WaitUntil(() => fNormalAttackCancelTimer > 0.2f || cStateMachine.GetCurrentState() == cIdleState);
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
    protected void IncreaseAttackCount()
    {
        if (cStateMachine.GetCurrentState() == cNormalAttack[nNormalAttackCount] && fNormalAttackCancelTimer < fNormalAttackCancelTime)
        {
            nNormalAttackCount = nNormalAttackCount < cNormalAttack.Length - 1 ? ++nNormalAttackCount : 0;
        }
    }

    protected void ResetAttackCount()
    {
        if (isNotNormalAttackState)
        {
            canNextAttack = true;
            nNormalAttackCount = 0;
        }
    }

    protected void CheckExceededCancelTime()
    {
        if (fNormalAttackCancelTimer >= fNormalAttackCancelTime - 0.05f
            && cStateMachine.GetCurrentState() != cQSkillState
            && cStateMachine.GetCurrentState() != cWSkillState
            && cStateMachine.GetCurrentState() != cESkillState)
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
        while (fNormalAttackCancelTimer < fNormalAttackCancelTime)
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
        if (canTag == true)
        {
            canTag = false;
            Player.instance.ConvertCharacter();
            cStateMachine.ChangeState(cTagState);
        }
    }
    public IEnumerator StartTagCoolDown()
    {
        while (fTagTimer <= fTagCoolTime)
        {
            fTagTimer += Time.deltaTime;
            yield return null;
        }
        fTagTimer = 0f;
        canTag = true;
    }
    #endregion

    public string GetCurrentStateName()
    {
        return cStateMachine.GetCurrentState().strStateName;
    }

    public override void Damage(float fAmount)
    {

    }

    public override void Die()
    {

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
            && cStateMachine.GetCurrentState() != cQSkillState
            && cStateMachine.GetCurrentState() != cWSkillState
            && cStateMachine.GetCurrentState() != cESkillState)
        {
            cStateMachine.ChangeState(cMoveState);
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
        RaycastHit hit;
        Vector3 pos = Vector3.zero;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, 1 << 6))
            pos = hit.point;
        else
            pos = transform.position;

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


}
