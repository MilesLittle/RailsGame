using System.Collections.Generic;
using UnityEngine;

public static class SkillExecutor
{
    public static void ExecuteSkill(Unit user, Skill skill, Vector2Int? chosenTile = null)
    {
        if (!user.CanUseSkill(skill))
        {
            Debug.Log($"{user.unitName} doesnÅft have enough SP to use {skill.skillName}!");
            return;
        }

        user.UseSP(skill.spCost);

        List<Vector2Int> affectedTiles = SkillTargetingSystem.GetAffectedTiles(user, skill, chosenTile);

        Debug.Log($"{user.unitName} used {skill.skillName}:");

        foreach (Vector2Int tile in affectedTiles)
        {
            Debug.Log($" - affects tile {tile}");
            // TODO: Check for units on these tiles and apply effects
        }
    }
}
