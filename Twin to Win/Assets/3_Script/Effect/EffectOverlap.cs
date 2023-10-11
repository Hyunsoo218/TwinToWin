using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOverlap : Effect
{
	[SerializeField] protected bool bPreviewOverlapArea;
	[SerializeField] protected Vector3 vAttackAreaCenter;
	[SerializeField] protected Vector3 vAttackAreaSize;
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		transform.SetParent(tUser);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(EffectManager.instance.transform);

		Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;

		Collider[] arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, transform.rotation, nTargetLayer);

		foreach (Collider cItem in arrOverlapObj)
		{
			Character cTarget;
			if (cItem.TryGetComponent<Character>(out cTarget))
			{
                DamageType type = DamageType.red;

                if (Random.Range(0,100) < 30) 
				{
					type = DamageType.red;
                    fDamage = fDamage * Random.Range(1f, 1.5f);
                }

				cTarget.Damage(fDamage);
				UIManager.instance.OnDamageFont(cTarget.transform.position, type, fDamage);
				//print($"{tUser.name}��(��) {cTarget.name}���� {fDamage}�� ������ ����");
				// ������ ���� - ����׿�							  		   
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
