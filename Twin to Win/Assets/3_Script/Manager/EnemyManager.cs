using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [SerializeField] private GameObject WTD_tutorial;
    [SerializeField] private GameObject WGS_tutorial_1;
    [SerializeField] private GameObject WGS_tutorial_2;
    [SerializeField] private GameObject WGS_tutorial_3;
    [SerializeField] private GameObject tStage1Enemy1;
    [SerializeField] private GameObject tStage1Enemy2;
    [SerializeField] private GameObject tStage1Enemy3;
    [SerializeField] private GameObject tStage1Enemy4;

    private List<GameObject> prefabs = new List<GameObject>();
    private List<GameObject> clones = new List<GameObject>();
    private int monsterSetNum;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        prefabs.Add(WTD_tutorial);   
        prefabs.Add(WGS_tutorial_1);   
        prefabs.Add(WGS_tutorial_2);
        prefabs.Add(WGS_tutorial_3);   
        prefabs.Add(tStage1Enemy1);   
        prefabs.Add(tStage1Enemy2);   
        prefabs.Add(tStage1Enemy3);   
        prefabs.Add(tStage1Enemy4);   
    }
    public void SetTitle()
    {
        clones.Clear();
    }
    public void SetGame()
    {
        clones.Clear();
        for (int i = 0;i < prefabs.Count; i++) 
        {
            clones.Add(Instantiate(prefabs[i]));
        }
    }
    public void OnActiveEnemy(StageEnemySet set)
    {
        monsterSetNum = (int)set;
        clones[monsterSetNum].SetActive(true);
        StartActionAllEnemy();
    }
    public void StopAllEnemy() 
    {
        foreach (var item in MonsterCharacter.allMonsterCharacters)
            item.StopAction();
    }
    public void StartActionAllEnemy() 
    {
        foreach (var item in MonsterCharacter.allMonsterCharacters) 
            item.StartAction();
    }
    public void SlowAllEnemy(float amount) 
    {
        EffectManager.instance.DisableAllEnemyEffect();
        foreach (var item in MonsterCharacter.allMonsterCharacters)
            item.Slow(amount);
    }
    public void SlowEndAllEnemy()
    {
        foreach (var item in MonsterCharacter.allMonsterCharacters)
            item.SlowEnd();
    }
    public void MonsterDie() 
    {
        if (monsterSetNum < 4) return;
        int monsterCount = MonsterCharacter.allMonsterCharacters.Count;
		if (monsterCount == 0)
		{
            monsterSetNum++;
            OnActiveEnemy((StageEnemySet)monsterSetNum);
        }
    }
}
public enum StageEnemySet 
{
    WTD_tutorial, WGS_tutorial_1, WGS_tutorial_2, WGS_tutorial_3, Stage1_1
}