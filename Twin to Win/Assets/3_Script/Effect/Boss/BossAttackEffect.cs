using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffect : Effect
{
	[SerializeField] private Transform tProjectile;
	[SerializeField] private float fSpeed = 1f;
	private Animator cAnimator;

	private void Awake()
	{
		cAnimator = GetComponent<Animator>();
	}
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		transform.SetParent(tUser);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(EffectManager.instance.transform);

		string strAnimationTrigger = "Action";
		if (GameManager.instance.phase == Phase.Phase_3)
		{
			strAnimationTrigger += "Enhance";
		}
		cAnimator.SetTrigger(strAnimationTrigger);
		StartCoroutine(MoveToTarget());
	}
	private IEnumerator MoveToTarget()
	{
		Vector3 v3CurrtPos = tProjectile.position;
		Vector3 v3TargetPos = Player.instance.cCurrentCharacter.transform.position;

		float runTime = 0;
		float duration = 0.5f;

		while (runTime < duration)
		{
			runTime += Time.deltaTime * fSpeed;

			transform.position = Vector3.Lerp(v3CurrtPos, v3TargetPos, runTime / duration);

			yield return null;
		}
	}
	public void Explosion(Transform tCrater)
	{

	}
}
