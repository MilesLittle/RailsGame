using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SkillAreaType {
    SingleTile,
    Line,
    Circle,
    Cone,

}



[CreateAssetMenu(fileName = "NewSkill", menuName = "Tactics/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public int apCost;
    [Range(1, 5)]
    public int tier;
    public List<string> prerequisites;
    public SkillAreaType areaType;
    public int areaSize;
    [TextArea]
    public string description;

    public void ApplyEffect(Unit user, List<Unit> targets)
    {
        // Implement skill effect logic here

    }
}
