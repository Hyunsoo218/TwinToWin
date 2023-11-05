using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectWTD_W_Skill : PlayerEffect
{
	private Transform user;
	[SerializeField] private Collider damageArea;
	public override void Initialize()
	{
		base.Initialize();
		damageArea.enabled = false;
	}
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		user = tUser;
		StartCoroutine(ChackDamageArea());
	}
	private IEnumerator ChackDamageArea() 
	{
		transform.SetParent(user);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;
		damageArea.enabled = true;
		yield return new WaitForSeconds(0.15f);
		transform.SetParent(null);
		damageArea.enabled = false;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 7)
		{
			MonsterCharacter target;
			if (other.TryGetComponent<MonsterCharacter>(out target))
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
