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
public class TacticsController : MonoBehaviour
{

    public TacticsCameraState tacticsCameraState;
    public TacticsState tacticsState;
    public GameController gameController;
    private int selectedAction = 0;
    private string[] actionOptions = { "Move", "Skill", "Item", "End Turn" };
    public Queue<Unit> turnQueue = new Queue<Unit>();
    [SerializeField]
    public ActionChain actionChain;
    [SerializeField]
    public ComboResolver comboResolver;

    private void Awake()
    {
        if (comboResolver == null)
        {
            comboResolver = new ComboResolver();

        }
    }
    void OnGUI()
    {
       if(gameController.gameState == GamePlayState.Tactics)
        {
            if(tacticsState == TacticsState.PlayerActionSelect)
            {
                GUI.Label(new Rect(10, 10, 200, 20), "Choose Action:");
                for (int i = 0; i < actionOptions.Length; i++)
                {
                    GUI.color = (i == selectedAction) ? Color.yellow : Color.white;
                    if (GUI.Button(new Rect(10,40 + i * 30, 100, 25), actionOptions[i]))
                    {
                        HandleActionSelect(actionOptions[i]);
                    }
                }
                GUI.color = Color.white;
            }

            if(tacticsState == TacticsState.PlayerSkillQueueing)
            {
                Unit unit = gameController.currentUnit;
                for(int i = 0; i < unit.unitStats.availableSkills.Count; i++)
                {
                    Skill skill = unit.unitStats.availableSkills[i];
                    if(GUI.Button(new Rect(10,250 + i * 40, 150, 30),
                            $"{skill.skillName} ({skill.apCost} AP)") && skill.apCost <= unit.currentAP)
                    {
                        unit.skillQueue.Enqueue(skill);
                        Debug.Log(unit.skillQueue.Count);
                        unit.currentAP -= skill.apCost;
                    }
                }
                if(GUI.Button(new Rect(200, 440, 150, 30), "Execute Skills"))
                {
                    ExecuteSkillQueue(unit);
                    tacticsState = TacticsState.ShiftingTurn;
                    shiftTurnState();
                }
            }
           
           

            var queueArray = turnQueue.ToArray();
            GUI.color = Color.black;
            for (int i = 0; i < queueArray.Length; i++)
            {
                GUI.Label(new Rect(400, 10 + i * 20, 200, 20), $"{i + 1}: {queueArray[i].unitName}");
            }
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
                gameController.currentUnit.ResetAp();
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
            if(tacticsCameraState == TacticsCameraState.FollowingMovement)
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

    private void ExecuteSkillQueue(Unit unit)
    {
        if(unit.skillQueue == null || unit.skillQueue.Count == 0)
        {
            return;
        }

        Queue<Skill> resolvedQueue = comboResolver.ResolveCombos(unit.skillQueue);

        foreach (Skill skill in resolvedQueue)
        {
            Debug.Log($"Executing Skill: {skill.skillName}");
            ApplySkillEffect(unit, skill);

        }

        unit.skillQueue.Clear();
        unit.currentAP = 0;


    }

    private void ApplySkillEffect(Unit caster, Skill skill)
    {

       //implement this later on
    }
    
}
