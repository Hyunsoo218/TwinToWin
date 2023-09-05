using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	public static EffectManager instance;

	[SerializeField] private List<Effect> arrPoolingEffect;

	private Dictionary<GameObject, EffectPooler> dicPoolers = new Dictionary<GameObject, EffectPooler>();

	private void Awake()
	{
		instance = this;
		foreach (var item in arrPoolingEffect)
		{
			EffectPooler cAddPooler = new EffectPooler(item.gameObject);
			dicPoolers.Add(item.gameObject, cAddPooler);
			print(item.gameObject.name + " 풀러 생성");
		}
	}
	public GameObject GetEffect(GameObject objPrefab)
	{
		if (dicPoolers.ContainsKey(objPrefab))
		{
			return dicPoolers[objPrefab].OutPool();
		}
		return Instantiate(objPrefab);
	}
}
