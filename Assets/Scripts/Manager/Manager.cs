using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Manager
{
    public static PoolManager Pool => PoolManager.Instance;
    public static AudioManager Audio => AudioManager.Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        PoolManager.CreateInstance();
        AudioManager.CreateInstance();        
    }
}
