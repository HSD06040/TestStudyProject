using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{

    private void Start()
    {
        Debug.Log("Log");
        Debug.LogWarning("Warning");
        Debug.LogError("Error");
        Debug.LogException(new System.Exception("Exception"));
        Debug.LogFormat("d"); 
    }
}
