using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonClass<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance 
    {
        get 
        {
            if (instance == null)
                print("instance = null");
            return instance;
        }
    }
    protected virtual void Awake()
    {
        if (Instance == null) instance = this as T;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
