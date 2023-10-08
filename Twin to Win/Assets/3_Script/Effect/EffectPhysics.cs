using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPhysics : Effect
{
	protected Collider cCollider;
	protected Transform tUser;
	protected float fDamage;
	protected int nTargetLayer;
	protected void Awake()
	{
		cCollider = GetComponent<Collider>();
		cCollider.enabled = false;
	}
	protected override void InPool()
	{
		base.InPool();
		cCollider.enabled = false;
	}
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		transform.SetParent(tUser);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(EffectManager.instance.transform);

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
				//print($"{tUser.name}��(��) {cTarget.name}���� {fDamage}�� ������ ����");
				// ������ ���� - ����׿�							  		   
			}
		}
	}
}
