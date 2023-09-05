using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
	[SerializeField] protected bool bPreviewOverlapArea;
	[SerializeField] protected float fRunTime = 3f;
	[SerializeField] protected Vector3 vAttackAreaCenter;
	[SerializeField] protected Vector3 vAttackAreaSize;
	public virtual void OverlapBox(Transform tUser, float fDamage, int nTargetLayer)
	{
		Invoke("InPool", fRunTime);
		transform.SetParent(tUser);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(EffectManager.instance.transform);

		Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward) * vAttackAreaCenter + transform.position;

		Collider[] arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, transform.rotation, nTargetLayer);

		foreach (Collider cItem in arrOverlapObj)
		{
			Character cTarget;
			if (cItem.TryGetComponent<Character>(out cTarget))
			{
				cTarget.Damage(fDamage);
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
	protected virtual void InPool()
	{
		gameObject.SetActive(false);
	}
}
