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
	//[SerializeField] private GameObject Skill_R_Stay_effect;
	//[SerializeField] private GameObject Skill_R_Stay_Trail_effect;
	//[SerializeField] private GameObject Skill_R_End_effect;
	//[SerializeField] private GameObject motionTrail;
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
	//private IEnumerator RSkillAction()
	//{
	//	nextEffect = Skill_R.effect;
	//	EnableAttackEffect();
	//	yield return new WaitForSeconds(3f);
	//	nextEffect = Skill_R_Stay_effect;
	//	float runTime;
	//	float time = 0.1f;
	//	float randomRange = 3f;
	//	MonsterCharacter target;
	//	RaycastHit hit;
	//	Vector3 targetPos;
	//	Vector3 startPos;
	//	Vector3 offsetPos;
	//	float offsetX;
	//	float offsetZ;
	//	List<Vector3> endAttackPos = new List<Vector3>();
	//	for (int i = 0; i < 10; i++)
	//	{
	//		if (MonsterCharacter.canTargetingMonsterCharacters.Count == 0) break;
	//		runTime = 0;
	//		target = MonsterCharacter.canTargetingMonsterCharacters.OrderBy(obj => {
	//			return Vector3.Distance(transform.position, obj.transform.position);
	//		}).FirstOrDefault();
	//		offsetX = Math.Clamp(Random.Range(-randomRange, randomRange), 2f, 3f);
	//		offsetZ = Math.Clamp(Random.Range(-randomRange, randomRange), 2f, 3f);
	//		offsetPos = new Vector3(offsetX, 0, offsetZ);
	//		targetPos = (target.transform.position + offsetPos - transform.position).normalized * 4f + target.transform.position;
	//		startPos = transform.position;
	//		Rotate(targetPos);  
	//		EnableAttackEffect();
	//		while (runTime <= time)
	//		{
	//			runTime += Time.deltaTime;
	//			if (!Physics.SphereCast(transform.position + new Vector3(0, 0.6f, 0), 0.25f, transform.forward, out hit, 1f, 1 << 6))
 //               {
	//				transform.position = Vector3.Lerp(startPos, targetPos, runTime / time);
	//				GameObject effect = EffectManager.instance.GetEffect(Skill_R_Stay_Trail_effect);
	//				effect.transform.position = transform.position;
	//				endAttackPos.Add(transform.position);
	//			}
	//			yield return null;
	//		}
	//		GameObject motionEffect = EffectManager.instance.GetEffect(motionTrail);
	//		motionEffect.GetComponent<Effect>().OnAction(transform, 100f, 1 << 6);
	//	}
	//	yield return new WaitForSeconds(1f);
	//	CameraManager.instance.ShakeCamera(5f, 0.5f);
 //       foreach (var pos in endAttackPos)
 //       {
	//		GameObject effect = EffectManager.instance.GetEffect(Skill_R_End_effect);
	//		effect.transform.position = pos;
	//	}
	//	ReturnToIdle();
	//}
	public override void ResetSkillTime()
	{
		Skill_Q.time.current = Skill_Q.time.max;
		Skill_W.time.current = Skill_W.time.max;
		Skill_E.time.current = Skill_E.time.max;
	}
}