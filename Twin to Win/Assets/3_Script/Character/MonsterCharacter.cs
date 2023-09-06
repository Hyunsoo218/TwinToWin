using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterCharacter : Character
{
	[SerializeField] protected float fAttackDistance;
	[SerializeField] protected float fAttackDelayTime = 3f;
	[SerializeField] protected GameObject objAttackEffectPrefab;
	
	protected NavMeshAgent cAgent;
	protected State cStateIdle = new State("Idle");
	protected State cStateMove = new State("Move");
	protected State cStateAttack = new State("Attack");
	protected State cStateDamage = new State("Damage");
	protected State cStateDie = new State("Die");
	protected bool bCanAttack = true;
	protected Vector3 vTargetPos;
	protected float fTargetDist = 99f;

	protected void Awake()
	{
		cStateMachine = GetComponent<StateMachine>();
		cAnimator = GetComponent<Animator>();
		cAgent = GetComponent<NavMeshAgent>();
		StateInitializeOnEnter();
		StateInitializeOnStay();
		StateInitializeOnExit();
		ChangeState(cStateIdle);
		cAgent.isStopped = true;
	}
	protected void Start()
	{
		StartCoroutine(SetTarget());
	}
	protected virtual void StateInitializeOnEnter()
	{
		cStateIdle.onEnter = () => {
			ChangeAnimation(cStateIdle.strStateName);
		};
		cStateMove.onEnter = () => {
			ChangeAnimation(cStateMove.strStateName);
			Move();
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
	protected virtual void StateInitializeOnStay()
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
	protected void StateInitializeOnExit()
	{
		cStateMove.onExit = () => {
			cAgent.isStopped = true;
			cAgent.velocity = Vector3.zero;
		};
	}

	public void ResetState()
	{
		if (fTargetDist < fAttackDistance)
		{
			ChangeState(cStateIdle);
		}
		else
		{
			ChangeState(cStateMove);
		}
	}
	public void EnableEffect()
	{
		GameObject objEffect = EffectManager.instance.GetEffect(objAttackEffectPrefab);
		objEffect.GetComponent<Effect>().OverlapBox(transform, fPower, 1 << 8);
	}

	protected IEnumerator SetTarget()
	{
		while (true)
		{
			vTargetPos = Player.instance.cCurrentCharacter.transform.position;
			cAgent.SetDestination(vTargetPos);
			fTargetDist = Vector3.Distance(transform.position, vTargetPos);
			yield return new WaitForSeconds(0.25f);
		}
	}
	protected IEnumerator AttackDelay()
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
		cAgent.isStopped = false;
	}
	public override void ChangeAnimation(string strTrigger)
	{
		if (cStateMachine.GetPrevState() != null)
			cAnimator.ResetTrigger(cStateMachine.GetPrevState().strStateName);
		cAnimator.SetTrigger(strTrigger);
	}
}
