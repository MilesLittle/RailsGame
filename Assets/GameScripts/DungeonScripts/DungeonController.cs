using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridController;
[RequireComponent(typeof(GameController))]
public class DungeonController : MonoBehaviour
{
    // Start is called before the first frame update

    
    public GameController gameController;
    public Tile dungeonStartTile;
    public int dungeonIndex;
    // Update is called once per frame


    public void initializeDungeon()
    {

        dungeonStartTile = gameController.gridMap[gameController.lvlIndexToStartTiles[dungeonIndex]];
        if(gameController.currentUnit != null)
        {
            PlaceUnit(gameController.currentUnit, (dungeonStartTile.tileCoords.Item1, dungeonStartTile.tileCoords.Item3));
        }
    }
    void Update()
    {
    if (gameController.gameState == GamePlayState.Dungeon)
    {
        if (Input.GetKeyDown(KeyCode.W))
        {

            PlaceUnit(gameController.currentUnit, CheckCoordsDirectionMoving(gameController.currentUnit, 0));
             SetDungeonCamera();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlaceUnit(gameController.currentUnit, CheckCoordsDirectionMoving(gameController.currentUnit, 180));
            SetDungeonCamera();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlaceUnit(gameController.currentUnit, CheckCoordsDirectionMoving(gameController.currentUnit, 270));
            SetDungeonCamera();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlaceUnit(gameController.currentUnit, CheckCoordsDirectionMoving(gameController.currentUnit, 90));
            SetDungeonCamera();
        }
        if ((Input.GetKeyDown(KeyCode.E)))
        {

            ChangeUnitDirection(gameController.currentUnit, 90);
                SetDungeonCamera();

            }
        if ((Input.GetKeyDown(KeyCode.Q)))
        {
            ChangeUnitDirection(gameController.currentUnit, -90);
                SetDungeonCamera();


            }
        {

        }
    }
}

    void OnGUI()
    {
       if(gameController.gameState == GamePlayState.Dungeon)
        {
            GUI.Label(new Rect(10, 10, 200, 20), $"Current Tile: {gameController.currentUnit.currentTile.tileCoords}");
            GUI.Label(new Rect(10, 30, 200, 20), $"Unit Position: {gameController.currentUnit.transform.position}");
            GUI.Label(new Rect(10, 50, 200, 20), $"Unit Direction: {gameController.currentUnit.directionFacing}");
        }
    }

    public void ChangeUnitDirection(Unit unit, int direction)
    {
        if (unit == null || Mathf.Abs(direction) != 90)
        {
            return;
        }
        switch (unit.directionFacing)
        {
            case Unit.DirectionFacing.North:
                if (direction == 90)
                {
                    unit.directionFacing = Unit.DirectionFacing.East;
                    unit.transform.rotation = unit.transform.rotation * Quaternion.Euler(0, 90, 0);
                    
                }
                else if (direction == -90)
                {
                    unit.directionFacing = Unit.DirectionFacing.West;
                    unit.transform.rotation = unit.transform.rotation * Quaternion.Euler(0, -90, 0);
                    
                }
                break;
            case Unit.DirectionFacing.East:
                if (direction == 90)
                {
                    unit.directionFacing = Unit.DirectionFacing.South;
                    unit.transform.rotation = unit.transform.rotation * Quaternion.Euler(0, 90, 0);
                    
                }
                else if (direction == -90)
                {
                    unit.directionFacing = Unit.DirectionFacing.North;
                    unit.transform.rotation = unit.transform.rotation * Quaternion.Euler(0, -90, 0);
                   

                }
                break;
            case Unit.DirectionFacing.South:
                if (direction == 90)
                {
                    unit.directionFacing = Unit.DirectionFacing.West;
                    unit.transform.rotation = unit.transform.rotation * Quaternion.Euler(0, 90, 0);
                    
                }
                else if (direction == -90)
                {
                    unit.directionFacing = Unit.DirectionFacing.East;
                    unit.transform.rotation = unit.transform.rotation * Quaternion.Euler(0, -90, 0);
                }
                break;
            case Unit.DirectionFacing.West:
                if (direction == 90)
                {
                    unit.directionFacing = Unit.DirectionFacing.North;
                    unit.transform.rotation = unit.transform.rotation * Quaternion.Euler(0, 90, 0);
                }
                else if (direction == -90)
                {
                    unit.directionFacing = Unit.DirectionFacing.South;
                    unit.transform.rotation = unit.transform.rotation * Quaternion.Euler(0, -90, 0);
                }
                break;

        }
        gameController.playerCamera.transform.rotation = unit.transform.rotation;
    }
    public void SetDungeonCamera()
    {
        if (gameController.playerCamera != null)
        {
            gameController.playerCamera.transform.position = new Vector3(gameController.currentUnit.unitHead.transform.position.x, gameController.currentUnit.unitHead.transform.position.y, gameController.currentUnit.unitHead.transform.position.z);

        }
    }

