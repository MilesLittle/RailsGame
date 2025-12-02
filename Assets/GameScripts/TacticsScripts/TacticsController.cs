using JetBrains.Annotations;
using Sirenix.Reflection.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public enum TacticsState
{
    PlayerTurn,
    PlayerActionSelect,
    PlayerChoosingTile,
    PlayerSkillQueueing,
    EnemyTurn,
    MovingTiles,
    ShiftingTurn,
    NonPlayerTurn
}

public enum TacticsCameraState
{
    SelectedUnit,
    SelectedTile,
    FollowingMovement,
    
}

public enum TileSelectionMode
{
    None,
    Movement,
    SkillTargeting
}
public class TacticsController : MonoBehaviour
{

    public TacticsCameraState tacticsCameraState;
    public TacticsState tacticsState;
    public GameController gameController;
    private int selectedAction = 0;
    private string[] actionOptions = { "Move", "Skill", "Item", "End Turn" };
    public Queue<Unit> turnQueue = new Queue<Unit>();
    public Skill selectedSkill;
    public DirectionFacing selectedDirection = DirectionFacing.North;

    public TileSelectionMode tileSelectionMode = TileSelectionMode.None;

    public List<Tile> validSkillTargetTiles = new List<Tile>();

    public HashSet<Tile> highlightedTiles = new HashSet<Tile>();
   
    private void Awake()
    {
       
    }
    void OnGUI()
    {
        if (gameController.gameState == GamePlayState.Tactics)
        {
            // ---------------- PLAYER ACTION SELECT ----------------
            if (tacticsState == TacticsState.PlayerActionSelect)
            {
                GUI.Label(new Rect(10, 10, 200, 20), "Choose Action:");

                for (int i = 0; i < actionOptions.Length; i++)
                {
                    GUI.color = (i == selectedAction) ? Color.yellow : Color.white;
                    if (GUI.Button(new Rect(10, 40 + i * 30, 100, 25), actionOptions[i]))
                    {
                        HandleActionSelect(actionOptions[i]);
                    }
                }

                GUI.color = Color.white;
            }
            if(tacticsState == TacticsState.PlayerSkillQueueing)
            {
                GUI.Label(new Rect(200, 10, 200, 20), $"Current Direction: {selectedDirection.ToString()}");
                List<Skill> skills = gameController.currentUnit.GetAvailableSkills();
                for(int i = 0; i < skills.Count; i++)
                {
                    if(GUI.Button(new Rect(10, 40 + i * 30, 150, 25), skills[i].skillName))
                    {
                        selectedSkill = skills[i];
                        selectedDirection = gameController.currentUnit.directionFacing;
                        validSkillTargetTiles = GetValidTargetTiles(gameController.currentUnit, selectedSkill, selectedDirection);

                        if(validSkillTargetTiles.Count > 0)
                        {
                            gameController.currentlySelectedTile = validSkillTargetTiles[0];
                        } else
                        {
                            gameController.currentlySelectedTile = gameController.currentUnit.currentTile;
                        }

                        tileSelectionMode = TileSelectionMode.SkillTargeting;
                    }
                }
            }

            
            

                GUI.enabled = true;
            }
        }


