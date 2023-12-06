using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonClass<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance => instance;
    protected virtual void Awake()
    {
        if (instance == null) instance = this as T;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}