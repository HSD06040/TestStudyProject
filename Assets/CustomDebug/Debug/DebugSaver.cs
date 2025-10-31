using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSaver : MonoBehaviour
{
    private void Awake() => DontDestroyOnLoad(gameObject);

    private void OnApplicationQuit() => Debug.Save();
}
