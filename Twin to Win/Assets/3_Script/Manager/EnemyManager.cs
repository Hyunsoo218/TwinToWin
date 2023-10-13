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

    private List<GameObject> prefabs = new List<GameObject>();
    private List<GameObject> clones = new List<GameObject>();
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        prefabs.Add(WTD_tutorial);   
        prefabs.Add(WGS_tutorial_1);   
        prefabs.Add(WGS_tutorial_2);
        prefabs.Add(WGS_tutorial_3);   
        prefabs.Add(tStage1Enemy1);   
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
        clones[(int)set].SetActive(true);
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
    public void SlowAllEnemy(float time, float amount) 
    {
        EffectManager.instance.DisableAllEnemyEffect();
        foreach (var item in MonsterCharacter.allMonsterCharacters)
            item.Slow(time, amount);
    }
    public void SlowEndAllEnemy()
    {
        EffectManager.instance.DisableAllEnemyEffect();
        foreach (var item in MonsterCharacter.allMonsterCharacters)
            item.SlowEnd();
    }
}
public enum StageEnemySet 
{
    WTD_tutorial, WGS_tutorial_1, WGS_tutorial_2, WGS_tutorial_3, Stage1_1
}