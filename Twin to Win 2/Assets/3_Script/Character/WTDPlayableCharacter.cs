using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class WTDPlayableCharacter : PlayerbleCharacter
{
	[SerializeField] private GameObject root;
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
		rSkillState.onEnter = () => {
			canMove = false;
			canAttack = false;
			canDodge = false;
			canTag = false;
			canDamage = false;
			canSkill = false;
			GameManager.instance.AsynchronousExecution(InitializeSkillTime(Skill_R));
			StartCoroutine(RSkillAction());
			root.SetActive(false);
		};
	}
	protected override void StateInitalizeOnExit()
	{
		base.StateInitalizeOnExit();
		rSkillState.onExit += () => {
			root.SetActive(true);
		};
	}
	private IEnumerator RSkillAction()
	{
		float time = 0.15f;
		nextEffect = Skill_W.effect;
		for (int i = 0; i < 10; i++)
		{
			if (MonsterCharacter.allMonsterCharacters.Count == 0) break;

			float runTime = 0;
			Vector3 targetPos = (MonsterCharacter.allMonsterCharacters[0].transform.position - transform.position).normalized * 4f + MonsterCharacter.allMonsterCharacters[0].transform.position;
			Vector3 startPos = transform.position;
			RaycastHit hit;
			Rotate(targetPos);
			EnableAttackEffect();
			while (runTime <= time)
			{
				runTime += Time.deltaTime;
				if (!Physics.SphereCast(transform.position + new Vector3(0, 0.6f, 0), 0.25f, transform.forward, out hit, 1f, 1 << 6))
					transform.position = Vector3.Lerp(startPos, targetPos, runTime / time);
				yield return null;
			}

			//yield return new WaitForSeconds(0.15f);
		}
		ReturnToIdle();
	}
	public override void ResetSkillTime()
	{
		Skill_Q.time.current = Skill_Q.time.max;
		Skill_W.time.current = Skill_W.time.max;
		Skill_E.time.current = Skill_E.time.max;
	}
}