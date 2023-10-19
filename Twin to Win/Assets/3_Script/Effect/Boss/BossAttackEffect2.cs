using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffect2 : Effect
{
	[SerializeField] private bool bPreviewOverlapArea;
	[SerializeField] private Vector3 vAttackAreaCenter;
	[SerializeField] private Vector3 vAttackAreaSize;
	[SerializeField] private List<Transform> arrExplosionPos;
	[SerializeField] private List<DamagableSpaceControl> arrDecalProjector;
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

		if (GameManager.instance.phase == Phase.Phase_3)
		{
			cAnimator.SetTrigger("Action");
		}

		Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;

		Collider[] arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, transform.rotation, nTargetLayer);

		foreach (Collider cItem in arrOverlapObj)
		{
			Character cTarget;
			if (cItem.TryGetComponent<Character>(out cTarget))
			{
				cTarget.Damage(fDamage);
				print($"{tUser.name}이(가) {cTarget.name}에게 {fDamage}의 데미지 입힘");
				// 지우지 마영 - 디버그용							  		   
			}
		}

		if (bPreviewOverlapArea)
		{
			GameObject objPreviewBox = Resources.Load<GameObject>("AttackAreaPreviewCube");
			GameObject obj = Instantiate(objPreviewBox);
			obj.transform.position = vOverlapPos;
			obj.transform.rotation = transform.rotation;
			obj.transform.localScale = vAttackAreaSize * 2f;
			Destroy(obj, 1f);
		}
	}

	public void Explosion()
	{
		soundComponent.PlayOneShot(clip);
		nExplosionCount++;
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
				cTarget.Damage(fDamage);
				print($"{tUser.name}이(가) {cTarget.name}에게 {fDamage}의 데미지 입힘");
				// 지우지 마영 - 디버그용							  		   
			}
		}
	}
	protected override void InPoolEvent()
	{
		base.InPoolEvent();
		nExplosionCount = 0;
	}
}
