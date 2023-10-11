using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFontControl : MonoBehaviour
{
    [SerializeField] private GameObject damageFont;
    [SerializeField] private GameObject damageFontRed;
    [SerializeField] private GameObject damageFontBlue;
    private List<DamageFont> damages = new List<DamageFont>();
    private List<DamageFont> damageReds = new List<DamageFont>();
    private List<DamageFont> damageBlues = new List<DamageFont>();

    private void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            damages.Add(Instantiate(damageFont, transform).GetComponent<DamageFont>());
            damageReds.Add(Instantiate(damageFontRed, transform).GetComponent<DamageFont>());
            damageBlues.Add(Instantiate(damageFontBlue, transform).GetComponent<DamageFont>());
        }
    }
    public void EnableDamageFont(Vector3 targetPos, DamageType type, float damage)
    {
        List<DamageFont> poolingObj = GetPooler(type);
        for (int i = 0; poolingObj.Count > 0;) 
        {
            if (!poolingObj[i].gameObject.activeSelf)
            {
                poolingObj[i].EnableDamage(damage);
                poolingObj[i].gameObject.transform.position = Camera.main.WorldToScreenPoint(targetPos);
                poolingObj[i].gameObject.SetActive(true);
                return;
            }
        }
        AddPoolingObj(poolingObj, type);
        EnableDamageFont(targetPos, type, damage);
    }
    private List<DamageFont> GetPooler(DamageType type) 
    {
        switch (type)
        {
            case DamageType.normal:     return damages;
            case DamageType.red:        return damageReds;
            case DamageType.blue:       return damageBlues;
        }
        return null;
    }
    private void AddPoolingObj(List<DamageFont> poolingObj, DamageType type) 
    {
        GameObject obj;
        switch (type)
        {
            case DamageType.normal:     obj = damageFont; break;
            case DamageType.red:        obj = damageFontRed; break;
            case DamageType.blue:       obj = damageFontBlue; break;
        }
        for (int i = 0; i < 5; i++)
        {
            poolingObj.Add(Instantiate(damageFont, transform).GetComponent<DamageFont>());
        }
    }
}
public enum DamageType
{ 
    normal, red, blue
}