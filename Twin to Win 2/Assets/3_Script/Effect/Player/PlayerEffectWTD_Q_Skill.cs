using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectWTD_Q_Skill : PlayerEffect
{
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		transform.position = tUser.transform.position;
		transform.eulerAngles = tUser.transform.eulerAngles;
		if (shakeCamera)
			CameraManager.instance.ShakeCamera(shakeCameraPower, shakeCameraTime);
		Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;
		Collider[] arrOverlapObj = null;
		if (isSphere)
			arrOverlapObj = Physics.OverlapSphere(vOverlapPos, sphereAttackAreaRange, nTargetLayer);
		else
			arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, tUser.rotation, nTargetLayer);
		foreach (Collider cItem in arrOverlapObj)
		{
			if (cItem.TryGetComponent<MonsterCharacter>(out var target)) 
			{
				DamageInfo damageInfo = DamageCalculator.GetDamageInfo(target, damage, criticalHit);
				if (target.GetHP() <= damageInfo.damage) 
					Player.instance.TwinSword.ResetSkillTime(); 
				target.Damage(damageInfo.damage);
				UIManager.instance.OnDamageFont(target.transform.position, damageInfo.fontColor, damageInfo.damage);
			}
		}
	}
}
