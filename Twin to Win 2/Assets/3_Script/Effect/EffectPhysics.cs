using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPhysics : Effect
{
	protected Collider cCollider;
	protected Transform tUser;
	[SerializeField] protected float fDamage;
	protected int nTargetLayer;

	public override void Initialize()
	{
		base.Initialize();
		cCollider = GetComponent<Collider>();
		cCollider.enabled = false;
	}
	protected override void InPoolEvent()
	{
		cCollider.enabled = false;
	}
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		transform.SetParent(tUser);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(null);

		cCollider.enabled = true;
		this.tUser = tUser;
		this.fDamage = fDamage;
		this.nTargetLayer = nTargetLayer;
	}
	protected virtual void OnTriggerEnter(Collider other)
	{
		if ((1 << other.gameObject.layer) == nTargetLayer)
		{
			Character cTarget;
			if (other.TryGetComponent<Character>(out cTarget))
			{
				cTarget.Damage(fDamage);
				//print($"{tUser.name}이(가) {cTarget.name}에게 {fDamage}의 데미지 입힘");
				// 지우지 마영 - 디버그용							  		   
			}
		}
	}
}
