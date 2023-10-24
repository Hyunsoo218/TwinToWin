using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	public static EffectManager instance;
	public int finishedPoolingObjCount = 0;
	public int poolingObjCount = 0;

	[SerializeField] private Material mHitEffectRed;
	[SerializeField] private GameObject monsterEnableEffect;
	[SerializeField] private List<Effect> enemyPoolingEffect;
	[SerializeField] private List<Effect> enemyPoolingEffectBomb;
	[SerializeField] private List<Effect> playerPoolingEffect;

	private Dictionary<GameObject, EffectPooler> dicEnemyPoolers = new Dictionary<GameObject, EffectPooler>();
	private Dictionary<GameObject, EffectPooler> dicEnemyBombPoolers = new Dictionary<GameObject, EffectPooler>();
	private Dictionary<GameObject, EffectPooler> dicPlayerPoolers = new Dictionary<GameObject, EffectPooler>();
	private List<GameObject> allEffects = new List<GameObject>();
	private EffectPooler monsterEnableEffects;

	private void Awake()
	{
        if (instance == null) instance = this;
        else Destroy(gameObject);
		poolingObjCount = enemyPoolingEffect.Count * 5;
		poolingObjCount += playerPoolingEffect.Count * 5;
		poolingObjCount += enemyPoolingEffectBomb.Count * 310;
		poolingObjCount += 10;
	}
	public IEnumerator SetGame() 
	{
		dicEnemyPoolers.Clear();
		dicPlayerPoolers.Clear();
		dicEnemyBombPoolers.Clear();
		finishedPoolingObjCount = 0;
		monsterEnableEffects = new EffectPooler(monsterEnableEffect);

		foreach (var item in enemyPoolingEffect)
		{
			EffectPooler cAddPooler = new EffectPooler(item.gameObject);
			for (int i = 0; i < 5; i++)
			{
				cAddPooler.AddObject(1);
				finishedPoolingObjCount++;
				yield return null;
			}
			dicEnemyPoolers.Add(item.gameObject, cAddPooler);
		}
		foreach (var item in playerPoolingEffect)
		{
			EffectPooler cAddPooler = new EffectPooler(item.gameObject);
			for (int i = 0; i < 5; i++)
			{
				cAddPooler.AddObject(1);
				finishedPoolingObjCount++;
				yield return null;
			}
			dicPlayerPoolers.Add(item.gameObject, cAddPooler);
		}
		foreach (var item in enemyPoolingEffectBomb)
		{
			EffectPooler cAddPooler = new EffectPooler(item.gameObject);
			for (int i = 0; i < 310; i++)
			{
				cAddPooler.AddObject(1);
				finishedPoolingObjCount++;
				yield return null;
			}
			dicEnemyBombPoolers.Add(item.gameObject, cAddPooler);
		}
		for (int i = 0; i < 10; i++)
		{
			monsterEnableEffects.AddObject(1);
		}
		DisableAllEffect(); 
	}
	public GameObject GetMonsterEnableEffect() 
	{
		return monsterEnableEffects.OutPool();
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