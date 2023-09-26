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

    private void Awake()
    {
        instance = this;
    }
}
