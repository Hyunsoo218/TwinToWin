using System;
using System.Collections;
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
    #endregion

    #region Move
    private float fMouseTimer = 0f;
    private float fDistanceToPlane;

    private bool isMoving = false;

    Coroutine moveCoroutine;
    InputAction moveAction;
    mouseState eMouseState;

    Plane virtualGround = new Plane(Vector3.up, 0);
    private Vector3 mousePosOnVirtualGround;
    private Vector3 mousePosOnGround;
    #endregion

    private float fDodgeTimer;

    private void Awake()
    {
        Initialize();
        StateInitalizeOnEnter();
        MouseStateInitialize();
    }


    private void FixedUpdate()
    {
        if (moveAction.IsPressed())
        {
            fMouseTimer += Time.deltaTime;
        }
        Move();
        Dodge();
    }
    private void Initialize()
    {
        moveAction = GetComponent<PlayerInput>().actions["Move"];
        cStateMachine = GetComponent<StateMachine>();
        cAnimator = GetComponent<Animator>();
        fDodgeTimer = fDodgeCoolTime;
    }

    private void StateInitalizeOnEnter()
    {
        cIdleState.onEnter += () => { ChangeAnimation(cIdleState.strStateName); };
        cMoveState.onEnter += () => { ChangeAnimation(cMoveState.strStateName); };
        cDodgeState.onEnter += () => { ChangeAnimation(cDodgeState.strStateName); };
    }

    private void MouseStateInitialize()
    {
        moveAction.performed += ctx =>
        {
            if (fMouseTimer >= 0.1f && eMouseState == mouseState.None)
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
                ChangeAnimation("toStand");
            }
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
                moveCoroutine = StartCoroutine(DoMove(mousePosOnGround));

                fMouseTimer = 0f;
                eMouseState = mouseState.None;
                break;
            case mouseState.Hold:
                if (cStateMachine.GetCurrentState() != cMoveState && cStateMachine.GetCurrentState() != cDodgeState)
                {
                    cStateMachine.ChangeState(cMoveState);
                }
                mousePosOnVirtualGround = GetPositionOnVirtualGround();

                transform.localRotation = GetMouseAngle();
                transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * fMoveSpeed);
                break;
        }
    }
    private IEnumerator DoMove(Vector3 _mousePosOnGround)
    {
        isMoving = true;
        while (eMouseState != mouseState.Hold && cStateMachine.GetCurrentState() == cMoveState)
        {
            if (Vector3.Distance(transform.position, _mousePosOnGround) <= 0.1f)
            {
                ChangeAnimation("toStand");
                isMoving = false;
                yield break;
            }
            transform.position = Vector3.MoveTowards(transform.position, _mousePosOnGround, Time.deltaTime * fMoveSpeed);

            yield return null;
        }
        isMoving = false;
    }
    public void Dodge()
    {
        // 여기가 반복됨
        if (cStateMachine.GetCurrentState() == cDodgeState && fDodgeTimer >= fDodgeCoolTime)
        {
            StartCoroutine(DoDodge());
        }
    }

    private IEnumerator DoDodge()
    {
        transform.position += transform.forward * Time.deltaTime * fDodgePower;
        yield return new WaitForSeconds(fDodgePlayTime);
        ChangeAnimation("toStand");
    }

    private IEnumerator DodgeCoolDown()
    {
        cStateMachine.ChangeState(cDodgeState);
        while (fDodgeTimer >= 0)
        {
            fDodgeTimer -= Time.deltaTime;
            yield return null;
        }
        fDodgeTimer = fDodgeCoolTime;
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


    public void OnDash(InputAction.CallbackContext context)
    {
        if (cStateMachine.GetCurrentState() != cDodgeState && fDodgeTimer >= fDodgeCoolTime)
        {
            StartCoroutine(DodgeCoolDown());
        }
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
