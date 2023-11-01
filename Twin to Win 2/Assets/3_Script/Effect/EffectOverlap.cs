using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOverlap : Effect
{
	[SerializeField] protected bool bPreviewOverlapArea;
	[SerializeField] protected Vector3 vAttackAreaCenter;
	[SerializeField] protected Vector3 vAttackAreaSize;
	[SerializeField] bool isSphere = false;
	[SerializeField] float sphereAttackAreaRange = 0f;

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		if (isSphere == true)
		{
			Gizmos.DrawWireSphere(transform.position, sphereAttackAreaRange * 2f);
		}
		else
		{
			Gizmos.DrawWireCube(Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position, vAttackAreaSize * 2f);
		}

	}
#endif
	protected override void OnEnable()
	{
		base.OnEnable();

		Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;

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
