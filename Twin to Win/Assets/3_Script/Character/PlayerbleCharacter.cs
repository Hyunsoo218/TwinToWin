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
    [SerializeField] private float fDodgeSpeed = 30f;
    [SerializeField] private float fDodgeDistance = 5f;

    private float fTimer = 0f;
    private float fDistanceToPlane;

    private bool isMoving = false;

    #region State
    private State cIdleState = new State("idleState");
    private State cMoveState = new State("moveState");
    private State cDodgeState = new State("dodgeState");
    #endregion

    Coroutine moveCoroutine;
    InputAction moveAction;
    mouseState eMouseState;

    Plane virtualGround = new Plane(Vector3.up, 0);
    private Vector3 mousePosOnVirtualGround;
    private Vector3 mousePosOnGround;

    private Rigidbody characterRid;

    private void Awake()
    {
        moveAction = GetComponent<PlayerInput>().actions["Move"];
        cStateMachine = GetComponent<StateMachine>();
        cAnimator = GetComponent<Animator>();
        characterRid = GetComponent<Rigidbody>();
        StateInitalizeOnEnter();
        MouseStateInlitalize();
    }


    private void FixedUpdate()
    {
        if (moveAction.IsPressed())
        {
            fTimer += Time.deltaTime;
        }
        Move();
        Dodge();
    }

    private void StateInitalizeOnEnter()
    {
        cIdleState.onEnter += () => { ChangeAnimation(cIdleState.strStateName); };
        cMoveState.onEnter += () => { ChangeAnimation(cMoveState.strStateName); };
        cDodgeState.onEnter += () => { ChangeAnimation(cDodgeState.strStateName); };
        cStateMachine.ChangeState(cIdleState);
    }

    private void MouseStateInlitalize()
    {
        moveAction.performed += ctx =>
        {
            if (fTimer >= 0.1f && eMouseState == mouseState.None)
                eMouseState = mouseState.Hold;
        };
        moveAction.canceled += ctx =>
        {
            if (eMouseState == mouseState.Hold == false && eMouseState == mouseState.None)
                eMouseState = mouseState.Click;

            if (eMouseState == mouseState.Hold)
            {
                fTimer = 0f;
                eMouseState = mouseState.None;
                cStateMachine.ChangeState(cIdleState);
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
        cAnimator.ResetTrigger(cStateMachine.GetCurrentState().strStateName);
        cAnimator.SetTrigger(strTrigger);
    }


    public override void Move()
    {
        switch (eMouseState)
        {
            case mouseState.Click:
                mousePosOnGround = GetPositionOnGround();
                transform.localRotation = GetMouseAngle();

                if (isMoving)
                    StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(StartMoveToTarget(mousePosOnGround));

                fTimer = 0f;
                eMouseState = mouseState.None;
                break;
            case mouseState.Hold:
                mousePosOnVirtualGround = GetPositionOnVirtualGround();
                if (cStateMachine.GetCurrentState() != cMoveState)
                    cStateMachine.ChangeState(cMoveState);

                transform.localRotation = GetMouseAngle();
                transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * fMoveSpeed);
                break;
        }
    }

    public void Dodge()
    {
        if (cStateMachine.GetCurrentState() == cDodgeState)
        {
            //transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * fDodgeSpeed);
        }
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
        Vector3 worldPosition = Vector3.zero;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, 1 << 6))
            worldPosition = hit.point;
        else
            worldPosition = transform.position;

        return worldPosition;
    }
    private Quaternion GetMouseAngle()
    {
        double angle = Math.Atan2(GetMouseNormalizedXPosition(), GetMouseNormalizedYPosition()) * 180 / Math.PI;
        double targetAngle = angle < 0f ? angle + 360f : angle;

        return Quaternion.Euler(new Vector3(transform.rotation.x, (float)targetAngle + Camera.main.transform.localEulerAngles.y, transform.rotation.z));
    }
    private IEnumerator StartMoveToTarget(Vector3 _mousePosOnGround)
    {
        isMoving = true;
        cStateMachine.ChangeState(cMoveState);
        while (eMouseState != mouseState.Hold && cStateMachine.GetCurrentState() == cMoveState)
        { 
            if (Vector3.Distance(transform.position, _mousePosOnGround) <= 0.1f)
            {
                cStateMachine.ChangeState(cIdleState);
                isMoving = false;
                yield break;
            }
            transform.position = Vector3.MoveTowards(transform.position, _mousePosOnGround, Time.deltaTime * fMoveSpeed);

            yield return null;
        }
        isMoving = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        cStateMachine.ChangeState(cDodgeState);
        mousePosOnVirtualGround = GetPositionOnVirtualGround();
        transform.localRotation = GetMouseAngle();
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
