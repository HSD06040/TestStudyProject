using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ITile
{
    public Vector2Int tilePos {  get; set; }
}

public class Tile : MonoBehaviour, ITile
{
    public Vector2Int tilePos { get; set; }

    public void Init(Vector2Int tilePos)
    {
        this.tilePos = tilePos;
    }
}