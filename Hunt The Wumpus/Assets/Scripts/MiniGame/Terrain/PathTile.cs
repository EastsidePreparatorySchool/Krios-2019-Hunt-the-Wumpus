using System.Collections;
using System.Collections.Generic;
using MiniGame;
using UnityEngine;

public class PathTile : ITile
{
    private const bool Walkable = true;

    public GameObject tileGameObject;
    
    public bool CanWalkOn()
    {
        return Walkable;
    }

    public PathTile(GameObject tileGameObject)
    {
        this.tileGameObject = tileGameObject;
    }
    
}
