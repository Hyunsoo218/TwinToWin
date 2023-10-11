using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPooler : MonoBehaviour
{
	private GameObject objOrigin;
	private List<GameObject> arrEffects = new List<GameObject>();

	public EffectPooler(GameObject objOrigin)
	{
		this.objOrigin = objOrigin;
		AddObject(5);
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
		AddObject(5);
		return OutPool();
	}
	private void AddObject(int nCount)
	{
		for (int i = 0; i < nCount; i++)
		{
			GameObject obj = Instantiate(objOrigin, EffectManager.instance.transform);
			arrEffects.Add(obj);
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
}
