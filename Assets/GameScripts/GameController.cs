using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using static GridController;
public enum GamePlayState
{
    Dungeon,
    Tactics,
    Test,
    Dialogue
}

public class PriorityQueue<T>
{
    private readonly SortedDictionary<float, Queue<T>> elements = new();
    public int Count { get; private set; } = 0;

    public void Enqueue(T item, float priority)
    {
        if (!elements.ContainsKey(priority))
        {
            elements[priority] = new Queue<T>();
        }

        elements[priority].Enqueue(item);
        Count++;
    }

    public T Dequeue()
    {
        var firstPair = elements.First();
        var item = firstPair.Value.Dequeue();
        if(firstPair.Value.Count == 0)
        {
            elements.Remove(firstPair.Key);
        }
        Count--;
        return item;
    }

    public bool Contains(T item)
    {
        return elements.Values.Any(q => q.Contains(item));
    }
}
public class GameController : MonoBehaviour
{
    public Unit currentUnit;
    public Camera playerCamera;
    public UnitBeastiary unitBeastiary;
    public GridController gridController;
    public DungeonController dungeonController;
    public TacticsController tacticsController;
    public Dictionary<(int, int), Tile> gridMap;
    public Tile currentlySelectedTile;
    public List<Unit> unitsInPlay =  new List<Unit>();
    public DialogueManager dialogueManager;
    public GamePlayState gameState;
    public GridVisualizor gridVisualizer;
    
    public GamePlayState previousState;
    public Dictionary<int, (int, int)> lvlIndexToStartTiles = new Dictionary<int, (int, int)>();
    private void Awake()
    {
        initializeGameController();
    }

    void initializeGameController()
    {
        dungeonController.enabled = false;
        tacticsController.enabled = false;
        gridController.enabled = false;
        
       
        gridController = FindObjectOfType<GridController>();
        dungeonController = FindObjectOfType<DungeonController>();
        tacticsController = FindObjectOfType<TacticsController>();
        gridMap = gridController.gridMap;

        FindAllUnits();
        
        lvlIndexToStartTiles.Add(0, (0, 0));
        
        

    }

    public void FindAllUnits()
    {
        unitsInPlay.Clear(); // Clear the list before adding new units
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unitsInPlay.Add(unit);
        }
    }

    private void Update()
    {
        if(gameState == GamePlayState.Test)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                shiftToDungeon();
                Debug.Log("Shifted to Dungeon");
            }
           
        } else if(gameState == GamePlayState.Dungeon)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                shiftToTactics(unitsInPlay);
                Debug.Log("Shifted to Tactics");
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                dialogueManager.StartDialogue(dialogueManager.allGameDialogue["Test"]);
            }
        }
       else if (gameState == GamePlayState.Tactics)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                shiftToDungeon();
                Debug.Log("Shifted to Dungeon");
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                dialogueManager.StartDialogue(dialogueManager.allGameDialogue["Test"]);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            gridVisualizer.ToggleGridLines();
        }
    }

  

    public void shiftToDungeon()
    {
        gameState = GamePlayState.Dungeon;
        currentUnit.currentTile = gridMap[(0, 0)];
        currentUnit.directionFacing = Unit.DirectionFacing.North;
        gridController.enabled = true;
        dungeonController.enabled = true;
        dungeonController.initializeDungeon();
       

    }
    public void PlaceUnit(Unit unit, Tile tile)
    {
        if (unit == null || tile == null)
        {
            Debug.LogError("Unit or Tile is null");
            return;
        }
        unit.currentTile = tile;
        unit.transform.position = tile.tilePosition;
        
    }
    public void shiftToTactics(List<Unit> units)
    {
        gameState = GamePlayState.Tactics;
        int i = 0;
        foreach(Unit unit in units)
        {
            unit.currentTile = gridMap[(i,i)];
            i++;
            PlaceUnit(currentUnit, currentUnit.currentTile);
            currentUnit.directionFacing = Unit.DirectionFacing.North;
        }
           
            
        gridController.enabled = true;
        tacticsController.enabled = true;
        
        // whenever i shift to tactics, i will run a method that will initialize things like the initial turn order and go from there. for now we just set it to player turn and keep it pushing!
        tacticsController.tacticsState = TacticsState.PlayerTurn;
    }
    public List<Tile> FindAdjacentTiles(Tile myTile)
    {
        List<Tile> adjacentTiles = new List<Tile>();
        // Get the tile's coordinates
        (int x, float y, int z) = myTile.tileCoords;
        // Check the 4 possible adjacent tiles (up, down, left, right)
        foreach (var offset in new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
        {
            var newCoords = ((x + offset.Item1), y, (z + offset.Item2));
            if (gridMap.TryGetValue((newCoords.Item1, newCoords.Item3), out Tile adjacentTile))
            {
                adjacentTiles.Add(adjacentTile);
            }
        }
        return adjacentTiles;
    }

    public Tile findExistingTile(int x, int z) {
        if (gridMap.TryGetValue((x, z), out Tile existingTile))
        {
            return existingTile;
        }
        return null;
        }


    
}
