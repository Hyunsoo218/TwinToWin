using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;
enum mouseState
{
    None,
    Click,
    Hold
};

public class PlayerbleCharacter : Character
{
    [Header("Character Info")]
    [SerializeField] private float fMoveSpeed = 3f;
    [SerializeField] private float fDodgePower = 30f;
    [SerializeField] private float fDodgePlayTime = 0.1f;
    [SerializeField] private float fDodgeCoolTime = 3f;
    
    #region State
    private State cIdleState = new State("idleState");
    private State cMoveState = new State("moveState");
    private State cDodgeState = new State("dodgeState");
    private State cToStand = new State("toStand");
    private State[] cNormalAttack = new State[3] { new State("normalAttack1"), new State("normalAttack2"), new State("normalAttack3") };
    #endregion

    #region Move
    private float fMouseTimer;
    private float fDistanceToPlane;

    private bool isMoving = false;

    Coroutine moveCoroutine;
    InputAction moveAction;
    mouseState eMouseState;

    Plane virtualGround = new Plane(Vector3.up, 0);
    private Vector3 mousePosOnVirtualGround;
    private Vector3 mousePosOnGround;
    #endregion

    #region Dodge
    private float fDodgeTimer;
    private float fDodgePlayTimer;
    private bool isDodging = false;
    #endregion

    #region NormalAttack
    private int nNormalAttackCount = 0;
    private float fNormalAttackCancelTimer = 0f;
    private float fNormalAttackCancelTime = 2f;
    private bool isWaitForNextAttack = false;
    private bool canNextAttack = true;
    private bool isNotNormalAttackState;

    private Coroutine normalAttackCancelTimer;
    private InputAction normalAttackAction;
    #endregion

