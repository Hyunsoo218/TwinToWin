using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOverlap : Effect
{
	[SerializeField] protected Vector3 vAttackAreaCenter;
	[SerializeField] protected Vector3 vAttackAreaSize;
	[SerializeField] protected bool isSphere = false;
	[SerializeField] protected float sphereAttackAreaRange = 0f;

#if UNITY_EDITOR
	protected void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		if (isSphere == true)
		{
			Gizmos.DrawWireSphere(transform.position, sphereAttackAreaRange);
		}
		else
		{
			Gizmos.DrawWireCube(Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position, vAttackAreaSize * 2f);
		}

	}
#endif
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		transform.SetParent(tUser);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(null);

		Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;

		Collider[] arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, transform.rotation, nTargetLayer);

		foreach (Collider cItem in arrOverlapObj)
		{
			Character cTarget;
			if (cItem.TryGetComponent<Character>(out cTarget))
			{
				DamageCalculator.OnDamage(cTarget, fDamage, criticalHit);
			}
		}
	}
}
