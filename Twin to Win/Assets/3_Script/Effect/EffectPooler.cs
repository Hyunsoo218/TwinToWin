using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPooler
{
	private GameObject objOrigin;
	private List<GameObject> arrEffects = new List<GameObject>();

	public EffectPooler(GameObject objOrigin)
	{
		this.objOrigin = objOrigin;
	}
	public GameObject OutPool()
	{
		for (int i = 0; i < arrEffects.Count; i++)
		{
			if (!arrEffects[i].activeSelf)
			{
				arrEffects[i].SetActive(true);
				return arrEffects[i];
			}
		}
		AddObject(10);
		return OutPool();
	}
	public void AddObject(int nCount)
	{
		for (int i = 0; i < nCount; i++)
		{
			GameObject obj = EffectManager.instance.GetClone(objOrigin);
			arrEffects.Add(obj);
			obj.GetComponent<Effect>().Initialize();
			obj.SetActive(false);
		}
	}
	public void DisableAllEffect() 
	{
		foreach (var item in arrEffects)
		{
			item.SetActive(false);
		}
	}
	public void SlowAllEffect(bool active, float slowAmount)
	{
		if (active)
		{
			foreach (var item in arrEffects)
			{
				item.GetComponent<Effect>()?.Slow(slowAmount);
			}
		}
		else
		{
			foreach (var item in arrEffects)
			{
				item.GetComponent<Effect>()?.SlowEnd();
			}
		}
	}
}
