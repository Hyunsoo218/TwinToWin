using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class BossStateUI : MonoBehaviour
{
    [SerializeField] private Hpbar hpbar;
    public void Initialize(float max)
    {
        hpbar.Initialize(max, 0, max);
    }
    public void Set(float hp) 
    {
        hpbar.Set(hp);
    }
}
