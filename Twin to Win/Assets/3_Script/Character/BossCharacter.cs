using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : MonsterCharacter
{
	[SerializeField] protected GameObject objAttack2EffectPrefab;
	[SerializeField] protected GameObject objAttack3EffectPrefab;
	[SerializeField] protected GameObject objAttack4EffectPrefab;
	[SerializeField] protected GameObject objAttack5EffectPrefab;
	[SerializeField] protected GameObject objAttack6EffectPrefab;
	[SerializeField] protected GameObject objAttack6FinishEffectPrefab;
	[SerializeField] protected GameObject objAttackBreakTilePrefab;
	[SerializeField] protected Transform tRoot;

	protected State cStateAttack2 = new State("Attack2");  // 근접 기본
	protected State cStateAttack3 = new State("Attack3");  // 근접 회전
	protected State cStateAttack4 = new State("Attack4");  // 근접 360도 폭탄 
	protected State cStateAttack5 = new State("Attack5");  // 원거리 가시
	protected State cStateAttack6 = new State("Attack6");  // 원거리 돌진
	protected State cStateAttack6_Finish = new State("Attack6 Finish");  // 원거리 돌진 마지막
	protected State cStateBreakTile = new State("BreakTile");  
	protected int nAttackCount = 0;
	protected List<State> arrAttackRanged = new List<State>();
	protected List<State> arrAttackMelee = new List<State>();
	protected Coroutine coLookTarget;

	protected Dictionary<State, GameObject> dAttackEffects = new Dictionary<State, GameObject>();

	protected override void Awake()
	{
		base.Awake();
		arrAttackMelee.Add(cStateAttack3);
		arrAttackMelee.Add(cStateAttack4);

		arrAttackRanged.Add(cStateAttack5);
		arrAttackRanged.Add(cStateAttack6);

		//arrAttackRanged.Add(cStateBreakTile);

		dAttackEffects.Add(cStateAttack, objAttackEffectPrefab);
		dAttackEffects.Add(cStateAttack2, objAttack2EffectPrefab);
		dAttackEffects.Add(cStateAttack3, objAttack3EffectPrefab);
		dAttackEffects.Add(cStateAttack4, objAttack4EffectPrefab);
		dAttackEffects.Add(cStateAttack5, objAttack5EffectPrefab);
		dAttackEffects.Add(cStateAttack6, objAttack6EffectPrefab);
		dAttackEffects.Add(cStateAttack6_Finish, objAttack6FinishEffectPrefab);
		dAttackEffects.Add(cStateBreakTile, objAttackBreakTilePrefab);
	}
	protected override void StateInitializeOnEnter()
	{
		base.StateInitializeOnEnter();
		cStateMove.onEnter += () => {
			coLookTarget = StartCoroutine(LookTargetMoveState());
		};
		cStateAttack.onEnter = () => {
			ChangeAnimation(cStateAttack.strStateName);
			Attack();
		};
		cStateAttack2.onEnter = () => {
			ChangeAnimation(cStateAttack2.strStateName);
			Attack();
		};
		cStateAttack3.onEnter = () => {
			ChangeAnimation(cStateAttack3.strStateName);
			Attack();
		};
		cStateAttack4.onEnter = () => {
			ChangeAnimation(cStateAttack4.strStateName);
			Attack();
		};
		cStateAttack5.onEnter = () => {
			ChangeAnimation(cStateAttack5.strStateName);
			Attack();
		};
		cStateAttack6.onEnter = () => {
			ChangeAnimation(cStateAttack6.strStateName);
			Attack();
		};
		cStateAttack6_Finish.onEnter = () => {
			ChangeAnimation(cStateAttack6_Finish.strStateName);
			transform.position = vTargetPos;
			cAgent.enabled = true;
			cAgent.isStopped = true;
			cAgent.radius = 0;
		};//
        cStateBreakTile.onEnter = () => {
            ChangeAnimation(cStateBreakTile.strStateName);
            transform.position = (Vector3.forward + Vector3.right) * 14f;
            cAgent.enabled = false;
        };//cStateBreakTile
    }
	protected override void StateInitializeOnStay()
	{
		cStateIdle.onStay = () => {
			if (fTargetDist >= fAttackDistance) // 근접범위 밖이면
			{
				if (fTargetDist < fAttackDistance * 2f && bCanAttack) // 원거리 범위 안이면, 공격 가능하면
				{
					ChangeAttackStateRanged();
				}
				else if (fTargetDist > fAttackDistance * 2f) // 원거리 범위 밖이면
				{
					ChangeState(cStateMove);
				}
			}
			else // 금접 범위 안이면
			{
				if (bCanAttack) // 공격 가능하면
				{
					ChangeAttackStateMelee();
				}
			}
		};
		cStateMove.onStay = () => {
			if (fTargetDist >= fAttackDistance) // 근접범위 밖이면
			{
				if (fTargetDist < fAttackDistance * 2f && bCanAttack) // 원거리 범위 안이면, 공격 가능하면
				{
					ChangeAttackStateRanged();
				}
				else if (fTargetDist < fAttackDistance * 2f && !bCanAttack) // 원거리 범위 안이면, 공격 못하면
				{
					ChangeState(cStateIdle);
				}

			}
			else // 금접 범위 안이면
			{
				if (bCanAttack) // 공격 가능하면
				{
					ChangeAttackStateMelee();
				}
				else
				{
					ChangeState(cStateIdle);
				}
			}
		};
	}
	protected override void StateInitializeOnExit()
	{
		base.StateInitializeOnExit();
		cStateMove.onExit += () => {
			StopCoroutine(coLookTarget);
		};
		cStateAttack2.onExit = () => {
			if (coAttackDelay != null)
			{
				StopCoroutine(coAttackDelay);
			}
			coAttackDelay = StartCoroutine(AttackDelay());
		};
		cStateAttack3.onExit = () => {
			if (coAttackDelay != null)
			{
				StopCoroutine(coAttackDelay);
			}
			coAttackDelay = StartCoroutine(AttackDelay());
		};
		cStateAttack4.onExit = () => {
			if (coAttackDelay != null)
			{
				StopCoroutine(coAttackDelay);
			}
			coAttackDelay = StartCoroutine(AttackDelay());
		};
		cStateAttack5.onExit = () => {
			if (coAttackDelay != null)
			{
				StopCoroutine(coAttackDelay);
			}
			coAttackDelay = StartCoroutine(AttackDelay());
		};
		cStateAttack6.onExit = () => {
			if (coAttackDelay != null)
			{
				StopCoroutine(coAttackDelay);
			}
			coAttackDelay = StartCoroutine(AttackDelay());
		};
		cStateAttack6_Finish.onExit = () => {
			cAgent.radius = 0.5f;
		};
        cStateBreakTile.onExit = () => {
            cAgent.enabled = true;
            cAgent.isStopped = true;
            cAgent.radius = 0.5f;
        };//cStateBreakTile
    }
	protected void ChangeAttackStateRanged()
	{
		if (nAttackCount < 2)
		{
			nAttackCount++;
			ChangeState(cStateAttack);
		}
		else
		{
			nAttackCount = 0;
			int nPatternCount = Random.Range(0, arrAttackRanged.Count);
			ChangeState(arrAttackRanged[nPatternCount]);
		}
	}
	protected void ChangeAttackStateMelee()
	{
		if (nAttackCount < 2)
		{
			nAttackCount++;
			ChangeState(cStateAttack2);
		}
		else
		{
			nAttackCount = 0;
			int nPatternCount = Random.Range(0, arrAttackMelee.Count);
			ChangeState(arrAttackMelee[nPatternCount]);
		}
	}
	public override void Attack()
	{
		tRoot.LookAt(vTargetPos);
		tRoot.localEulerAngles = new Vector3(0, tRoot.localEulerAngles.y, 0);
	}
	public override void EnableEffect()
	{
		State cCurrentState = cStateMachine.GetCurrentState();

		if (cCurrentState == cStateAttack3)
		{
			StartCoroutine(Attack3Move());
		}

		GameObject objEffect = EffectManager.instance.GetEffect(dAttackEffects[cCurrentState]);
		objEffect.GetComponent<Effect>().OnAction(transform, fPower, 1 << 8);
	}
	protected IEnumerator LookTargetMoveState()
	{
		while (true)
		{
			tRoot.LookAt(Player.instance.cCurrentCharacter.transform);
			tRoot.localEulerAngles = new Vector3(0, tRoot.localEulerAngles.y, 0);
			yield return null;
		}
	}
	private IEnumerator Attack3Move() 
	{
		float fDefaultSpeed = cAgent.speed;
		cAgent.isStopped = false;
		cAgent.speed = 2f;
		yield return new WaitForSeconds(3.8f);
		cAgent.isStopped = true;
		cAgent.speed = fDefaultSpeed;
	}
	public void Rush() 
	{
		StartCoroutine(Attack6Rush());
	}
	private IEnumerator Attack6Rush() 
	{
		cAgent.enabled = false;
		yield return new WaitForSeconds(1f);

		transform.position += Vector3.up * 10f;

		if (GameManager.instance.phase == Phase.Phase_3)
			yield return new WaitForSeconds(3.7f);
		else
			yield return new WaitForSeconds(7.5f);

		ChangeState(cStateAttack6_Finish);
	}
    protected override void InsertHpbar()
    {
        // 보스 hp ui 띄우기
    }
}