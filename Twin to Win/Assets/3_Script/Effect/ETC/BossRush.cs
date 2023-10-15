using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRush : Effect
{
	[SerializeField] private DamagableSpaceControl cDSC;
	[SerializeField] private bool bPreviewOverlapArea;
	[SerializeField] private Vector3 vAttackAreaCenter;
	[SerializeField] private Vector3 vAttackAreaSize;
	private Animator cAnimator;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;
	protected override void Awake()
	{
		base.Awake();
		cAnimator = GetComponent<Animator>();
	}
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		this.tUser = tUser;
		this.fDamage = fDamage;
		this.nTargetLayer = nTargetLayer;

		Vector3 v3Direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
		transform.position = Quaternion.LookRotation(v3Direction, Vector3.up) * Vector3.forward * 30f + new Vector3(14f, 0, 14f);

		string strPlayerState = Player.instance.GetCurrentCharacterStateName();
		Transform tPlayer = Player.instance.cCurrentCharacter.transform;
		Vector3 v3Target, v3TargetPos;
		float fZ, fX;
		if (strPlayerState.Equals("moveState"))
		{
			v3Target = tPlayer.forward * 3f + tPlayer.position;
			fZ = Random.Range(v3Target.z, v3Target.z);
			fX = Random.Range(v3Target.x, v3Target.x);
			v3TargetPos = new Vector3(fX, 0, fZ);
		}
		else
		{
			v3Target = tPlayer.position;
			fZ = Random.Range(v3Target.z, v3Target.z);
			fX = Random.Range(v3Target.x, v3Target.x);
			v3TargetPos = new Vector3(fX, 0, fZ);
		}

		transform.LookAt(v3TargetPos);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

		cAnimator.SetTrigger("Action");

		cDSC.OnAction(1f, FillType.X);

		StartCoroutine(LateOverlap());
	}
	private IEnumerator LateOverlap() 
	{
		yield return new WaitForSeconds(1f);
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
}
