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
    //[SerializeField] private GameObject tStage1Enemy4;
    [SerializeField] private GameObject tStage2;
    [SerializeField] private GameObject tStage3;

    private List<GameObject> tutirialPrefabs = new List<GameObject>();
    private List<GameObject> stage1Prefabs = new List<GameObject>();

    private List<GameObject> tutirialClones = new List<GameObject>();
    private List<GameObject> stage1Clones = new List<GameObject>();
    private List<GameObject> stage2Clones = new List<GameObject>();
    private List<GameObject> stage3Clones = new List<GameObject>();
    private List<GameObject> clones;
    private int monsterSetNum = 0;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        tutirialPrefabs.Add(WTD_tutorial);   
        tutirialPrefabs.Add(WGS_tutorial_1);   
        tutirialPrefabs.Add(WGS_tutorial_2);
        tutirialPrefabs.Add(WGS_tutorial_3);   

        stage1Prefabs.Add(tStage1Enemy1);   
        stage1Prefabs.Add(tStage1Enemy2);   
        stage1Prefabs.Add(tStage1Enemy3);
        //stage1Prefabs.Add(tStage1Enemy4);   
    }
    public void SetTitle()
    {
        stage1Clones.Clear();
        stage2Clones.Clear();
        stage3Clones.Clear();
        tutirialClones.Clear();
    }
    public void SetGame()
    {
        stage1Clones.Clear();
        stage2Clones.Clear();
        stage3Clones.Clear();
        tutirialClones.Clear();

        for (int i = 0;i < stage1Prefabs.Count; i++)  
            stage1Clones.Add(Instantiate(stage1Prefabs[i]));
        for (int i = 0; i < tutirialPrefabs.Count; i++)
            tutirialClones.Add(Instantiate(tutirialPrefabs[i]));
        stage2Clones.Add(Instantiate(tStage2));
        stage3Clones.Add(Instantiate(tStage3));
    }
    public void SetStage(Phase phase) 
    {
		switch (phase)
		{
			case Phase.Phase_1:
                clones = stage1Clones;
                break;
			case Phase.Phase_2:
                clones = stage2Clones;
                break;
			case Phase.Phase_3:
                clones = stage3Clones;
                break;
            case Phase.Tutorial:
                clones = tutirialClones;
                break;
		}
	}
    public void OnActiveEnemy(TutorialEnemySet set)
    {
        monsterSetNum = (int)set;
        if (clones.Count != monsterSetNum)
        {
            clones[monsterSetNum].SetActive(true);
            StartActionAllEnemy();
        }
        else
        {
            monsterSetNum = 0;
            GameManager.instance.StageClear();
        }
    }
    public void OnActiveEnemy(Stage1EnemySet set)
    {
        monsterSetNum = (int)set;
		if (clones.Count != monsterSetNum)
        {
            clones[monsterSetNum].SetActive(true);
            StartActionAllEnemy();
        }
		else
		{
            monsterSetNum = 0;
            GameManager.instance.StageClear();
		}
    }
    public void OnActiveEnemy(Stage2EnemySet set)
    {
        monsterSetNum = (int)set;
        if (clones.Count != monsterSetNum)
        {
            clones[monsterSetNum].SetActive(true);
            StartActionAllEnemy();
        }
        else
        {
            monsterSetNum = 0;
            GameManager.instance.StageClear();
        }
    }
    public void OnActiveEnemy(Stage3EnemySet set)
    {
        monsterSetNum = (int)set;
        if (clones.Count != monsterSetNum)
        {
            clones[monsterSetNum].SetActive(true);
            StartActionAllEnemy();
        }
        else
        {
            monsterSetNum = 0;
            GameManager.instance.StageClear();
        }
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
        if (clones == tutirialClones) return;
        int monsterCount = MonsterCharacter.allMonsterCharacters.Count;
		if (monsterCount == 0)
		{
            monsterSetNum++;
            OnActiveEnemy((Stage1EnemySet)monsterSetNum);
        }
    }
}
public enum TutorialEnemySet
{
    WTD_tutorial, WGS_tutorial_1, WGS_tutorial_2, WGS_tutorial_3
}
public enum Stage1EnemySet 
{
    Stage1_1
}
public enum Stage2EnemySet
{
    Boss_normal
}
public enum Stage3EnemySet
{
    Boss_angry
}