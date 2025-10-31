using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float nodeSize = 1f;
    public TextMeshPro textPrefab;
    public Material dangerZoneMaterial;

    Node[,] grid;
    readonly Dictionary<Vector3, int> nodeSectorData = new Dictionary<Vector3, int>();

    void PrecomputeSector()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var node in grid)
        {
            if (!node.isWalkable)
                continue;

            //int sectorMask = 0;

            foreach(var enemy in enemies)
            {
                if (enemy.GetComponent<MonoBehaviour>() is not { } enemyComponent) continue;
            }
        }
    }

    void InitializeGrid()
    {
        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                var pos = new Vector3(x, 0, z) * nodeSize;
                grid[x, z] = new Node(pos, IsPositionOnNavMesh(pos));
            }
        }
    }

    bool IsPositionOnNavMesh(Vector3 position) => NavMesh.SamplePosition(position, out _, nodeSize / 2, NavMesh.AllAreas);
}