    private void Awake()
    {
        Initialize();
        StateInitalizeOnEnter();
        RightMouseStateInitialize();
        LeftMouseStateInitalize();
    }
    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Move();
        Dodge();
        NormalAttack();
    }
    private void Initialize()
    {
        moveAction = GetComponent<PlayerInput>().actions["Move"];
        normalAttackAction = GetComponent<PlayerInput>().actions["NormalAttack"];
        cStateMachine = GetComponent<StateMachine>();
        cAnimator = GetComponent<Animator>();
    }

    private void StateInitalizeOnEnter()
    {
        cIdleState.onEnter += () => { ChangeAnimation(cIdleState.strStateName); };
        cMoveState.onEnter += () => { ChangeAnimation(cMoveState.strStateName); };
        cDodgeState.onEnter += () => { ChangeAnimation(cDodgeState.strStateName); };
        cToStand.onEnter += () => { ChangeAnimation(cToStand.strStateName); };
        cNormalAttack[0].onEnter += () => { ChangeAnimation(cNormalAttack[0].strStateName); };
        cNormalAttack[1].onEnter += () => { ChangeAnimation(cNormalAttack[1].strStateName); };
        cNormalAttack[2].onEnter += () => { ChangeAnimation(cNormalAttack[2].strStateName); };
    }

    private void RightMouseStateInitialize()
    {
        moveAction.performed += ctx =>
        {
            if (fMouseTimer >= 0.1f && eMouseState == mouseState.None )
                eMouseState = mouseState.Hold;
        };
        moveAction.canceled += ctx =>
        {
            if (eMouseState == mouseState.Hold == false && eMouseState == mouseState.None)
                eMouseState = mouseState.Click;

            if (eMouseState == mouseState.Hold)
            {
                fMouseTimer = 0f;
                eMouseState = mouseState.None;
                cStateMachine.ChangeState(cToStand);
            }
        };
    }
    // Hold 상태일 때 Attack하면 멈춘 다음 Attack 다 끝나고 다시 Hold로 돌아갈건지 의논
    private void LeftMouseStateInitalize()
    {
        normalAttackAction.started += ctx =>
        {
            if (fNormalAttackCancelTimer > 0f)
            {
                isWaitForNextAttack = true;
            }

            if (eMouseState != mouseState.Hold && cStateMachine.GetCurrentState() != cNormalAttack[nNormalAttackCount] && cStateMachine.GetCurrentState() != cDodgeState && canNextAttack)
            {
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
            }
        };
        normalAttackAction.canceled += ctx =>
        {

        };
    }

    public override void Attack()
    {

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

    public override void ChangeAnimation(string strTrigger)
    {
        if (cStateMachine.GetPrevState() != null)
            cAnimator.ResetTrigger(cStateMachine.GetPrevState().strStateName);
        cAnimator.SetTrigger(strTrigger);
    }

    private  void ReturnToIdle()
    {
        cStateMachine.ChangeState(cIdleState);
    }


    public override void Move()
    {
        IncreaseMousePressTime();

        switch (eMouseState)
        {
            case mouseState.Click:
                if (cStateMachine.GetCurrentState() != cMoveState && cStateMachine.GetCurrentState() != cDodgeState)
                {
                    cStateMachine.ChangeState(cMoveState);
                }
                mousePosOnGround = GetPositionOnGround();
                transform.localRotation = GetMouseAngle();

                if (isMoving)
                {
                    StopCoroutine(moveCoroutine);
                }
                moveCoroutine = StartCoroutine(MoveCoroutine(mousePosOnGround));

                fMouseTimer = 0f;
                eMouseState = mouseState.None;
                break;
            case mouseState.Hold:
                // 홀드로 이동하다가 바로 공격키 누르면 toStand가 계속 켜져있음
                if (cStateMachine.GetCurrentState() != cMoveState && cStateMachine.GetCurrentState() != cDodgeState )
                {
                    cStateMachine.ChangeState(cMoveState);
                }

                if (cStateMachine.GetCurrentState() == cMoveState)
                {
                    mousePosOnVirtualGround = GetPositionOnVirtualGround();

                    transform.localRotation = GetMouseAngle();
                    transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * fMoveSpeed);
                }
                break;
        }
    }
    private void IncreaseMousePressTime()
    {
        if (moveAction.IsPressed())
        {
            fMouseTimer += Time.deltaTime;
        }
    }
    private IEnumerator MoveCoroutine(Vector3 mousePosOnGround)
    {
        isMoving = true;
        while (eMouseState != mouseState.Hold && cStateMachine.GetCurrentState() == cMoveState)
        {
            if (Vector3.Distance(transform.position, mousePosOnGround) <= 0.1f)
            {
                cStateMachine.ChangeState(cToStand);
                isMoving = false;
                yield break;
            }
            transform.position = Vector3.MoveTowards(transform.position, mousePosOnGround, Time.deltaTime * fMoveSpeed);

            yield return null;
        }
        isMoving = false;
    }
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
        if (cStateMachine.GetCurrentState() != cDodgeState && fDodgeTimer <= 0f)
        {
            cStateMachine.ChangeState(cDodgeState);
            StartCoroutine(StartDodgeCoolDown());
            isDodging = true;
        }
    }

    public void NormalAttack()
    {
        isNotNormalAttackState = cStateMachine.GetCurrentState() != cNormalAttack[0] && cStateMachine.GetCurrentState() != cNormalAttack[1] && cStateMachine.GetCurrentState() != cNormalAttack[2];
        IncreaseAttackCount();
        ResetAttackCount();
        ExceededCancelTime();
    }

    private void IncreaseAttackCount()
    {
        if (cStateMachine.GetCurrentState() == cNormalAttack[nNormalAttackCount] && fNormalAttackCancelTimer < fNormalAttackCancelTime)
        {
            nNormalAttackCount = nNormalAttackCount < cNormalAttack.Length - 1 ? ++nNormalAttackCount : 0;
        }
    }

    private void ResetAttackCount()
    {
        if (isNotNormalAttackState)
        {
            canNextAttack = true;
            nNormalAttackCount = 0;
        }
    }

    private void ExceededCancelTime()
    {
        if (fNormalAttackCancelTimer >= fNormalAttackCancelTime - 0.05f)
        {
            ReturnToIdle();
        }
    }

    private void DisableNextAttack()
    {
        canNextAttack = false;
    }

    private void ActiveNextAttack()
    {
        normalAttackCancelTimer = StartCoroutine(StartNormalAttackCancelTimer());
        canNextAttack = true;
    }

    private IEnumerator StartNormalAttackCancelTimer()
    {
        while (fNormalAttackCancelTimer < fNormalAttackCancelTime)
        {
            if (isWaitForNextAttack == true)
            {
                fNormalAttackCancelTimer = 0f;
                isWaitForNextAttack = false;
                yield break;
            }
            fNormalAttackCancelTimer += Time.deltaTime;
            yield return null;
        }
        
        fNormalAttackCancelTimer = 0f;
        isWaitForNextAttack = false;
    }

    private Vector3 GetPositionOnVirtualGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = Vector3.one;
        if (virtualGround.Raycast(ray, out fDistanceToPlane))
        {
            pos = ray.GetPoint(fDistanceToPlane);
        }
        return pos;
    }
    private Vector3 GetPositionOnGround()
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
    private Quaternion GetMouseAngle()
    {
        double angle = Math.Atan2(GetMouseNormalizedXPosition(), GetMouseNormalizedYPosition()) * 180 / Math.PI;
        double targetAngle = angle < 0f ? angle + 360f : angle;

        return Quaternion.Euler(new Vector3(transform.rotation.x, (float)targetAngle + Camera.main.transform.localEulerAngles.y, transform.rotation.z));
    }

    private float GetMouseNormalizedXPosition()
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

    private float GetMouseNormalizedYPosition()
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