    private void HighlightSkillArea()
    {
        // Clear previous highlights
     
        highlightedTiles.Clear();

        if (selectedSkill != null && gameController.currentlySelectedTile != null)
        {
            highlightedTiles.AddRange(GetTilesInArea(gameController.currentlySelectedTile, selectedSkill.areaType, selectedSkill.areaSize, selectedDirection));
          
        }
    }
    private void HandleActionSelect(string action)
    {
        switch(action)
        {
            case "Move":
                tacticsState = TacticsState.PlayerChoosingTile;
                gameController.currentUnit.traversableTiles = GetReachableTiles(gameController.currentUnit);
                SetSelectedTile(gameController.currentUnit.currentTile);
                
                tacticsCameraState = TacticsCameraState.SelectedTile;
                moveCamera();
                break;
            case "Skill":
                tacticsState = TacticsState.PlayerSkillQueueing;
                break;

           
            case "Item":
                Debug.Log("No Item Menu Yet");
                    break;

            case "End Turn":
                tacticsState = TacticsState.ShiftingTurn;
                shiftTurnState();
                break;
        }
    }
    public HashSet<Tile> GetReachableTiles(Unit unit)
    {
        Debug.Log("GetReachableTiles called");
        var reachableTiles = new HashSet<Tile>();
        var frontier = new Queue<(Tile tile, int remainingMovement)>();
        var visited = new Dictionary<Tile, int>();
        reachableTiles.Add(unit.currentTile); // Start with the current tile
        frontier.Enqueue((unit.currentTile, unit.movementRange));
        visited[unit.currentTile] = unit.movementRange;

        while (frontier.Count > 0)
        {
            var (currentTile, remaining) = frontier.Dequeue();

            foreach (Tile neighbor in gameController.FindAdjacentTiles(currentTile))
            {
                // Skip if enemy/ally unit blocks tile (except self)
                if (neighbor.unitOnTile != null && neighbor.unitOnTile != unit)
                    continue;

                // Height difference check
                float heightDifference = Mathf.Abs(neighbor.tilePosition.y - currentTile.tilePosition.y);
                bool canTraverse = false;

                switch (unit.moveType)
                {
                    case UnitMovementType.Air:
                        canTraverse = heightDifference <= 20f; // Max flying height
                        break;
                    case UnitMovementType.Ground:
                        canTraverse = heightDifference <= 2f;
                        break;
                    case UnitMovementType.Cavalry:
                        canTraverse = heightDifference <= 1f;
                        break;
                    case UnitMovementType.Teleport:
                        canTraverse = true; // No height restriction
                        break;
                }

                if (!canTraverse)
                    continue;

                int moveCost = (unit.moveType == UnitMovementType.Air) ? 1 : neighbor.moveCost;
                int remainingAfterMove = remaining - moveCost;

                if (remainingAfterMove < 0)
                    continue;

                if (!visited.ContainsKey(neighbor) || visited[neighbor] < remainingAfterMove)
                {
                    visited[neighbor] = remainingAfterMove;
                    reachableTiles.Add(neighbor);
                    frontier.Enqueue((neighbor, remainingAfterMove));
                }
            }
        }

        return reachableTiles;
    }


  
    public void SetSelectedTile(Tile tile)
    {
        if(gameController.currentlySelectedTile == null && gameController.currentUnit.traversableTiles.Contains(tile))
        {
            gameController.currentlySelectedTile = tile;
        }
    }

    public void changeSelectedTile((int, int) direction, Tile currentTile)
    {
        Tile newTile;
        if (gameController.gameState != GamePlayState.Tactics)
            return;

        if(gameController.currentlySelectedTile == null || !gameController.currentUnit.traversableTiles.Contains(currentTile))
        {
            gameController.currentlySelectedTile = currentTile;
            return;
        }
        newTile = gameController.findExistingTile((currentTile.tileCoords.Item1 + direction.Item1), (currentTile.tileCoords.Item3 + direction.Item2));
        if (gameController.currentUnit.traversableTiles.Contains(newTile)) {
            gameController.currentlySelectedTile = newTile;
        }

        
    }

   /* public void moveToSelectedTile(Tile tile, Unit unit)
    {
        if (gameController.gameState == GamePlayState.Tactics)
        {
            if (tacticsState == TacticsState.PlayerChoosingTile)
            {
                    if(unit.traversableTiles.Contains(tile))
                {

                }
            }
            else
            {
                Debug.Log("Cannot select tile, not in PlayerChoosingTile state.");
            }
        }
    } */


