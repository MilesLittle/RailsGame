using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillTargetingSystem
{
    public static List<Vector2Int> GetAffectedTiles(Unit user, Skill skill, Vector2Int? targetTile = null)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        Vector2Int origin;
        if (skill.originatesFromUser || targetTile == null)
        {
            origin = GetFrontTile(new Vector2Int(user.currentTile.tileCoords.Item1,user.currentTile.tileCoords.Item3), user.directionFacing, skill.shapeOffSet);
        }
        else
        {
            origin = targetTile.Value;

        }

        switch (skill.shapeType)
        {
            case SkillShapeType.Single:
                tiles.Add(origin);
                break;
            case SkillShapeType.HorizontalLine:
                tiles.AddRange(GetHorizontalLine(origin, user.directionFacing, skill.shapeLength));
                break;
            case SkillShapeType.VerticalLine:
                tiles.AddRange(GetVerticalLine(origin, user.directionFacing, skill.shapeLength));
                break;
            case SkillShapeType.Circle:
                tiles.AddRange(GetCircle(origin, skill.shapeLength));
                break;
        }

        return tiles;

    }

    public static List<Vector2Int> GetTilesInRange(Vector2Int userPos, int range)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector2Int pos = new Vector2Int(userPos.x + x, userPos.y + y);
                if (Mathf.Abs(x) + Mathf.Abs(y) <= range)
                {
                    tiles.Add(pos);
                }
            }
            
        }
        return tiles;
    }
    private static Vector2Int GetFrontTile(Vector2Int position, Unit.DirectionFacing facing, int offset)
    {
        return facing switch
        {
            Unit.DirectionFacing.North => position + new Vector2Int(0, offset),
            Unit.DirectionFacing.South => position + new Vector2Int(0, -offset),
            Unit.DirectionFacing.East => position + new Vector2Int(offset, 0),
            Unit.DirectionFacing.West => position + new Vector2Int(-offset, 0),
            _ => position
        };
    }

    private static List<Vector2Int> GetHorizontalLine(Vector2Int origin, Unit.DirectionFacing facing, int length)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        for (int dx = -length; dx <= length; dx++)
            result.Add(origin + new Vector2Int(dx, 0));
        return result;
    }

    private static List<Vector2Int> GetVerticalLine(Vector2Int origin, Unit.DirectionFacing facing, int length)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        for (int dy = -length; dy <= length; dy++)
            result.Add(origin + new Vector2Int(0, dy));
        return result;
    }

    private static List<Vector2Int> GetCircle(Vector2Int origin, int radius)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                    result.Add(origin + new Vector2Int(x, y));
            }
        }

        return result;
    }
}
