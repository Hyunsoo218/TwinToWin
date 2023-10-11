using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	public static EffectManager instance;

	[SerializeField] private Material mHitEffectRed;
	[SerializeField] private List<Effect> enemyPoolingEffect;
	[SerializeField] private List<Effect> playerPoolingEffect;

	private Dictionary<GameObject, EffectPooler> dicEnemyPoolers = new Dictionary<GameObject, EffectPooler>();
	private Dictionary<GameObject, EffectPooler> dicPlayerPoolers = new Dictionary<GameObject, EffectPooler>();

	private void Awake()
	{
		instance = this;
		foreach (var item in enemyPoolingEffect)
		{
			EffectPooler cAddPooler = new EffectPooler(item.gameObject);
			dicEnemyPoolers.Add(item.gameObject, cAddPooler);
		}
		foreach (var item in playerPoolingEffect)
		{
			EffectPooler cAddPooler = new EffectPooler(item.gameObject);
			dicPlayerPoolers.Add(item.gameObject, cAddPooler);
		}
	}
	public GameObject GetEffect(GameObject objPrefab)
	{
		if (dicEnemyPoolers.ContainsKey(objPrefab))
		{
			return dicEnemyPoolers[objPrefab].OutPool();
		}
		if (dicPlayerPoolers.ContainsKey(objPrefab))
		{
			return dicPlayerPoolers[objPrefab].OutPool();
		}
		return Instantiate(objPrefab);
	}
	public Material GetHitEffect() 
	{
		return mHitEffectRed;
	}
	public void DisableAllEnemyEffect() 
	{
		foreach (var item in dicEnemyPoolers)
		{
			item.Value.DisableAllEffect();
		}
	}
}
