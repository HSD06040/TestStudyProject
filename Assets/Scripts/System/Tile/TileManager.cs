using System.Collections.Generic;
using UnityEngine;

public class TileManager
{
    private readonly Dictionary<Vector2Int, Tile> tiles;

    public TileManager(MapConfig mapConfig)
    {
        tiles = new Dictionary<Vector2Int, Tile>(mapConfig.GetCapecity());
    }

    public void Create(MapConfig mapConfig, GameObject tileObject)
    {        
        for (int y = 1; y < mapConfig.mapSize.y + 1; y++)
        {
            for(int x = 1; x < mapConfig.mapSize.x + 1; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Tile tile = Object.Instantiate(tileObject).GetComponent<Tile>();

                tile.Init(pos);
                tiles.Add(pos, tile);
            }
        }
    }

    public Tile GetTile(Vector2Int pos)
    {
        return tiles.TryGetValue(pos, out var tile) ? tile : null;
    }
}

public class MapConfig : ScriptableObject
{    
    public Vector3 mapSpawnPosition;
    public Vector2Int mapSize;

    public int GetCapecity()
    {
        return mapSize.x * mapSize.y;
    }
}