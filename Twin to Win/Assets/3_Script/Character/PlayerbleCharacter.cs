using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerbleCharacter : Character
{
	private Vector3 vecTarget;

	private bool isRightButtonDown = false;

    private State cIdleState = new State("idleState");
    private State cMoveState = new State("moveState");
    private State cDashState = new State("dashState");

    private void Awake()
    {
        cStateMachine = GetComponent<StateMachine>();
        cAnimator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        StateInitalizeOnEnter();

    }

    private void Update()
    {
        Move();
        ReturnToIdle();
    }

    private void StateInitalizeOnEnter()
    {
        cIdleState.onEnter += () => { ChangeAnimation(cIdleState.strStateName); };
        cMoveState.onEnter += () => { ChangeAnimation(cMoveState.strStateName); };
        cDashState.onEnter += () => { ChangeAnimation(cDashState.strStateName); };
        cStateMachine.ChangeState(cIdleState);
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


        if (isRightButtonDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                vecTarget = hit.point;

                

                if (cStateMachine.GetCurrentState().strStateName == "idleState")
                {
                    cStateMachine.ChangeState(cMoveState);
                }
            }



        }


    }

    public void OnMove(InputAction.CallbackContext context)
    {
        isRightButtonDown = context.ReadValueAsButton();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        cStateMachine.ChangeState(cDashState);
    }

    private void ReturnToIdle()
    {
        if (isRightButtonDown == false && cStateMachine.GetCurrentState() == cMoveState)
        {
            cStateMachine.ChangeState(cIdleState);
        }
    }


}