    public List<Tile> FindPath(Unit unit, Tile startTile, Tile targetTile)
    {
        var openSet = new PriorityQueue<Tile>();
        var cameFrom = new Dictionary<Tile, Tile>();
        var gScore = new Dictionary<Tile, int>();
        var fScore = new Dictionary<Tile, int>();

         gScore[startTile] = 0;
        fScore[startTile] = HeuristicCostEstimate(startTile, targetTile);
        openSet.Enqueue(startTile, fScore[startTile]);

        while (openSet.Count > 0)
        {
            Tile current = openSet.Dequeue();

            if (current == targetTile)
            {
                return ReconstructPath(cameFrom, current, unit);

            }

            foreach (Tile neighbor in gameController.FindAdjacentTiles(current))
            {
                if (!gameController.gridController.CanTraverse(unit, current, neighbor))
                {
                    continue;
                }

                int moveCost = (unit.moveType == UnitMovementType.Air) ? 1 : neighbor.moveCost;
                int tentativeGScore = gScore[current] + moveCost;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + HeuristicCostEstimate(neighbor, targetTile);

                    if(!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        return null;
    }

    private List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current, Unit unit)
    {
        var path = new List<Tile> { current };
        while(cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);

        }
        path.Reverse();
        StartCoroutine(moveAlongPath(gameController.currentUnit, path));
        
     
        return path;// Clear the selected tile after pathfinding
    }
    private int HeuristicCostEstimate(Tile a, Tile b)
    {
        // Using Manhattan distance as heuristic
        return Mathf.Abs(a.tileCoords.Item1 - b.tileCoords.Item1) + Mathf.Abs(a.tileCoords.Item3 - b.tileCoords.Item3);
    }
    private void Update()
    {
        if (gameController.gameState == GamePlayState.Tactics)
        {
            if (tacticsState == TacticsState.PlayerTurn)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {

                    TacticsStateSwap();
                }
            }
            else if (tacticsState == TacticsState.PlayerChoosingTile)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    FindPath(gameController.currentUnit, gameController.currentUnit.currentTile, gameController.currentlySelectedTile);
                    TacticsStateSwap();

                    Debug.Log("Player Turn: Tile Selected, Turn Ended");

                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    changeSelectedTile((0, 1), gameController.currentlySelectedTile);
                    moveCamera();
                    Debug.Log("W pressed, moving up to tile: " + gameController.currentlySelectedTile.tileCoords);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    changeSelectedTile((0, -1), gameController.currentlySelectedTile);
                    moveCamera();
                    Debug.Log("S pressed, moving down to tile: " + gameController.currentlySelectedTile.tileCoords);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    changeSelectedTile((-1, 0), gameController.currentlySelectedTile);
                    moveCamera();
                    Debug.Log("A pressed, moving left to tile: " + gameController.currentlySelectedTile.tileCoords);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    changeSelectedTile((1, 0), gameController.currentlySelectedTile);
                    moveCamera();
                    Debug.Log("D pressed, moving right to tile: " + gameController.currentlySelectedTile.tileCoords);

                }
            }
            else if (tacticsState == TacticsState.PlayerSkillQueueing && tileSelectionMode == TileSelectionMode.SkillTargeting)
            {
                HighlightSkillArea();

                int currentIndex = validSkillTargetTiles.IndexOf(gameController.currentlySelectedTile);
                int newIndex = currentIndex;

                bool directionFlipped = false;

                switch (selectedDirection)
                {
                    case DirectionFacing.North:
                        if (Input.GetKeyDown(KeyCode.W) && currentIndex < validSkillTargetTiles.Count - 1)
                            newIndex++;
                        else if (Input.GetKeyDown(KeyCode.S))
                        {
                            if (currentIndex > 0)
                                newIndex--;
                            else
                            {
                                selectedDirection = DirectionFacing.South;
                                directionFlipped = true;
                            }
                        }
                        break;
                    case DirectionFacing.South:
                        if (Input.GetKeyDown(KeyCode.S) && currentIndex < validSkillTargetTiles.Count - 1)
                            newIndex++;
                        else if (Input.GetKeyDown(KeyCode.W))
                        {
                            if (currentIndex > 0)
                                newIndex--;
                            else
                            {
                                selectedDirection = DirectionFacing.North;
                                directionFlipped = true;
                            }
                        }
                        break;
                    case DirectionFacing.East:
                        if (Input.GetKeyDown(KeyCode.D) && currentIndex < validSkillTargetTiles.Count - 1)
                            newIndex++;
                        else if (Input.GetKeyDown(KeyCode.A))
                        {
                            if (currentIndex > 0)
                                newIndex--;
                            else
                            {
                                selectedDirection = DirectionFacing.West;
                                directionFlipped = true;
                            }
                        }
                        break;
                    case DirectionFacing.West:
                        if (Input.GetKeyDown(KeyCode.A) && currentIndex < validSkillTargetTiles.Count - 1)
                            newIndex++;
                        else if (Input.GetKeyDown(KeyCode.D))
                        {
                            if (currentIndex > 0)
                                newIndex--;
                            else
                            {
                                selectedDirection = DirectionFacing.East;
                                directionFlipped = true;
                            }
                        }
                        break;
                }

                // Handle direction flip and reset index
                if (directionFlipped)
                {
                    validSkillTargetTiles = GetValidTargetTiles(gameController.currentUnit, selectedSkill, selectedDirection);
                    if (validSkillTargetTiles.Count > 0)
                    {
                        gameController.currentlySelectedTile = validSkillTargetTiles[0];
                        moveCamera();
                    }
                    return; // Skip the rest of the block for this frame
                }

                // Update selected tile if index changed
                if (newIndex != currentIndex && newIndex >= 0 && newIndex < validSkillTargetTiles.Count)
                {
                    gameController.currentlySelectedTile = validSkillTargetTiles[newIndex];
                    moveCamera();
                }

                // Handle direction change and keep index
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        switch (selectedDirection)
                        {
                            case DirectionFacing.North: selectedDirection = DirectionFacing.East; break;
                            case DirectionFacing.East: selectedDirection = DirectionFacing.South; break;
                            case DirectionFacing.South: selectedDirection = DirectionFacing.West; break;
                            case DirectionFacing.West: selectedDirection = DirectionFacing.North; break;
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Q))
                    {
                        switch (selectedDirection)
                        {
                            case DirectionFacing.North: selectedDirection = DirectionFacing.West; break;
                            case DirectionFacing.West: selectedDirection = DirectionFacing.South; break;
                            case DirectionFacing.South: selectedDirection = DirectionFacing.East; break;
                            case DirectionFacing.East: selectedDirection = DirectionFacing.North; break;
                        }
                    }

