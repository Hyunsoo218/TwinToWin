using System.Collections;
using UnityEngine;
public class PlayerEffect : EffectOverlap
{
	[SerializeField] float damage;
    public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
    {
		transform.position = tUser.transform.position;
		transform.eulerAngles = tUser.transform.eulerAngles;

		Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;

		Collider[] arrOverlapObj = null;

        if (isSphere)
			arrOverlapObj = Physics.OverlapSphere(vOverlapPos, sphereAttackAreaRange, nTargetLayer);
		else
			arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, tUser.rotation, nTargetLayer);

		foreach (Collider cItem in arrOverlapObj)
		{
			Character target;
			if (cItem.TryGetComponent<Character>(out target))
				DamageCalculator.OnDamage(target, damage, criticalHit);
		}
	}
}
