using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewUnitInfo", menuName = "Unit/Create New Unit")]
public class UnitInfo : ScriptableObject
{
    [Header("Base STats")]
    public int Strength;
    public int Intelligence;
    public int Constitution;
    public int Dexterity;
    public int Luck;
    public int Agility;
    public int Fortitude;
    [Header("Growth Rates (in percentage)")]
    public float strengthGrowthRate;
    public float intelligenceGrowthRate;
    public float constitutionGrowthRate;
    public float dexterityGrowthRate;
    public float luckGrowthRate;
    public float agilityGrowthRate;
    public float fortitudeGrowthRate;

   

    

    public List<Skill> availableSkills;
    


  
}



/* These stats will define the unit's functional combat stats
   * Health : (Strength + Constitution + Fortitude) * (Level / 3)
   * Attack : (Strength + Agility) * (Dexterity  / 2)
   * Defense : (Constitution + Agility) * (Strength / 2 )
   * Speed : (Dexterity * Agility) + (level)
   * Evasion : (Fortitude * Agility) + (Luck * 2)
   * Hit Chance : (Dexterity + Intelligence + Fortitude + Agility)/4 * (Luck / 4)
   * Resistance : (Intelligence * Fortitude) * (Constitution / 2)
   * Magic Attack : (Intelligence + Dexterity) * (Fortitude / 2)
   * 
   * 
   * Units will have growth rates that determine whether or not they gain a stat each time they level up. 
   * For example, a unit might have a 60% strength growth rate, meaning that upon leveling up, they will have a 60% chance of gaining a point in strength.
   * For non player units however, they will have some base stats, and depending on the growth rate given, will simply gain an x% stat boost multiplied by their level.
   * For example, an enemy unit with a base strength stat of 3 and a strength growth rate of 60% will, at level 1 have a strength stat of 3 + (1 - 1) * .6 = 3. At level 2 however, they will have 3 + (2- 1) * .6 - 3.6. At level 100, they will have 3 + (100 - 1) * .6 = 60. 6. In other words, a 60% bonus applied 99 times. 
  */