                    validSkillTargetTiles = GetValidTargetTiles(gameController.currentUnit, selectedSkill, selectedDirection);

                    if (validSkillTargetTiles.Count > 0)
                    {
                        int clampedIndex = Mathf.Clamp(currentIndex, 0, validSkillTargetTiles.Count - 1);
                        gameController.currentlySelectedTile = validSkillTargetTiles[clampedIndex];
                    }
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    bool success = TryUseSkill(gameController.currentUnit, selectedSkill, gameController.currentlySelectedTile, selectedDirection);
                    if (success)
                    {
                        tacticsState = TacticsState.PlayerActionSelect;
                        tileSelectionMode = TileSelectionMode.None;
                        highlightedTiles.Clear();
                    }
                }
            }
            if (tacticsCameraState == TacticsCameraState.FollowingMovement)
            {
                gameController.playerCamera.transform.position = transform.InverseTransformPoint(gameController.currentUnit.transform.position + new Vector3(0, 15, -15));
            }

        }
    }
    public IEnumerator moveAlongPath(Unit unit, List<Tile> path, float moveSpeed =4f)
    {
        if (path == null || path.Count == 0) 
        {
            yield break;
        }

        for (int i = 1; i < path.Count; i++)
        {
            Tile startTile = path[i - 1];
            Tile targetTile = path[i];
            Vector3 start = startTile.tilePosition;
            Vector3 end = targetTile.tilePosition;

            float t = 0;
            while(t < 1f)
            {
                t += Time.deltaTime * moveSpeed;
                unit.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            unit.transform.position = end;
            unit.currentTile.unitOnTile = null;
            targetTile.unitOnTile = unit;
            unit.currentTile = targetTile;
            unit.traversableTiles.Clear();
            gameController.currentlySelectedTile = null;

            yield return null; 
        }
        Debug.Log("Unit has finished moving along the path.");
        TacticsStateSwap();
    }
    private void CancelTileSelection() {
        Debug.Log("Tile selection canceled."); tileSelectionMode = TileSelectionMode.Movement;
      

        tacticsState = TacticsState.PlayerTurn; }

    private void TacticsStateSwap() //For now this will simply cycle through these two states, later on there will be a turn charge state, a turn swap state, enemy phase state, stuff like that.
    {
        if (tacticsState == TacticsState.PlayerTurn)
        {
            tacticsState = TacticsState.PlayerActionSelect;
            moveCamera();


        }
        else if (tacticsState == TacticsState.PlayerChoosingTile)
        {
            tacticsState = TacticsState.MovingTiles;
            tacticsCameraState = TacticsCameraState.FollowingMovement;
            
        } else if(tacticsState == TacticsState.MovingTiles)
        {
            tacticsState = TacticsState.ShiftingTurn;
           
            shiftTurnState();
        } else if (tacticsState == TacticsState.ShiftingTurn)
        {
            tacticsState = TacticsState.PlayerTurn;
            tacticsCameraState = TacticsCameraState.SelectedUnit;
            moveCamera();
            
        } 
       

    }
    private void shiftTurnState()
    {
        while(turnQueue.Count < 10)
        {
            foreach(Unit unit in gameController.unitsInPlay)
            {
                unit.turnCharge += unit.turnInitiative;
                if(unit.turnCharge >= 100)
                {
                    turnQueue.Enqueue(unit);
                    unit.turnCharge -= 100;
                }
            }
        }
        gameController.currentUnit = turnQueue.Dequeue();
        TacticsStateSwap();

    }
   public void moveCamera()
    {
        switch(tacticsCameraState)
        {
            case TacticsCameraState.SelectedUnit:
                gameController.playerCamera.transform.position = transform.InverseTransformPoint(gameController.currentUnit.transform.position + new Vector3(0, 15, -15));
                gameController.playerCamera.transform.LookAt(gameController.currentUnit.currentTile.tilePosition);
                break;
            case TacticsCameraState.SelectedTile:
                if (gameController.currentlySelectedTile != null)
                {
                    gameController.playerCamera.transform.position = transform.InverseTransformPoint(gameController.currentlySelectedTile.tilePosition + new Vector3(0, 15, -15));
                    gameController.playerCamera.transform.LookAt(gameController.currentlySelectedTile.tilePosition);
                }
                    break;
            default:
                break;
        }
    }


    private Tile FindClosestReachableTile(Unit unit, Tile startTile, Tile targetTile)
    {
        HashSet<Tile> reachableTiles = GetReachableTiles(unit);

        float closestDistance = float.MaxValue;
        Tile closestTile = null;

        foreach(Tile tile in reachableTiles)
        {
            if(tile.unitOnTile != null && tile.unitOnTile != unit)
            {
                continue;
            }
            float distance = Vector3.Distance(tile.tilePosition, targetTile.tilePosition);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }
        return closestTile;
   }
    
    private Tile FindTileClosestToEnemyUnit(Unit unitSearching)
    {
        HashSet<Tile> reachableTiles = GetReachableTiles(unitSearching);

        Unit closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach(Unit unit in gameController.unitsInPlay)
        {
            
          switch(unitSearching.faction)
            {
                case UnitFaction.R7:
                    if(unit.faction == UnitFaction.Wails || unit.faction == UnitFaction.SPC || unit.faction == UnitFaction.Space_Pirates)
                    {
                        float distance = Vector3.Distance(unitSearching.currentTile.tilePosition, unit.currentTile.tilePosition);
                        if(distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemy = unit;
                        }
                    }
                    break;
                case UnitFaction.Wails:
                    if(unit.faction == UnitFaction.R7)
                    {
                        float distance = Vector3.Distance(unitSearching.currentTile.tilePosition, unit.currentTile.tilePosition);
                        if(distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemy = unit;
                        }
                    }
                    break;
                case UnitFaction.SPC:
                    if(unit.faction == UnitFaction.R7 || unit.faction == UnitFaction.Space_Pirates)
                    {
                        float distance = Vector3.Distance(unitSearching.currentTile.tilePosition, unit.currentTile.tilePosition);
                        if(distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemy = unit;
                        }
                    }
                    break;

                case UnitFaction.Space_Pirates:
                    if (unit.faction == UnitFaction.Wails || unit.faction == UnitFaction.SPC)
                    {
                        float distance = Vector3.Distance(unitSearching.currentTile.tilePosition, unit.currentTile.tilePosition);
                            if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemy = unit;
                        }
                    }
                    break;
                        
                    
            }
        }
        if (closestEnemy == null)
        {
            return null;
        }

        Tile targetTile = null;
        float minDistanceToTile = float.MaxValue;

        foreach(Tile tile in reachableTiles)
        {
            float distanceToEnemy = Vector3.Distance(tile.tilePosition, closestEnemy.currentTile.tilePosition);

            if(tile.unitOnTile != null && tile.unitOnTile != unitSearching)
            {
                continue;
            }

            if(distanceToEnemy < minDistanceToTile)
            {
                minDistanceToTile = distanceToEnemy;
                targetTile = tile;
            }
        }

        return targetTile;
    }

    public bool TryUseSkill(Unit unit, Skill skill, Tile targetTile, DirectionFacing direction)
    {
        if (unit.currentAP < skill.apCost)
        {
            return false;

        }

        if(skill.prerequisites != null && skill.prerequisites.Count > 0)
        {
            bool hasPrereq = skill.prerequisites.Any(prereq => unit.skillsUsedThisTurn.Contains(prereq));
            if(!hasPrereq)
            {
                return false;
            }
        }

     

        List<Unit> affectedUnits = GetUnitsInArea(targetTile, skill.areaType, skill.areaSize, direction);

        skill.ApplyEffect(unit, affectedUnits);

        unit.currentAP -= skill.apCost;
        unit.skillsUsedThisTurn.Add(skill.skillName);

        return true;
        
            
        
    }

    public List<Tile> GetValidTargetTiles(Unit unit, Skill skill, DirectionFacing direction)
    {
        List<Tile> targetTiles = new List<Tile>();
        targetTiles.Add(unit.currentTile);
        int dx = 0;
        int dz = 0;

        switch(direction)
        {
            case DirectionFacing.North:
                dz = 1; break;
            case DirectionFacing.South:
                dz = -1; break;
            case DirectionFacing.East:
                dx = 1; break;
            case DirectionFacing.West:
                dx = -1; break;


        }

        for (int i = 1; i <= skill.targetRange; i++)
        {
            int x = unit.currentTile.tileCoords.Item1 + dx * i;
            int z = unit.currentTile.tileCoords.Item3 + dz * i;
            Tile tile = gameController.findExistingTile(x, z);
            if(tile != null)
            {
                targetTiles.Add(tile);
            }
        }
        return targetTiles;
    }
    public List<Tile> GetTilesInArea(Tile targetTile, SkillAreaType areaType, int areaSize, DirectionFacing direction )
    {
        List<Tile> affectedTiles = new List<Tile>();

        switch (areaType)
        {
            case SkillAreaType.SingleTile:
                if (targetTile != null)
                    affectedTiles.Add(targetTile);
                break;
            case SkillAreaType.Line:
                affectedTiles.AddRange(GetTilesInLine(targetTile, areaSize, direction));
                break;
            case SkillAreaType.Circle:
                affectedTiles.AddRange(GetTilesInCircle(targetTile, areaSize));
                break;
            case SkillAreaType.Cone:
                affectedTiles.AddRange(GetTilesInCone(targetTile, areaSize, direction));
                break;
        }
        return affectedTiles;
    }

    private List<Tile> GetTilesInLine(Tile startTile, int length, DirectionFacing direction)
    {
        List<Tile> tiles = new List<Tile>();
        int dx = 0, dz = 0;
        switch (direction)
        {
            case DirectionFacing.North: dz = 1; break;
            case DirectionFacing.South: dz = -1; break;
            case DirectionFacing.East: dx = 1; break;
            case DirectionFacing.West: dx = -1; break;
        }
        for (int i = 0; i <= length; i++)
        {
            Tile tile = gameController.findExistingTile(
                startTile.tileCoords.Item1 + i * dx,
                startTile.tileCoords.Item3 + i * dz);
            if (tile != null)
                tiles.Add(tile);
        }
        return tiles;
    }

    private List<Tile> GetTilesInCircle(Tile centerTile, int radius)
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Tile tile in gameController.gridMap.Values)
        {
            int dx = Mathf.Abs(tile.tileCoords.Item1 - centerTile.tileCoords.Item1);
            int dz = Mathf.Abs(tile.tileCoords.Item3 - centerTile.tileCoords.Item3);
            if (dx + dz <= radius)
                tiles.Add(tile);
        }
        return tiles;
    }

    private List<Tile> GetTilesInCone(Tile originTile, int length, DirectionFacing direction)
    {
        List<Tile> tiles = new List<Tile>();
        int dx = 0, dz = 0;
        switch (direction)
        {
            case DirectionFacing.North: dz = 1; break;
            case DirectionFacing.South: dz = -1; break;
            case DirectionFacing.East: dx = 1; break;
            case DirectionFacing.West: dx = -1; break;
        }

    
        for (int i = 1; i <= length; i++)
        {
            
            for (int j = -i + 1; j <= i - 1; j++)
            {
                int x, z;
                if (dx != 0) 
                {
                    x = originTile.tileCoords.Item1 + dx * i;
                    z = originTile.tileCoords.Item3 + j;
                }
                else 
                {
                    x = originTile.tileCoords.Item1 + j;
                    z = originTile.tileCoords.Item3 + dz * i;
                }
                Tile tile = gameController.findExistingTile(x, z);
                if (tile != null)
                    tiles.Add(tile);
            }
        }
        
        int frontx = originTile.tileCoords.Item1 + dx;
        int frontz = originTile.tileCoords.Item3 + dz;
        Tile frontTile = gameController.findExistingTile(frontx, frontz);
        if (frontTile != null)
            tiles.Add(frontTile);

        return tiles;
    }

    public List<Unit> GetUnitsInArea(Tile targetTile, SkillAreaType areaType, int areaSize, DirectionFacing direction)
    {
        List<Unit> affectedUnits = new List<Unit>();

        switch(areaType)
        {
            case SkillAreaType.SingleTile:
                if(targetTile.unitOnTile != null)
                {
                    affectedUnits.Add(targetTile.unitOnTile);
                }
                break;
            case SkillAreaType.Line:
                affectedUnits.AddRange(GetUnitsInLine(targetTile, areaSize, direction));
                break;
            case SkillAreaType.Circle:
                affectedUnits.AddRange(GetUnitsInCircle(targetTile, areaSize));
                break;
            case SkillAreaType.Cone:
                affectedUnits.AddRange(GetUnitsInCone(targetTile, areaSize, direction));
                break;
        }
        return affectedUnits;
    }
    
    private List<Unit> GetUnitsInLine(Tile startTile, int length, DirectionFacing direction)
    {
        List<Unit> units = new List<Unit>();
        int dx = 0;
        int dz = 0;
        switch (direction)
        {
            case DirectionFacing.North:
                dz = 1; break;
            case DirectionFacing.South:
                dz = -1; break;
            case DirectionFacing.East:
                dx = 1; break;
            case DirectionFacing.West:
                dx = -1; break;
        }
        for (int i = 0; i <= length; i++)
        {
            Tile tile = gameController.findExistingTile(
                startTile.tileCoords.Item1 + i * dx,
                startTile.tileCoords.Item3 + i * dz);
            if(tile != null && tile.unitOnTile != null)
            {
                units.Add(tile.unitOnTile);
            }
        }
        return units;
    }

    private List<Unit> GetUnitsInCircle(Tile centerTile, int radius)
    {
        List<Unit> units = new List<Unit>();
        foreach(Tile tile in gameController.gridMap.Values)
        {
            if(Vector3.Distance(tile.tilePosition, centerTile.tilePosition) <= radius)
            {
                if(tile.unitOnTile != null)
                {
                    units.Add(tile.unitOnTile);
                }
            }
        }
        return units;
    }
    
    private List<Unit> GetUnitsInCone(Tile originTile, int length, DirectionFacing direction)
    {
        List<Unit> units = new List<Unit>();
        int dx = 0;
        int dz = 0;

        switch (direction)
        {
            case DirectionFacing.North:
                dz = 1; break;
            case DirectionFacing.South:
                dz = -1; break;
            case DirectionFacing.East:
                dx = 1;
                break;
            case DirectionFacing.West:
                dx = -1; break;
        }
        for (int i = 1; i <= length; i++)
        {
            for (int j = -i + 1; j <= i - 1; j++)
            {
                int x = originTile.tileCoords.Item1 + dx * i + (dz == 0 ? j : 0);
                int z = originTile.tileCoords.Item3 + dz * i + (dx == 0 ? j : 0);
                Tile tile = gameController.findExistingTile(x, z);
                if (tile != null && tile.unitOnTile != null)
                    units.Add(tile.unitOnTile);
            }
        }
        int frontx = originTile.tileCoords.Item1 + dx;
        int frontz = originTile.tileCoords.Item3 + dz;
        Tile frontTile = gameController.findExistingTile(frontx, frontz);
        if (frontTile != null && frontTile.unitOnTile != null)
        {
            units.Add(frontTile.unitOnTile);
        }
        return units;
    }
}
