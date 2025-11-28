using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.UIElements;

public class GridController : MonoBehaviour
{
    [OdinSerialize, ReadOnly] public Dictionary<(int, int), Tile> gridMap = new Dictionary<(int, int), Tile>();
   public byte gridWidth;
   public byte gridLength;
    public LayerMask terrainMask;
    public Material lineMaterial;
    public GameObject myTerrain;
    public float tileSize = 1f;
    public GameController gameController;
   
    public enum TacticsState
    {
        PlayerTurn,
        EnemyTurn
    }

    public TacticsState tacticsState;
    private void Awake()
    {
        initializeGrid();
    }
    private void initializeGrid()
    {

        ;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                // Compute the world position for the tile center
                
                Vector3 tileCenter = new Vector3(i * tileSize, 100f, j * tileSize); // high Y for downward cast

                // Define the box size (half extents)
                Vector3 boxHalfExtents = new Vector3(tileSize * .5f , 0.1f, tileSize * .5f);
                
                float castDistance = 200f;

                if (Physics.BoxCast(tileCenter, boxHalfExtents, Vector3.down, out RaycastHit hit, Quaternion.identity, castDistance, terrainMask))
                {

                    Vector3 hitPosition = hit.point;
                   
                    Tile tile = new Tile(
                        moveCost: Random.Range(1,5),
                        tileCoords: (i, hit.point.y,j),
                        tilePosition: new Vector3(tileCenter.x, hitPosition.y, tileCenter.z)
                    );

                    gridMap.Add((i, j), tile);

                   // Debug.Log($"Tile ({i}, {j}) at world pos {hitPosition}");
                }
            }
        }
    }

    public Tile GetTileAt(Vector2Int coords)
    {
        gridMap.TryGetValue((coords.x, coords.y), out Tile tile);
        return tile;
    }

    public bool CanTraverse(Unit unit, Tile fromTile, Tile toTile)
    {
        if(toTile.unitOnTile != null && toTile.unitOnTile != unit)
        {
            return false;
        }
        float heightDiff = Mathf.Abs(toTile.tilePosition.y - fromTile.tilePosition.y);
        switch (unit.moveType)
        {
            case UnitMovementType.Air:
                return heightDiff <= 20f;
            case UnitMovementType.Ground:
                return heightDiff <= 2f;
            case UnitMovementType.Cavalry:
                return heightDiff <= 1f;
            case UnitMovementType.Teleport:
                return true;
            default:
                return false;
        }
    }

   
               
 

    
 
    



        /* private void OnDrawGizmos()
         {
             Gizmos.color = Color.cyan;

             if (gridMap != null)
             {
                 foreach (var kvp in gridMap)
                 {
                     Tile tile = kvp.Value;
                     Vector3 pos = tile.tilePosition;

                     // Draw a wireframe cube or sphere at each tile's world position
                     Gizmos.DrawWireCube(pos + Vector3.up * 0.1f, Vector3.one * 3f);
                     // Or use Gizmos.DrawSphere(pos + Vector3.up * 0.1f, 0.2f);
                 }
             }
         } */



    }
