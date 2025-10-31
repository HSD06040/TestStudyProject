using UnityEngine;

public struct Node
{
    public Vector3 position;
    public bool isWalkable;

    public Node(Vector3 pos, bool walkable)
    {
        position = pos;
        isWalkable = walkable;
    }
}