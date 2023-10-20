using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageFont : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    private Vector3 targetPos;

    private void Update()
	{
        transform.position = Camera.main.WorldToScreenPoint(targetPos);
    }
	public void EnableDamage(float damage, Vector3 targetPos) 
    {
        damageText.text = damage.ToString("N0");
        this.targetPos = targetPos;
        transform.position = Camera.main.WorldToScreenPoint(targetPos);
    }
    public void Disable() 
    {
        gameObject.SetActive(false);
    }
}