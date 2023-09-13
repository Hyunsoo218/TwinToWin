using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
	[SerializeField] protected float fRunTime = 3f;
	protected virtual void OnEnable() => Invoke("InPool", fRunTime); 
	protected virtual void InPool() => gameObject.SetActive(false);
	public virtual void OnAction(Transform tUser, float fDamage, int nTargetLayer) => print("함수를 오버라이드 하세요");
}
