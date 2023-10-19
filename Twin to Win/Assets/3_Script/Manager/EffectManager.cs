using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	public static EffectManager instance;

	[SerializeField] private Material mHitEffectRed;
	[SerializeField] private List<Effect> enemyPoolingEffect;
	[SerializeField] private List<Effect> enemyPoolingEffectBomb;
	[SerializeField] private List<Effect> playerPoolingEffect;

	private Dictionary<GameObject, EffectPooler> dicEnemyPoolers = new Dictionary<GameObject, EffectPooler>();
	private Dictionary<GameObject, EffectPooler> dicEnemyBombPoolers = new Dictionary<GameObject, EffectPooler>();
	private Dictionary<GameObject, EffectPooler> dicPlayerPoolers = new Dictionary<GameObject, EffectPooler>();
	private List<GameObject> allEffects = new List<GameObject>();
	private void Awake()
	{
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
	}
	public void SetGame() 
	{
		dicEnemyPoolers.Clear();
		dicPlayerPoolers.Clear();
		dicEnemyBombPoolers.Clear();
		foreach (var item in enemyPoolingEffect)
		{
			EffectPooler cAddPooler = new EffectPooler(item.gameObject, 5);
			dicEnemyPoolers.Add(item.gameObject, cAddPooler);
		}
		foreach (var item in playerPoolingEffect)
		{
			EffectPooler cAddPooler = new EffectPooler(item.gameObject, 5);
			dicPlayerPoolers.Add(item.gameObject, cAddPooler);
		}
		foreach (var item in enemyPoolingEffectBomb)
		{
			EffectPooler cAddPooler = new EffectPooler(item.gameObject, 308);
			dicEnemyBombPoolers.Add(item.gameObject, cAddPooler);
		}
		DisableAllEffect(); 
	}
    public void SetTitle()
    {

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
		if (dicEnemyBombPoolers.ContainsKey(objPrefab))
		{
			return dicEnemyBombPoolers[objPrefab].OutPool();
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
    public void DisableAllEffect()
    {
		DisableAllEnemyEffect();
        foreach (var item in dicPlayerPoolers)
        {
            item.Value.DisableAllEffect();
        }
    }
	public GameObject GetClone(GameObject prefab) 
	{
		return Instantiate(prefab);
	}
	public void SlowEffect(bool active, float slowAmount = 0.1f) 
	{
		foreach (var poolers in dicEnemyPoolers)
		{
			poolers.Value.SlowAllEffect(active, slowAmount);
		}
	}
}