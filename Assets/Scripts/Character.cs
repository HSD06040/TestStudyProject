using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridChain = PredChain<Character, Tile>;

public class Character : AutoComponent
{
    public int maxMove = 5;
    public int hp = 100;
    public int damage = 10;

    public Vector2Int tilePos;

    public Rigidbody rb;

    IPred<Character, Tile> canMovePred;

    private void Awake()
    {
        var pred = GridChain.Start(new TilePred.IsMovable()).Build();
        canMovePred = GridChain.Start(new TilePred.IsMovable()).And(new CharacterPred.IsAlive()).Build();
    }

    [ContextMenu("Check Movable")]
    private void CheckMovable()
    {
        if(canMovePred.Test(this, null))
        {
        }
    }
}
