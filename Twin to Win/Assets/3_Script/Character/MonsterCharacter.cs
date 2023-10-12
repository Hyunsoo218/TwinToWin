using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterCharacter : Character
{
	public static List<MonsterCharacter> allMonsterCharacters = new List<MonsterCharacter>();

	[SerializeField] protected float fAttackDistance;
	[SerializeField] protected float fAttackDelayTime = 3f;
	[SerializeField] protected GameObject objAttackEffectPrefab;
	[SerializeField] private Vector3 hpbarOffset;

	protected SkinnedMeshRenderer cSMR;
	protected NavMeshAgent cAgent;
	protected State cStateIdle = new State("Idle");
	protected State cStateMove = new State("Move");
	protected State cStateAttack = new State("Attack");
	protected State cStateDamage = new State("Damage");
	protected State cStateDie = new State("Die");
	protected bool bCanAttack = true;
	protected Vector3 vTargetPos;
	protected float fTargetDist = 99f;
	protected Material mDefaultMaterial;
	protected Coroutine coAttackDelay;

	private bool isEnterMonsterDeath = false;

	protected virtual void Awake()
	{
		allMonsterCharacters.Add(this);
        cSMR = GetComponentInChildren<SkinnedMeshRenderer>();
		cStateMachine = GetComponent<StateMachine>();
		cAnimator = GetComponent<Animator>();
		cAgent = GetComponent<NavMeshAgent>();
		StateInitializeOnEnter();
		StateInitializeOnStay();
		StateInitializeOnExit();
		mDefaultMaterial = cSMR.material;
		fMaxHealthPoint = fHealthPoint;
	}
	protected void Start()
    {
        cAgent.isStopped = true;
		StartCoroutine(SetTarget());
		InsertHpbar();
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
			cSMR.material = EffectManager.instance.GetHitEffect();
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
	protected virtual void StateInitializeOnExit()
	{
		cStateMove.onExit = () => {
			cAgent.isStopped = true;
			cAgent.velocity = Vector3.zero;
		};
		cStateDamage.onExit = () => {
			cSMR.material = mDefaultMaterial;
			if (coAttackDelay != null)
			{
				StopCoroutine(coAttackDelay);
			}
			coAttackDelay = StartCoroutine(AttackDelay());
		};
		cStateAttack.onExit = () => {
			if (coAttackDelay != null)
			{
				StopCoroutine(coAttackDelay);
			}
			coAttackDelay = StartCoroutine(AttackDelay());
		};
	}

	public void StartAction() 
	{
        ChangeState(cStateIdle);
        cAnimator.speed = 1f;
    }
	public void StopAction() 
	{
		ChangeState(null);
		cAnimator.speed = 0;
    }
	public void Slow(float time, float amount) 
	{
		StartCoroutine(SlowAction(time, amount));
	}
	protected IEnumerator SlowAction(float time, float amount) 
	{
		ChangeState(cStateIdle);
		float currentSpeed = cAgent.speed;
		cAnimator.speed *= amount;
		cAgent.speed *= amount;
		yield return new WaitForSeconds(time);
		cAnimator.speed = 1f;
		cAgent.speed = currentSpeed;
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
	public virtual void EnableEffect()
	{
		if (cStateMachine.GetCurrentState() != cStateAttack) return;
		GameObject objEffect = EffectManager.instance.GetEffect(objAttackEffectPrefab);
		objEffect.GetComponent<Effect>().OnAction(transform, fPower, 1 << 8);
	}

	protected virtual IEnumerator SetTarget()
	{
		while (true)
		{
			vTargetPos = Player.instance.cCurrentCharacter.transform.position;
			vTargetPos.y = 0;
			if (cAgent.enabled)
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
		transform.LookAt(vTargetPos);
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
	}
	public override void ChangeState(State cNextState)
	{
		cStateMachine.ChangeState(cNextState);
	}
	public override void Damage(float fAmount)
	{
		if (cStateMachine.GetCurrentState() == cStateDie) return;

		fHealthPoint -= fAmount;
        UIManager.instance.SetHp(this);
        if (fHealthPoint <= 0)
		{
			Die();
			isEnterMonsterDeath = true;
		}
		else
		{
			ChangeState(cStateDamage);
		}
	}
	public override void Die()
	{
		allMonsterCharacters.Remove(this);
        ChangeState(cStateDie);
        cAgent.enabled = false;
		GetComponent<Collider>().enabled = false;
        UIManager.instance.RemoveHpbar(this);
    }
	public override void Move()
	{
		if(cAgent.enabled)
			cAgent.isStopped = false;
	}
	public override void ChangeAnimation(string strTrigger)
	{
		if (cStateMachine.GetPrevState() != null)
			cAnimator.ResetTrigger(cStateMachine.GetPrevState().strStateName);
		cAnimator.SetTrigger(strTrigger);
	}
	protected virtual void InsertHpbar() 
	{
        UIManager.instance.InsertHpbar(this, hpbarOffset);
    }

	public bool GetIsEnterMonsterDead()
	{
		return isEnterMonsterDeath;
	}
	public void SetIsEnterMonsterDead(bool isMonsterDead)
	{
		this.isEnterMonsterDeath = isMonsterDead;
	}
}
