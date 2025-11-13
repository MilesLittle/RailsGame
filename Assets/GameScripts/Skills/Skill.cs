using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum SkillShapeType
{
    Single,
    HorizontalLine,
    VerticalLine,
    Circle,
    Cone
}
[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Create New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    [TextArea] public string description;

    public int spCost = 1;
    public int basePower = 10;
    public float powerMultiplier = 1.0f;

    public SkillShapeType shapeType;
    public int range; //how far from the unit this can target
    public int shapeLength = 0; //how big the shape extends
    public int shapeOffSet = 1; //Distance in front of unit to start the shape

    public bool isSupportSkill = false; //determines if this targets allies or enemies
    public bool canTargetSelf = false;
    public bool originatesFromUser = true;

    
    

}
