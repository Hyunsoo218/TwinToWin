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
			nextEffect = Skill_R.effect;
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
		float runTime;
		float time = 0.15f;
		float randomRange = 3f;
		MonsterCharacter target;
		RaycastHit hit;
		Vector3 targetPos;
		Vector3 startPos;
		Vector3 offsetPos;
		float offsetX;
		float offsetZ;
		for (int i = 0; i < 10; i++)
		{
			if (MonsterCharacter.canTargetingMonsterCharacters.Count == 0) break;
			runTime = 0;
			target = MonsterCharacter.canTargetingMonsterCharacters.OrderBy(obj => {
				return Vector3.Distance(transform.position, obj.transform.position);
			}).FirstOrDefault();
			offsetX = Math.Clamp(Random.Range(-randomRange, randomRange), 1.5f, 3f);
			offsetZ = Math.Clamp(Random.Range(-randomRange, randomRange), 1.5f, 3f);
			offsetPos = new Vector3(offsetX, 0, offsetZ);
			targetPos = (target.transform.position + offsetPos - transform.position).normalized * 4f + target.transform.position;
			startPos = transform.position;
			Rotate(targetPos);  
			EnableAttackEffect();
			while (runTime <= time)
			{
				runTime += Time.deltaTime;
				if (!Physics.SphereCast(transform.position + new Vector3(0, 0.6f, 0), 0.25f, transform.forward, out hit, 1f, 1 << 6))
					transform.position = Vector3.Lerp(startPos, targetPos, runTime / time);
				yield return null;
			}
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