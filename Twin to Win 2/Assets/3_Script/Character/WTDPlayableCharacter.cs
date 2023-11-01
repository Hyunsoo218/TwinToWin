using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class WTDPlayableCharacter : PlayerbleCharacter
{
	protected override void StateInitalize()
	{
		base.StateInitalize();
		for (int i = 0; i < 3; i++)
			normalAttack.Add(new State("Attack_" + i));
	}
	protected override void StateInitalizeOnEnter()
	{
		base.StateInitalizeOnEnter();
		wSkillState.onEnter += () => {
			Action exitEvent = () => ReturnToIdle();
			StartCoroutine(LinearMovement(0.15f, 8f, 0, exitEvent));
			canDodge = false;
		};
		eSkillState.onEnter += () => {
			Action exitEvent = () => ReturnToIdle();
			StartCoroutine(LinearMovement(0.5f, 8f, 0, exitEvent));
			canDodge = false;
		};
		rSkillState.onEnter = () => {

		};
	}
}