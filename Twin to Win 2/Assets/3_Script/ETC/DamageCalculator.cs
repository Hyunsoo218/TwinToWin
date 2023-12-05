using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct DamageInfo 
{
    public DamageInfo(float damage, DamageType fontColor, Vector3 pos) 
    {
        this.damage = damage;
        this.fontColor = fontColor;
        this.pos = pos;
    }
    public float damage;
    public DamageType fontColor;
    public Vector3 pos;
}
public class DamageCalculator
{
    public static void OnDamage(Character target, float defaultDamage, bool criticalHit = false)
    {
        DamageType fontColor = DamageType.normal;

        float damage = defaultDamage * Random.Range(0.8f, 1.2f);
		if (criticalHit) 
        { 
            if (Random.Range(0, 100f) < 71f)
            {
                CharacterType type = Player.Instance.CurrentCharacter.Type;
                switch (type)
                {
                    case CharacterType.wtd:     fontColor = DamageType.red; break;
                    case CharacterType.wgs:     fontColor = DamageType.blue; break;
                    default:                    fontColor = DamageType.normal; break;
                }
                damage = damage * Random.Range(1.3f, 1.8f);
            }
        }
        target.Damage(damage);
        UIManager.instance.OnDamageFont(target.transform.position, fontColor, damage);
    }
    public static DamageInfo GetDamageInfo(Character target, float defaultDamage, bool criticalHit = false) 
    {
        DamageType fontColor = DamageType.normal;

        float damage = defaultDamage * Random.Range(0.8f, 1.2f);
        if (criticalHit)
        {
            if (Random.Range(0, 100f) < 71f)
            {
                CharacterType type = Player.Instance.CurrentCharacter.Type;
                switch (type)
                {
                    case CharacterType.wtd: fontColor = DamageType.red; break;
                    case CharacterType.wgs: fontColor = DamageType.blue; break;
                    default: fontColor = DamageType.normal; break;
                }
                damage = damage * Random.Range(1.3f, 1.8f);
            }
        }
        return new DamageInfo(damage, fontColor, target.transform.position);
    }
}