using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterCharacter : Character
{
	[SerializeField] private float fAttackDistance;
	[SerializeField] private float fAttackDelayTime = 3f;

	private NavMeshAgent cAgent;
	private State cStateIdle = new State("Idle");
	private State cStateMove = new State("Move");
	private State cStateAttack = new State("Attack");
	private State cStateDamage = new State("Damage");
	private State cStateDie = new State("Die");
	private bool bCanAttack = true;
	private Vector3 vTargetPos;
	private float fTargetDist = 99f;

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
	private void Start()
	{
		StartCoroutine(SetTarget());
	}
	private void StateInitializeOnEnter()
	{
		cStateIdle.onEnter = () => {
			ChangeAnimation(cStateIdle.strStateName);
		};
		cStateMove.onEnter = () => {
			ChangeAnimation(cStateMove.strStateName);
			cAgent.isStopped = false;
		};
		cStateAttack.onEnter = () => {
			ChangeAnimation(cStateAttack.strStateName);
			Attack();
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
		cStateIdle.onStay = () => {
			if (fTargetDist >= fAttackDistance)
			{
				ChangeState(cStateMove);
			}
			else
			{
				if (bCanAttack)
				{
					ChangeState(cStateAttack);
				}
			}
		};
		cStateMove.onStay = () => {
			if (fTargetDist < fAttackDistance)
			{
				if (bCanAttack)
				{
					ChangeState(cStateAttack);
				}
				else
				{
					ChangeState(cStateIdle);
				}
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

	private IEnumerator SetTarget()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.05f);
			vTargetPos = Player.instance.cCurrentCharacter.transform.position;
			cAgent.SetDestination(vTargetPos);
			fTargetDist = Vector3.Distance(transform.position, vTargetPos);
		}
	}
	private IEnumerator AttackDelay()
	{
		bCanAttack = false;
		float fWaitTime = Random.Range(fAttackDelayTime - 1f , fAttackDelayTime + 1f);
		yield return new WaitForSeconds(fWaitTime);
		bCanAttack = true;
	}
	public override void Attack()
	{
		StartCoroutine(AttackDelay());
		transform.LookAt(vTargetPos);
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		// ¿À¹ö·¾ ÇÏ±â
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
