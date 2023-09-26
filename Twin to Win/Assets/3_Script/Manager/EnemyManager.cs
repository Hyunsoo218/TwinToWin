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

    private void Awake()
    {
        instance = this;
    }
    public void OnActiveEnemy(StageEnemySet set)
    {
        switch (set)
        {
            case StageEnemySet.Stage1_1: tStage1Enemy1.SetActive(true); break;
        }
    }
}
public enum StageEnemySet 
{
    Stage1_1
}