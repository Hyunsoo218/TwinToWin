using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCharacter : Character
{
	[SerializeField] private float fAttackDistance;
	private NavMeshAgent cAgent;
	private State cStateIdle = new State("Idle");
	private State cStateMove = new State("Move");
	private State cStateAttack = new State("Attack");
	private State cStateDamage = new State("Damage");
	private State cStateDie = new State("Die");

	private void Awake()
	{
		cStateMachine = GetComponent<StateMachine>();
		cAnimator = GetComponent<Animator>();
		cAgent = GetComponent<NavMeshAgent>();
		StateInitializeOnEnter();
		StateInitializeOnStay();
		StateInitializeOnExit();
		ChangeState(cStateIdle);
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			cAgent.SetDestination(new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f)));
			ChangeState(cStateMove);
		}
	}

	private void StateInitializeOnEnter()
	{
		cStateIdle.onEnter = () => {
			ChangeAnimation(cStateIdle.strStateName);
		};
		cStateMove.onEnter = () => {
			cAgent.isStopped = false;
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
		cStateMove.onStay = () => {
			float fDist = Vector3.Distance(transform.position, cAgent.destination);
			if (fDist <= fAttackDistance)
			{
				ChangeState(cStateAttack);
			}
		};
	}
	private void StateInitializeOnExit()
	{
		cStateMove.onExit = () => {
			cAgent.isStopped = true;
		};
	}
	public void ResetState()
	{
		ChangeState(cStateIdle);
	}

	public override void Attack()
	{

	}
	public override void ChangeState(State cNextState)
	{
		cStateMachine.ChangeState(cNextState);
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
