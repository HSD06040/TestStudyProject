using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] MapConfig mapConfig;
    [SerializeField] GameObject tileObject;

    TileManager tileManager;

    private void Start()
    {
        Debug.Log("Log");
        Debug.LogWarning("Log Warning");
        Debug.LogError("Log Error");
    }

    [ContextMenu("Create")]
    public void Create()
    {
        tileManager = new TileManager(mapConfig);

        tileManager.Create(mapConfig, tileObject);
    }
}
