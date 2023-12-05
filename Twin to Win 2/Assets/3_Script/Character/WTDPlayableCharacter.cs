using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Linq;
using Random = UnityEngine.Random;

public class WTDPlayableCharacter : PlayerbleCharacter
{
	protected override void StateInitalizeOnEnter()
	{
		base.StateInitalizeOnEnter();
		wSkillState.onEnter += () => {
			Action exitEvent = () => ReturnToIdle();
			StartCoroutine(LinearMovement(0.15f, 8f, 0, exitEvent));
			nextEffect = wSkill.effect;
			EnableAttackEffectSkill();
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
		qSkill.time.current = qSkill.time.max;
		wSkill.time.current = wSkill.time.max;
		eSkill.time.current = eSkill.time.max;
	}
}