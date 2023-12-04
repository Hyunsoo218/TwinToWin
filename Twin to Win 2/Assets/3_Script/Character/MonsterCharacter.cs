using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterCharacter : Character
{
	public static List<MonsterCharacter> allMonsterCharacters = new List<MonsterCharacter>();
	public static List<MonsterCharacter> canTargetingMonsterCharacters = new List<MonsterCharacter>();

	[SerializeField] protected float fMaxHealthPoint;
	[SerializeField] protected float fHealthPoint;
	[SerializeField] protected float fPower;
	[SerializeField] protected float fAttackDistance;
	[SerializeField] protected float fAttackDelayTime = 3f;
	[SerializeField] protected GameObject objAttackEffectPrefab;
	[SerializeField] private Vector3 hpbarOffset;

	protected SkinnedMeshRenderer cSMR;
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
	protected float defultSpeed;

	private bool isEnterMonsterDeath = false;

	protected virtual void Awake()
	{
        cSMR = GetComponentInChildren<SkinnedMeshRenderer>();
		cStateMachine = GetComponent<StateMachine>();
		cAnimator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		StateInitializeOnEnter();
		StateInitializeOnStay();
		StateInitializeOnExit();
		mDefaultMaterial = cSMR.material;
		defultSpeed = agent.speed;
	}
    protected void OnEnable()
    {
        GetComponent<Collider>().enabled = true;
        agent.enabled = true;
        allMonsterCharacters.Add(this);
		canTargetingMonsterCharacters.Add(this);
        agent.isStopped = true;
        StartCoroutine(SetTarget());
        InsertHpbar();
		GameObject enableEffect = EffectManager.instance.GetMonsterEnableEffect();
		enableEffect.transform.SetParent(transform);
		enableEffect.transform.localPosition = Vector3.zero;
		enableEffect.transform.localEulerAngles = Vector3.zero;
		enableEffect.transform.localScale = Vector3.one * 2f;
		enableEffect.transform.SetParent(null);
	}
	protected void OnDisable()
    {
        allMonsterCharacters.Remove(this);
		canTargetingMonsterCharacters.Remove(this);
        UIManager.instance.RemoveHpbar(this);
    }
	protected virtual void StateInitializeOnEnter()
	{
		cStateIdle.onEnter = () => {
			ChangeAnimation(cStateIdle.strStateName);
		};
		cStateMove.onEnter = () => {
			ChangeAnimation(cStateMove.strStateName);
		};
		cStateAttack.onEnter = () => {
			ChangeAnimation(cStateAttack.strStateName);
			Attack(vTargetPos);
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
			agent.isStopped = true;
			agent.velocity = Vector3.zero;
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
	public void Slow(float amount) 
	{
        cAnimator.speed = amount;
		agent.speed *= amount;
	}
	public void SlowEnd()
	{
		cAnimator.speed = 1f;
		agent.speed = defultSpeed;
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
		if (cStateMachine.CurrentState != cStateAttack) return;
		GameObject objEffect = EffectManager.instance.GetEffect(objAttackEffectPrefab);
		objEffect.GetComponent<Effect>().OnAction(transform, fPower, 1 << 8);
	}

	protected virtual IEnumerator SetTarget()
	{
		while (true)
		{
			vTargetPos = Player.instance.CurrentCharacter.transform.position;
			vTargetPos.y = 0;
			if (agent.enabled)
				agent.SetDestination(vTargetPos);
			
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
	public override void Attack(Vector3 targetPos)
	{
		transform.LookAt(targetPos);
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
	}
	protected override void ChangeState(State cNextState)
	{
		if (cStateMachine.CurrentState == cStateDie) return;
		cStateMachine.ChangeState(cNextState);
	}
	public override void Damage(float fAmount)
	{
		if (cStateMachine.CurrentState == cStateDie) return;

		fHealthPoint -= fAmount;
		SetHp();
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
        ChangeState(cStateDie);
		allMonsterCharacters.Remove(this);
		canTargetingMonsterCharacters.Remove(this);
        agent.enabled = false;
		GetComponent<Collider>().enabled = false;
        UIManager.instance.RemoveHpbar(this);
		EnemyManager.instance.MonsterDie();
		StartCoroutine(Hide());
	}
	protected IEnumerator Hide() 
	{
		yield return new WaitForSeconds(2.5f);
		gameObject.SetActive(false);
	}
	public override void Move(Vector3 targetPos)
	{
		if(agent.enabled)
			agent.isStopped = false;
	}
	protected override void ChangeAnimation(string strTrigger)
	{
		if (cStateMachine.PrevState != null)
			cAnimator.ResetTrigger(cStateMachine.PrevState.strStateName);
		cAnimator.SetTrigger(strTrigger);
	}
	protected virtual void InsertHpbar() 
	{
        UIManager.instance.InsertHpbar(this, hpbarOffset);
    }
	protected virtual void SetHp()
	{
		UIManager.instance.SetHp(this);
	}

	public bool GetIsEnterMonsterDead()
	{
		return isEnterMonsterDeath;
	}
	public void SetIsEnterMonsterDead(bool isMonsterDead)
	{
		this.isEnterMonsterDeath = isMonsterDead;
	}

	public float GetMaxHP() { return fMaxHealthPoint; }
	public float GetHP() { return fHealthPoint; }
}
