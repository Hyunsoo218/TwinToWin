using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Input = UnityEngine.Input;
public enum mouseState
{
    None,
    Click,
    Hold
};
/** 
    버그 리스트
    =E스킬 사용하고 Q 광클하면 Q 두 번 실행 : 쿨타임 주면 해결
    =마우스 클릭 Hold 이동 절묘하게 느림
    =정말 가끔 MoveCoroutine이 Null로 뜸 / Null이 뜨는 조건 모름
    =W하고 E스킬 끝날 때 쯤 우클릭을 하면 스킬이 다시 안 써지는 버그 180번째 줄에 W스킬, E스킬 아닐 때를 지우면 되긴 하는데 대신 스킬 도중 이동이 가능함

    !@#$ 땅바닥을 그냥 넓게 해서 움직이게 할건지 아니면 바닥을 분리해서 움직이게 할건지 결정해야 함
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
    protected Dictionary<string, Skill> dicCurrentSkill = new Dictionary<string, Skill>();

    protected virtual void Awake()
    {
        Initialize();
        StateInitalizeOnEnter();
        InitializeRightMouseState();

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
    protected State cToStand = new State("toStand");
    protected State cQSkill = new State("qSkill");
    protected State cWSkill = new State("wSkill");
    protected State cESkill = new State("eSkill");
    protected State cRSkill = new State("rSkill");
    #endregion

    #region Move
    protected bool isMoving = false;

    Coroutine moveCoroutine;
    InputAction moveAction;
    protected mouseState eMouseState;

    protected Vector3 mousePosOnVirtualGround;
    protected Vector3 mousePosOnGround;
    #endregion

    #region Dodge
    private float fDodgeTimer;
    private float fDodgePlayTimer;
    private bool isDodging = false;
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

    #region Normal Attack
    protected int nNormalAttackCount = 0;
    protected float fNormalAttackCancelTimer = 0f;
    protected float fNormalAttackCancelTime = 2f;
    protected bool canResetDuringCancelTime = false;
    protected bool canNextAttack = true;
    protected bool isNotNormalAttackState;

    protected Coroutine normalAttackCancelTimer;
    protected InputAction normalAttackAction;
    #endregion

    #region Init Part
    protected void Initialize()
    {
        moveAction = GetComponent<PlayerInput>().actions["Move"];
        normalAttackAction = GetComponent<PlayerInput>().actions["NormalAttack"];
        cStateMachine = GetComponent<StateMachine>();
        cAnimator = GetComponent<Animator>();
        dicCurrentSkill.Add("QSkill", srtQSkill);
        dicCurrentSkill.Add("WSkill", srtWSkill);
        dicCurrentSkill.Add("ESkill", srtESkill);
        dicCurrentSkill.Add("RSkill", srtRSkill);
    }

    protected virtual void StateInitalizeOnEnter()
    {
        cIdleState.onEnter += () => { ChangeAnimation(cIdleState.strStateName); };
        cMoveState.onEnter += () => { ChangeAnimation(cMoveState.strStateName); };
        cDodgeState.onEnter += () => { ChangeAnimation(cDodgeState.strStateName); };
        cToStand.onEnter += () => { ChangeAnimation(cToStand.strStateName); };
        cQSkill.onEnter += () => { ChangeAnimation(cQSkill.strStateName); };
        cWSkill.onEnter += () => { ChangeAnimation(cWSkill.strStateName); };
        cESkill.onEnter += () => { ChangeAnimation(cESkill.strStateName); };
        cRSkill.onEnter += () => { ChangeAnimation(cRSkill.strStateName); };
    }

    protected void InitializeRightMouseState()
    {
        moveAction.performed += ctx =>
        {
            if (cStateMachine.GetCurrentState() != cWSkill &&
                cStateMachine.GetCurrentState() != cESkill)
            {
                isMoving = true;
                cStateMachine.ChangeState(cMoveState);
                eMouseState = mouseState.Hold;
            }
        };
        moveAction.canceled += ctx =>
        {
            if (eMouseState == mouseState.None &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cWSkill &&
                cStateMachine.GetCurrentState() != cESkill)
            {
                isMoving = true;
                eMouseState = mouseState.Click;
                cStateMachine.ChangeState(cMoveState);
            }
            if (eMouseState == mouseState.Hold)
            {
                isMoving = false;
                eMouseState = mouseState.None;
                cStateMachine.ChangeState(cToStand);
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
                mousePosOnVirtualGround = GetPositionOnVirtualGround();
                transform.localRotation = GetMouseAngle();
                transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * fMoveSpeed);
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
                cStateMachine.ChangeState(cToStand);
                yield break;
            }
            transform.position = Vector3.MoveTowards(transform.position, mousePosOnGround, Time.deltaTime * fMoveSpeed);

            yield return null;
        }
    }
    #endregion

    #region Dodge Part
    public void Dodge()
    {
        if (isDodging && cStateMachine.GetCurrentState() == cDodgeState)
        {
            fDodgePlayTimer = fDodgePlayTime;
            StartCoroutine(DodgeCoroutine());
            isDodging = false;
        }
    }

    private IEnumerator DodgeCoroutine()
    {
        yield return new WaitUntil(() => DoDodge() <= 0f);
        fDodgePlayTimer = fDodgePlayTime;
        cStateMachine.ChangeState(cToStand);
    }

    private float DoDodge()
    {
        transform.position += transform.forward * Time.deltaTime * fDodgePower;
        return fDodgePlayTimer -= Time.deltaTime;
    }

    private IEnumerator StartDodgeCoolDown()
    {
        while (fDodgeTimer <= fDodgeCoolTime)
        {
            fDodgeTimer += Time.deltaTime;
            yield return null;
        }
        fDodgeTimer = 0f;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && fDodgeTimer <= 0f &&
            cStateMachine.GetCurrentState() != cDodgeState &&
            cStateMachine.GetCurrentState() != cQSkill &&
            cStateMachine.GetCurrentState() != cWSkill &&
            cStateMachine.GetCurrentState() != cESkill &&
            cStateMachine.GetCurrentState() != cRSkill)
        {
            cStateMachine.ChangeState(cDodgeState);
            StartCoroutine(StartDodgeCoolDown());
            isDodging = true;
        }
    }

    #endregion

    #region Skill Part
    private void EnableSkillEffect(string skillName)
    {
        Skill currentSkill = dicCurrentSkill[skillName];
        GameObject obj = EffectManager.instance.GetEffect(currentSkill.objSkillEffect);
        obj.GetComponent<Effect>().OnAction(transform, fPower, 1 << 7);
    }

    private void OnMoveOnBySkill(AnimationEvent skillEvent)   // 애니메이션 이벤트에서 power 수정
    {
        srtCurrentSkill = dicCurrentSkill[(skillEvent.stringParameter)];
        StartCoroutine(StartMoveOnBySkill(skillEvent.floatParameter));
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
        cStateMachine.ChangeState(cToStand);
    }
    protected void KeepHoldMove()
    {
        if (isMoving == true &&
            cStateMachine.GetCurrentState() != cWSkill &&
            cStateMachine.GetCurrentState() != cQSkill &&
            cStateMachine.GetCurrentState() != cESkill &&
            cStateMachine.GetCurrentState() != cRSkill)
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
