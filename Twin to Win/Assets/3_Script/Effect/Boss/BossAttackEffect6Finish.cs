using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffect6Finish : Effect
{
	[SerializeField] private DamagableSpaceControl cDSC;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;

	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		this.tUser = tUser;
		this.fDamage = fDamage;
		this.nTargetLayer = nTargetLayer;

		transform.SetParent(tUser.GetChild(1));
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(EffectManager.instance.transform);
		transform.localScale = Vector3.one;

		cDSC.OnAction(1.5f, FillType.X_Y);
		StartCoroutine(OnAction());
	}
	private IEnumerator OnAction() 
	{
		yield return new WaitForSeconds(1.5f);
		Collider[] arrOverlapObj = Physics.OverlapSphere(tUser.position, 5f, nTargetLayer);
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
	}
}
