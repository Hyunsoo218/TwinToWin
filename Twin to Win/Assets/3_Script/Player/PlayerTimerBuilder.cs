using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[Serializable]
public class PlayerTimerBuilder : MonoBehaviour
{
    public static PlayerTimerBuilder instance;

    public List<IEnumerator> listTimers = new List<IEnumerator>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }

    public void AddTimer(string coroutineName, float time)
    {
        Coroutine coroutine = StartCoroutine(coroutineName, time);
        Action<IEnumerator> action = (IEnumerator timer) => { StartCoroutine(timer); };
    }

    //private IEnumerator StartDodgeCoolDown()
    //{
    //    while (fDodgeTimer <= fDodgeCoolTime)
    //    {
    //        fDodgeTimer += Time.deltaTime;
    //        yield return null;
    //    }
    //    fDodgeTimer = 0f;
    //}

}
