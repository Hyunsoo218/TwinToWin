using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    public static void OnDamage(Character target, float defaultDamage, bool criticalHit = false)
    {
        DamageType fontColor = DamageType.normal;

        float damage = defaultDamage * Random.Range(0.8f, 1.2f);
		if (criticalHit) 
        { 
            if (Random.Range(0, 100f) < 40f)
            {
                if( Player.instance.cCurrentCharacter == Player.instance.GetTwinSword()) 
                    fontColor = DamageType.red; 
			    else
                    fontColor = DamageType.blue; 
            
                damage = damage * Random.Range(1.3f, 1.8f);
            }
        }
        target.Damage(damage);
        UIManager.instance.OnDamageFont(target.transform.position, fontColor, damage);
        Debug.Log($"{target.gameObject.name}에게 {damage}데미지");
    }
}