    void PlaceUnit(Unit unit, (int, int) tileCoords)
    {
        if (gameController.gridMap.ContainsKey(tileCoords) && IsTileMoveable(tileCoords, unit) == true)
        {
            unit.currentTile = gameController.gridMap[tileCoords];
            unit.transform.position = new Vector3(unit.currentTile.tilePosition.x, unit.currentTile.tilePosition.y + 0.5f, unit.currentTile.tilePosition.z);

            Debug.Log($"Placing unit at tile {tileCoords} with world position {unit.currentTile.tilePosition}");
        }
    }

    public (int, int) CheckCoordsDirectionMoving(Unit unit, int direction)
    {
        (int, int) newCoords = (unit.currentTile.tileCoords.Item1, unit.currentTile.tileCoords.Item3);
        switch (unit.directionFacing)
        {
            case Unit.DirectionFacing.North:
                if (direction == 0)
                {
                    newCoords.Item2 = (unit.currentTile.tileCoords.Item3 + 1);
                    return newCoords;
                }
                else if (direction == 90)
                {
                    newCoords.Item1 = (unit.currentTile.tileCoords.Item1 + 1);
                    return newCoords;
                }
                else if (direction == 180)
                {
                    newCoords.Item2 = (unit.currentTile.tileCoords.Item3 - 1);
                    return newCoords;
                }
                else if (direction == 270)
                {
                    newCoords.Item1 = (unit.currentTile.tileCoords.Item1 - 1);
                    return newCoords;
                }
                else
                {
                    return newCoords;
                }
            case Unit.DirectionFacing.East:
                if (direction == 0)
                {
                    newCoords.Item1 = (unit.currentTile.tileCoords.Item1 + 1);
                    return newCoords;
                }
                else if (direction == 90)
                {
                    newCoords.Item2 = (unit.currentTile.tileCoords.Item3 - 1);
                    return newCoords;
                }
                else if (direction == 180)
                {
                    newCoords.Item1 = (unit.currentTile.tileCoords.Item1 - 1);
                    return newCoords;
                }
                else if (direction == 270)
                {
                    newCoords.Item2 = (unit.currentTile.tileCoords.Item3 + 1);
                    return newCoords;
                }
                else
                {
                    return newCoords;
                }
            case Unit.DirectionFacing.South:
                if (direction == 0)
                {
                    newCoords.Item2 = (unit.currentTile.tileCoords.Item3 - 1);
                    return newCoords;
                }
                else if (direction == 90)
                {
                    newCoords.Item1 = (unit.currentTile.tileCoords.Item1 - 1);
                    return newCoords;
                }
                else if (direction == 180)
                {
                    newCoords.Item2 = (unit.currentTile.tileCoords.Item3 + 1);
                    return newCoords;
                }
                else if (direction == 270)
                {
                    newCoords.Item1 = (unit.currentTile.tileCoords.Item1 + 1);
                    return newCoords;
                }
                else
                {
                    return newCoords;
                }
            case Unit.DirectionFacing.West:
                if (direction == 0)
                {
                    newCoords.Item1 = (unit.currentTile.tileCoords.Item1 - 1);
                    return newCoords;
                }
                else if (direction == 90)
                {
                    newCoords.Item2 = (unit.currentTile.tileCoords.Item3 + 1);
                    return newCoords;
                }
                else if (direction == 180)
                {
                    newCoords.Item1 = (unit.currentTile.tileCoords.Item1 + 1);
                    return newCoords;
                }
                else if (direction == 270)
                {
                    newCoords.Item2 = (unit.currentTile.tileCoords.Item3 - 1);
                    return newCoords;
                }
                else
                {
                    return newCoords;
                }
        }
        return newCoords;

    }

    public bool IsTileMoveable((int, int) tileCoords, Unit unit)
    {
        bool isMoveable = false;
        if (gameController.gridMap.TryGetValue(tileCoords, out Tile tile))
        {
            // Check if the tile is occupied by another unit
            if (tile.unitOnTile == null)
            {

                switch (unit.moveType)
                {
                    case UnitMovementType.Ground:
                        if (Mathf.Abs(tile.tileCoords.Item2 - unit.currentTile.tileCoords.Item2) <= 2)
                        {
                            isMoveable = true;
                        }
                        break;
                    case UnitMovementType.Air:
                        if (Mathf.Abs(tile.tileCoords.Item2 - unit.currentTile.tileCoords.Item2) <= unit.movementRange)
                        {
                            isMoveable = true;
                        }
                        break;

                    case UnitMovementType.Teleport:
                        if (Mathf.Abs(tile.tileCoords.Item2 - unit.currentTile.tileCoords.Item2) <= unit.movementRange)
                        {
                            isMoveable = true;
                        }
                        break;
                    case UnitMovementType.Cavalry:
                        if (Mathf.Abs(tile.tileCoords.Item2 - unit.currentTile.tileCoords.Item2) <= 1)
                        {
                            isMoveable = true;
                        }
                        break;
                }
            }



        }
        return isMoveable;
    }

}
