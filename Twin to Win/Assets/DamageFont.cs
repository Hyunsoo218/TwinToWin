using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageFont : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    public void EnableDamage(float damage) 
    {
        damageText.text = damage + "";
    }
    public void Disable() 
    {
        gameObject.SetActive(false);
    }
}