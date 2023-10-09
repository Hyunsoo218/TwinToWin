using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [SerializeField] private GameObject objBoss;
    [SerializeField] private GameObject objBeez;
    [SerializeField] private GameObject objTree;
    [SerializeField] private GameObject objPlanta;
    [SerializeField] private GameObject objTurnipa;
    [SerializeField] private GameObject tStage1Enemy1;
    [SerializeField] private GameObject WTD_tutorial;
    [SerializeField] private GameObject WGS_tutorial_1;
    [SerializeField] private GameObject WGS_tutorial_2;
    [SerializeField] private GameObject WGS_tutorial_3;

    private void Awake()
    {
        instance = this;
    }
    public void OnActiveEnemy(StageEnemySet set)
    {
        switch (set)
        {
            case StageEnemySet.WTD_tutorial: WTD_tutorial.SetActive(true); break;
            case StageEnemySet.WGS_tutorial_1: WGS_tutorial_1.SetActive(true); break;
            case StageEnemySet.WGS_tutorial_2: WGS_tutorial_2.SetActive(true); break;
            case StageEnemySet.WGS_tutorial_3: WGS_tutorial_3.SetActive(true); break;
            case StageEnemySet.Stage1_1: tStage1Enemy1.SetActive(true); break;
        }
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
}
public enum StageEnemySet 
{
    WTD_tutorial, WGS_tutorial_1, WGS_tutorial_2, WGS_tutorial_3, Stage1_1
}