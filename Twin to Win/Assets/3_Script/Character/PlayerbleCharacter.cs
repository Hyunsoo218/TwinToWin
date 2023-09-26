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
    Tag
};

public class Constants
{
    public static float fSpeedConstant = 1f;

}

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
        Dodge();
    }

    #region State Var
    protected State cIdleState = new State("idleState");
    protected State cMoveState = new State("moveState");
    protected State cDodgeState = new State("dodgeState");
    protected State cToStandState = new State("toStand");
    protected State cQSkillState = new State("qSkill");
    protected State cWSkillState = new State("wSkill");
    protected State cESkillState = new State("eSkill");
    protected State cTagState = new State("tagState");
    protected State[] cNormalAttack = new State[5] { new State("normalAttack1"), new State("normalAttack2"), new State("normalAttack3"), new State("normalAttack4"), new State("normalAttack5") };
    #endregion

    #region Move Var
    protected bool isMoving = false;
    private bool isHoldMoveDuringAttack = false;

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
    protected bool canNextAttack = true;
    protected bool isNormalAttackState;

    private Coroutine corouAttackCancelCoroutine;
    protected InputAction normalAttackAction;
    #endregion

    #region Fever Var
    private bool isFeverTime = false;
    protected float fCoolDownCutAndRestoreTime = 2f;
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
        cQSkillState.onEnter += () => { ChangeAnimation(cQSkillState.strStateName); Player.instance.canTag = false; };
        cWSkillState.onEnter += () => { ChangeAnimation(cWSkillState.strStateName); Player.instance.canTag = false; };
        cESkillState.onEnter += () => { ChangeAnimation(cESkillState.strStateName); Player.instance.canTag = false; };
    }

    protected void StateInitalizeOnExit()
    {
        cQSkillState.onExit += () => { Player.instance.canTag = true; };
        cWSkillState.onExit += () => { Player.instance.canTag = true; };
        cESkillState.onExit += () => { Player.instance.canTag = true; };
        cNormalAttack[0].onExit += () => { if (isMoving == true) eMouseState = mouseState.Hold; };
        cNormalAttack[1].onExit += () => { if (isMoving == true) eMouseState = mouseState.Hold; };
        cNormalAttack[2].onExit += () => { if (isMoving == true) eMouseState = mouseState.Hold; };
        cNormalAttack[3].onExit += () => { if (isMoving == true) eMouseState = mouseState.Hold; };
        cNormalAttack[4].onExit += () => { if (isMoving == true) eMouseState = mouseState.Hold; };
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

            if (cStateMachine.GetCurrentState() == cQSkillState ||
                cStateMachine.GetCurrentState() == cWSkillState ||
                cStateMachine.GetCurrentState() == cESkillState)
            {
                isMoving = false;
                eMouseState = mouseState.None;
            }

            if (eMouseState == mouseState.Hold &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                cStateMachine.GetCurrentState() != cESkillState)
            {
                isMoving = false;
                eMouseState = mouseState.None;
                if (gameObject.activeSelf == true)
                {
                    cStateMachine.ChangeState(cIdleState);
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

            if (eMouseState == mouseState.None && isMoving == false &&
                cStateMachine.GetCurrentState() != cNormalAttack[nNormalAttackCount] &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                cStateMachine.GetCurrentState() != cESkillState &&
                Player.instance.cCurrentCharacter == this &&
                canNextAttack)
            {
                FeverGauge.Instance.IncreaseNormalAttackFeverGauge();
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
            }
            else if ((eMouseState == mouseState.Click || eMouseState == mouseState.None) && isHoldMoveDuringAttack == false && 
                cStateMachine.GetCurrentState() != cNormalAttack[nNormalAttackCount] &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                cStateMachine.GetCurrentState() != cESkillState &&
                Player.instance.cCurrentCharacter == this &&
                canNextAttack)
            {
                isMoving = false;
                FeverGauge.Instance.IncreaseNormalAttackFeverGauge();
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
            }
            else if ((eMouseState == mouseState.Hold || eMouseState == mouseState.None) && isMoving == true &&
                cStateMachine.GetCurrentState() != cNormalAttack[nNormalAttackCount] &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkillState &&
                cStateMachine.GetCurrentState() != cWSkillState &&
                cStateMachine.GetCurrentState() != cESkillState &&
                Player.instance.cCurrentCharacter == this &&
                canNextAttack)
            {
                isHoldMoveDuringAttack = true;
                FeverGauge.Instance.IncreaseNormalAttackFeverGauge();
                eMouseState = mouseState.None;
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
                GameManager.instance.AsynchronousExecution(StartAttackCancelTimerWhenHoldMove());
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
                    cStateMachine.GetCurrentState() != cDodgeState)
                {
                    if (cStateMachine.GetCurrentState() != cMoveState)
                    {
                        cStateMachine.ChangeState(cMoveState);
                    }
                    mousePosOnVirtualGround = GetPositionOnVirtualGround();
                    transform.localRotation = GetMouseAngle();
                    transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * Constants.fSpeedConstant * fMoveSpeed);
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
            transform.position = Vector3.MoveTowards(transform.position, mousePosOnGround, Time.deltaTime * Constants.fSpeedConstant * fMoveSpeed);

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
        if (Player.instance.isDodging && cStateMachine.GetCurrentState() == cDodgeState)
        {
            Player.instance.fDodgePlayTimer = Player.instance.fDodgePlayTime;
            transform.localRotation = GetMouseAngle();
            GameManager.instance.AsynchronousExecution(DodgeCoroutine());
            Player.instance.isDodging = false;
        }
    }

    private IEnumerator DodgeCoroutine()
    {
        yield return new WaitUntil(() => DoDodge() <= 0f);
        Player.instance.fDodgePlayTimer = Player.instance.fDodgePlayTime;
        ReturnToIdleWithHold();
    }

    private float DoDodge()
    {
        transform.position += transform.forward * Time.deltaTime * Constants.fSpeedConstant * Player.instance.fDodgePower;
        return Player.instance.fDodgePlayTimer -= Time.deltaTime * Constants.fSpeedConstant;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.started &&
            cStateMachine.GetCurrentState() != cDodgeState &&
            cStateMachine.GetCurrentState() != cTagState &&
            cStateMachine.GetCurrentState() != cQSkillState &&
            cStateMachine.GetCurrentState() != cWSkillState &&
            cStateMachine.GetCurrentState() != cESkillState &&
            DodgeGauge.instance.IsUsedDodge() == true &&
            Player.instance.CanDodge())
        {
            cStateMachine.ChangeState(cDodgeState);
            Player.instance.isDodging = true;
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
        transform.localRotation = GetMouseAngle();
        yield return new WaitUntil(() => DoMoveOnBySkill(power) <= 0f);
    }

    private float DoMoveOnBySkill(float power)
    {
        transform.position += transform.forward * Time.deltaTime * Constants.fSpeedConstant * power;
        return fMoveOnBySkillTimer -= Time.deltaTime * Constants.fSpeedConstant;
    }
    #endregion

    #region Normal Attack Part
    protected IEnumerator StartAttackCancelTimerWhenHoldMove()
    {
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
        if (isNormalAttackState == false)
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
            isHoldMoveDuringAttack = false;
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
            fNormalAttackCancelTimer += Time.deltaTime * Constants.fSpeedConstant;
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
            Player.instance.fTagTimer += Time.deltaTime * Constants.fSpeedConstant;
            yield return null;
        }
        Player.instance.canTag = true;
        Player.instance.fTagTimer = Player.instance.fTagCoolDown;
    }
    #endregion

    #region Fever Part
    public virtual void OnFever(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            UIManager.instance.OnSkillBtn(KeyCode.R);
        }
        
        if (ctx.started && FeverGauge.Instance.IsDoubleFeverGaugeFull() == true)
        {
            Constants.fSpeedConstant = 2f;
            Player.instance.GetTwinSword().cAnimator.speed = Constants.fSpeedConstant;
            Player.instance.GetGreatSword().cAnimator.speed = Constants.fSpeedConstant;
            Player.instance.GetTwinSword().CutCoolDown(fCoolDownCutAndRestoreTime);
            Player.instance.GetGreatSword().CutCoolDown(fCoolDownCutAndRestoreTime);
            Player.instance.GetTwinSword().SetIsFeverTime(true);
            Player.instance.GetGreatSword().SetIsFeverTime(true);
            GameManager.instance.AsynchronousExecution(FeverGauge.Instance.StartDoubleFeverTime());
        }
    }
    public bool IsFeverTime()
    {
        return isFeverTime;
    }
    public void SetIsFeverTime(bool isFeverTime)
    {
        this.isFeverTime = isFeverTime;
    }
    public float GetCoolDownCutAndRestoreTime()
    {
        return fCoolDownCutAndRestoreTime;
    }
    public void CutCoolDown(float cutTime)
    {
        if (isFeverTime == false)
        {

            srtQSkill.fSkillCoolDown /= cutTime;
            fQSkillTimer = srtQSkill.fSkillCoolDown;

            srtWSkill.fSkillCoolDown /= cutTime;
            fWSkillTimer = srtQSkill.fSkillCoolDown;

            srtESkill.fSkillCoolDown /= cutTime;
            fESkillTimer = srtQSkill.fSkillCoolDown;
        }
    }
    public void RestoreCoolDown(float restoreTime)
    {
        if (isFeverTime == true)
        {

            srtQSkill.fSkillCoolDown *= restoreTime;
            fQSkillTimer = srtQSkill.fSkillCoolDown;

            srtWSkill.fSkillCoolDown *= restoreTime;
            fWSkillTimer = srtQSkill.fSkillCoolDown;

            srtESkill.fSkillCoolDown *= restoreTime;
            fESkillTimer = srtQSkill.fSkillCoolDown;
        }
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
            || (isMoving == true && isNormalAttackState == true))
        {
            isHoldMoveDuringAttack = false;
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
            else
            {
                return transform.position;
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