using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile 
{
    public Unit unitOnTile;
   public  int moveCost;
    public (int,float,int) tileCoords;
    public Vector3 tilePosition;
    public int tileCost;

    public Tile(int moveCost,  (int,float,int) tileCoords, Vector3 tilePosition)
    {
        this.moveCost = moveCost;
        this.tileCoords = tileCoords;
        this.tilePosition = tilePosition;
    }
    public void SetHighlighted(bool on)
    {
        
        
    }

}

public class PathNode
{
    public Tile tile;
    public PathNode parent;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
}
