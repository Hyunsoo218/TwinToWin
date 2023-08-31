using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCharacter : Character
{
	private State cStateIdle = new State("Idle");
	private State cStateMove = new State("Move");
	private State cStateAttack = new State("Attack");
	private State cStateDamage = new State("Damage");
	private State cStateDie = new State("Die");
	private void Awake()
	{
		cStateMachine = GetComponent<StateMachine>();
		cAnimator = GetComponent<Animator>();
		StateInitializeOnEnter();
		StateInitializeOnStay();
		StateInitializeOnExit();
	}
	private void StateInitializeOnEnter()
	{
		cStateIdle.onEnter = () => {
			ChangeAnimation(cStateIdle.strStateName);
		};
		cStateMove.onEnter = () => {
			ChangeAnimation(cStateMove.strStateName);
		};
		cStateAttack.onEnter = () => {
			ChangeAnimation(cStateAttack.strStateName);
		};
		cStateDamage.onEnter = () => {
			ChangeAnimation(cStateDamage.strStateName);
		};
		cStateDie.onEnter = () => {
			ChangeAnimation(cStateDie.strStateName);
		};
	}
	private void StateInitializeOnStay()
	{

	}
	private void StateInitializeOnExit()
	{

	}

	public override void Attack()
	{

	}
	public override void ChangeState(State cNextState)
	{

	}
	public override void Damage(float fAmount)
	{

	}
	public override void Die()
	{

	}
	public override void Move()
	{

	}
	public override void ChangeAnimation(string strTrigger)
	{
		cAnimator.ResetTrigger(cStateMachine.GetCurrentState()?.strStateName);
		cAnimator.SetTrigger(strTrigger);
	}
}
