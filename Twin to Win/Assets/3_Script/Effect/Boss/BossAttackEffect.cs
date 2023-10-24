using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffect : Effect
{
	[SerializeField] private Transform tProjectile;
	[SerializeField] private float fSpeed = 1f;
	[SerializeField] private List<Transform> arrExplosionPos;
	[SerializeField] private List<DamagableSpaceControl> arrDecalProjector;
	[SerializeField] private Animator bombAnimator;
	[SerializeField] private GameObject bomb;
	private Animator cAnimator;
	private int nExplosionCount = 0;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;

	public override void Initialize()
	{
		base.Initialize();
		cAnimator = GetComponent<Animator>();
	}
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		this.tUser = tUser;
		this.fDamage = fDamage;
		this.nTargetLayer = nTargetLayer;

		transform.SetParent(tUser.GetChild(1));
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(null);
		transform.localScale = Vector3.one;

		bomb.SetActive(true);
		bombAnimator.SetTrigger("Move");

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
		bombAnimator.SetTrigger("Explosion");
	}
	public void Explosion()
	{
		bomb.SetActive(false);
		nExplosionCount++;
		soundComponent.PlayOneShot(clip);

		switch (nExplosionCount)
		{
			case 1:
				Overlap(Physics.OverlapSphere(arrExplosionPos[0].position, 1f, nTargetLayer));
				break;
			case 2:
				Overlap(Physics.OverlapSphere(arrExplosionPos[1].position, 1.25f, nTargetLayer));
				Overlap(Physics.OverlapSphere(arrExplosionPos[2].position, 1.25f, nTargetLayer));
				Overlap(Physics.OverlapSphere(arrExplosionPos[3].position, 1.25f, nTargetLayer));
				Overlap(Physics.OverlapSphere(arrExplosionPos[4].position, 1.25f, nTargetLayer));
				break;
			case 3:
				Overlap(Physics.OverlapSphere(arrExplosionPos[5].position, 1.5f, nTargetLayer));
				Overlap(Physics.OverlapSphere(arrExplosionPos[6].position, 1.5f, nTargetLayer));
				Overlap(Physics.OverlapSphere(arrExplosionPos[7].position, 1.5f, nTargetLayer));
				Overlap(Physics.OverlapSphere(arrExplosionPos[8].position, 1.5f, nTargetLayer));
				break;
		}
	}
	public void OnDamageArea()
	{
		arrDecalProjector[0].OnAction(0.33f, FillType.X_Y);
	}
	public void OnDamageAreaEnhance()
	{
		arrDecalProjector[0].OnAction(0.33f, FillType.X_Y);

		arrDecalProjector[1].OnAction(0.33f * 2f, FillType.X_Y);
		arrDecalProjector[2].OnAction(0.33f * 2f, FillType.X_Y);
		arrDecalProjector[3].OnAction(0.33f * 2f, FillType.X_Y);
		arrDecalProjector[4].OnAction(0.33f * 2f, FillType.X_Y);
														   
		arrDecalProjector[5].OnAction(0.33f * 3f, FillType.X_Y);
		arrDecalProjector[6].OnAction(0.33f * 3f, FillType.X_Y);
		arrDecalProjector[7].OnAction(0.33f * 3f, FillType.X_Y);
		arrDecalProjector[8].OnAction(0.33f * 3f, FillType.X_Y);
	}
	private void Overlap(Collider[] colliders) 
	{
		foreach (Collider cItem in colliders)
		{
			Character cTarget;
			if (cItem.TryGetComponent<Character>(out cTarget))
			{
				DamageCalculator.OnDamage(cTarget, fDamage, criticalHit);
			}
		}
	}
	protected override void InPoolEvent()
	{
		base.InPoolEvent();
		nExplosionCount = 0;
	}
}
