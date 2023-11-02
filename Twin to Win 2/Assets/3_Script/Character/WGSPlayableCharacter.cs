using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class WGSPlayableCharacter : PlayerbleCharacter
{
	private (int max, int current) eSkillEffectCount = (20, 0);

	protected override void StateInitalizeOnEnter()
	{
		base.StateInitalizeOnEnter();
		eSkillState.onEnter += () => {
			agent.isStopped = false;
		};
		wSkillState.onEnter += () => {
			StartCoroutine(LinearMovement(1f, 12f, 0.5f));
			canDodge = false;
		};
	}
	protected override void StateInitalizeOnStay()
	{
		base.StateInitalizeOnStay();
		eSkillState.onStay += () => {
			transform.eulerAngles = Vector3.zero;
		};
	}
	protected override void StateInitalizeOnExit()
	{
		base.StateInitalizeOnExit();
		eSkillState.onExit += () => {
			eSkillEffectCount.current = 0;
			if (gameObject.activeSelf)
				agent.isStopped = true;
		};
	}
	public override void Move(Vector3 targetPos)
	{
		if (cStateMachine.GetCurrentState() == eSkillState)
			agent.SetDestination(targetPos);
		else {
			if (!canMove || isDie) return;
			agent.SetDestination(targetPos);
			if (cStateMachine.GetCurrentState() != moveState)
				ChangeState(moveState);
		}
	}
	public void ESkillEffect()
	{
		EnableAttackEffect();

		eSkillEffectCount.current++;

		if (eSkillEffectCount.current >= eSkillEffectCount.max)
		{
			ReturnToIdle();
		}
	}
}