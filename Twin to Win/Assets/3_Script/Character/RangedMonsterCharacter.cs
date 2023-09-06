using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMonsterCharacter : MonsterCharacter
{
	[SerializeField] protected GameObject objCastingEffect;
	protected State cStateCasting = new State("Idle");

	protected override void StateInitializeOnEnter()
	{
		base.StateInitializeOnEnter();

		cStateCasting.onEnter = () => {
			ChangeAnimation(cStateCasting.strStateName);
			StartCoroutine(AttackCasting());
		};
	}
	protected override void StateInitializeOnStay()
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
					ChangeState(cStateCasting);
				}
			}
		};
		cStateMove.onStay = () => {
			if (fTargetDist < fAttackDistance)
			{
				if (bCanAttack)
				{
					ChangeState(cStateCasting);
				}
				else
				{
					ChangeState(cStateIdle);
				}
			}
		};
		cStateCasting.onStay = () => {
			transform.LookAt(Player.instance.cCurrentCharacter.transform);
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		};
	}
	private IEnumerator AttackCasting()
	{
		objCastingEffect.SetActive(true);
		yield return new WaitForSeconds(3f);
		objCastingEffect.SetActive(false);
		ChangeState(cStateAttack);
	}
}
