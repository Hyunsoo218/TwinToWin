using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class WTDPlayableCharacter : PlayerbleCharacter
{
	protected override void StateInitalizeOnEnter()
	{
		base.StateInitalizeOnEnter();
		wSkillState.onEnter += () => {
			Action exitEvent = () => ReturnToIdle();
			StartCoroutine(LinearMovement(0.15f, 8f, 0, exitEvent));
			nextEffect = Skill_W.effect;
			EnableAttackEffect();
			canDodge = false;
		};
		eSkillState.onEnter += () => {
			Action exitEvent = () => ReturnToIdle();
			StartCoroutine(LinearMovement(0.5f, 8f, 0, exitEvent));
			StartCoroutine(LinearJumpMovement(0.5f, 3f));
			canDodge = false;
			canDamage = false;
		};
	}
	public override void ResetSkillTime()
	{
		Skill_Q.time.current = Skill_Q.time.max;
		Skill_W.time.current = Skill_W.time.max;
		Skill_E.time.current = Skill_E.time.max;
	}
}