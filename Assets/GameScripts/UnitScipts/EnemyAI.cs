using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum EnemyBehavior 
{
    MoveTowardsClosestEnemy,
        MoveTowardsClosestAlly,
        MoveAwayFromAllEnemies
}
public class EnemyAI : MonoBehaviour
{
    //temporary basic ai, just simple rule based behaviour. later on this will be replaced with a machine learning ai algorithm based of the player's playstyle

    private readonly TacticsController tacticsController;
    private readonly GameController gameController;
    public EnemyBehavior Behavior { get; set; }

    public EnemyAI(TacticsController tacticsController, EnemyBehavior initialBehavior = EnemyBehavior.MoveTowardsClosestEnemy)
    {
        this.tacticsController = tacticsController;
        this.gameController = tacticsController.gameController;
        Behavior = initialBehavior;
    }


    public void HandleTurn(Unit unit)
    {
        if(unit == null)
        {
            return;
        }

        Tile chosenTile = ChooseTileForBehavior(unit);

        if(chosenTile == null || chosenTile == unit.currentTile)
        {
            return; 
            //temporary decision, later could be use skills/attacks
        }

        List<Tile> path = ComputePath(unit, unit.currentTile, chosenTile);
        if(path == null || path.Count == 0)
        {
            return;
        }

        tacticsController.StartCoroutine(tacticsController.moveAlongPath(unit, path));
    }
    
    private Tile ChooseTileForBehavior(Unit unit)
    {
        switch (Behavior)
        {
            case EnemyBehavior.MoveTowardsClosestEnemy:
                return ChooseTileTowardsClosestEnemy(unit);
            case EnemyBehavior.MoveTowardsClosestAlly:
                return ChooseTileTowardsClosestAlly(unit);
            case EnemyBehavior.MoveAwayFromAllEnemies:
                return ChooseTileAwayFromEnemies(unit);
            default:
                return null;

        }
    }

    private Tile ChooseTileTowardsClosestEnemy(Unit unit)
    {
        Unit closestEnemy = FindClosestEnemyUnit(unit);
        if(closestEnemy == null)
        {
            return null;
        }
        return ChooseReachableTileClosestTo(unit, closestEnemy.currentTile);
    }

    private Tile ChooseTileTowardsClosestAlly(Unit unit)
    {
        Unit closestAlly = FindClosestAllyUnit(unit);
        if(closestAlly == null)
        {
            return null; 
        }
        return ChooseReachableTileClosestTo(unit, closestAlly.currentTile);
    }

    private Tile ChooseTileAwayFromEnemies(Unit unit)
    {
        List<Tile> reachable = tacticsController.GetReachableTiles(unit);
        var enemies = gameController.unitsInPlay.Where(u => IsEnemyOf(unit, u)).ToList();
        if(enemies.Count == 0)
        {
            return unit.currentTile;
        }
        Tile best = unit.currentTile;
        float bestScore = float.MinValue; 

        foreach(Tile tile in reachable)
        {
            if(tile.unitOnTile != null && tile.unitOnTile != unit)
            {
                continue;
            }
            float score = 0f;
            foreach(var e in enemies)
            {
                score += Vector3.Distance(tile.tilePosition, e.currentTile.tilePosition);
            }
            if(score > bestScore)
            {
                bestScore = score;
                best = tile;
            }
        }
        return best;

    }


}
