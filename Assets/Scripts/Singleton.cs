using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    [SerializeField] private bool m_bDestroyOnLoad = true;

    protected void Awake()
    {
        if (Instance)
        {
            Debug.LogError($"[Singleton] Instance de type {typeof(T)} à une seconde instance");
            return;
        }

        Instance = (T) this;

        if (!m_bDestroyOnLoad)
        {
            DontDestroyOnLoad(Instance);
        }
    }
}
