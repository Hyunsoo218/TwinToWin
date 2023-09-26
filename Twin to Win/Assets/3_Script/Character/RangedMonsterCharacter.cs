using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMonsterCharacter : MonsterCharacter
{
	[SerializeField] protected GameObject objCastingEffect;
	[SerializeField] protected DamagableSpaceControl cDSC;
	protected State cStateCasting = new State("Idle");
	protected Coroutine coCasting;

	protected override void StateInitializeOnEnter()
	{
		base.StateInitializeOnEnter();

		cStateCasting.onEnter = () => {
			ChangeAnimation(cStateCasting.strStateName);
			coCasting = StartCoroutine(AttackCasting());
			cDSC.OnAction(3f, FillType.X);
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
	protected override void StateInitializeOnExit()
	{
		base.StateInitializeOnExit();

		cStateCasting.onExit = () => {
			StopCoroutine(coCasting);
            cDSC.Cancel();

        };
	}
	private IEnumerator AttackCasting()
	{
		yield return new WaitForSeconds(3f);
		ChangeState(cStateAttack);
	}
}
