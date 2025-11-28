using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
public enum UnitMovementType
{
    Ground,
    Air,
    Teleport,
    Cavalry
}

public enum UnitFaction
{
    R7,
    Wails,
    Space_Pirates,
    SPC,
}

public enum DirectionFacing
{
    North,
    South,
    East,
    West,
};


public class Unit : MonoBehaviour
{
    public string unitName;
    public GameObject unitHead;
    public UnitMovementType moveType;
    public int movementRange;
    public Tile currentTile;
    public int unitLevel;
    public bool hasAttacked;

    public int turnInitiative;
    public int turnCharge;
    public DirectionFacing directionFacing;
    public HashSet<Tile> traversableTiles = new HashSet<Tile>();
   public  UnitFaction faction;
    public bool playerControlled;
    public UnitInfo unitStats;
    public GameController gameController;
    public int Atk;
    public int Def;
    public int Spd;
    public int Eva;
    public int Hit;
    public int Res;
    public int MAtk;
    public int MaxHp;
    public int Hp;
    public int maxAP;
    public int currentAP;
    public HashSet<string> skillsUsedThisTurn = new HashSet<string>();
    public List<Skill> knownSkills = new List<Skill>();
   

 
   
    
   
    
   

    public GameObject FindWithTagInChildrenOrSelf(GameObject root, string tag)
    {
        if(root.CompareTag(tag))
        {
            return root;

        }

        foreach(Transform child in root.GetComponentsInChildren<Transform>())
        {
            if(child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        return root;
    }


    public void CalculateStats()
    {
        if(unitStats == null)
        {
            if (gameController.unitBeastiary.Beastiary.ContainsKey(unitName))
            {
                unitStats = gameController.unitBeastiary.Beastiary[unitName];
                Debug.Log("Unit stats found for " + unitName);
            }
            else
            {
                return;
            }
            unitStats.Strength = Mathf.RoundToInt((unitStats.Strength + ((unitLevel - 1) * unitStats.strengthGrowthRate)));
            unitStats.Intelligence = Mathf.RoundToInt((unitStats.Intelligence + ((unitLevel - 1) * unitStats.intelligenceGrowthRate)));
            unitStats.Constitution = Mathf.RoundToInt((unitStats.Constitution + ((unitLevel - 1) * unitStats.constitutionGrowthRate)));
            unitStats.Dexterity = Mathf.RoundToInt((unitStats.Dexterity + ((unitLevel - 1) * unitStats.dexterityGrowthRate)));
            unitStats.Luck = Mathf.RoundToInt((unitStats.Luck + ((unitLevel - 1) * unitStats.luckGrowthRate)));
            unitStats.Agility = Mathf.RoundToInt((unitStats.Agility + ((unitLevel - 1) * unitStats.agilityGrowthRate)));
            unitStats.Fortitude = Mathf.RoundToInt((unitStats.Fortitude + ((unitLevel - 1) * unitStats.fortitudeGrowthRate)));
            Atk = Mathf.RoundToInt((unitStats.Strength + unitStats.Agility) * (unitStats.Dexterity / 2));
            Def = Mathf.RoundToInt((unitStats.Constitution + unitStats.Agility) * (unitStats.Strength / 2));
            Spd = Mathf.RoundToInt((unitStats.Dexterity * unitStats.Agility) + (unitLevel));
            Eva = (unitStats.Fortitude * unitStats.Agility) + (unitStats.Luck * 2);
            Hit = (unitStats.Dexterity + unitStats.Intelligence + unitStats.Fortitude + unitStats.Agility) / 4;
            Res = (unitStats.Intelligence * unitStats.Fortitude) * (unitStats.Constitution / 2);
            MAtk = (unitStats.Intelligence + unitStats.Dexterity) * (unitStats.Fortitude / 2);
            MaxHp = (unitStats.Strength + unitStats.Constitution + unitStats.Fortitude) * (unitLevel / 3);
            Debug.Log(Atk);

        }

    }

    public void StartTurn()
    {
        currentAP = maxAP;
        skillsUsedThisTurn.Clear();

    }

    public bool SpendAp(int amount)
    {
        if(currentAP < amount)
        {
            return false;
        }
        currentAP -= amount;
        return true;
        
            
        
    }

    public void RecordSkillUsage(string skillName)
    {
        skillsUsedThisTurn.Add(skillName);
    }

    public List<Skill> GetAvailableSkills()
    {
        List<Skill> available = new List<Skill>();
        foreach(Skill skill in knownSkills)
        {
         if(currentAP >= skill.apCost && 
                (skill.prerequisites == null || skill.prerequisites.Count == 0||
                skill.prerequisites.Any(prereq => skillsUsedThisTurn.Contains(prereq))))
            {
                available.Add(skill);
            }
        }
        return available;
    }

    public virtual void InitializeUnit()
    {

    }
}




