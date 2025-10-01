using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Create New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public int apCost;
    public Sprite icon;

    [TextArea]
    public string description;

    public enum EffectShape
    {
        Single, Line, Cone, Area
    }
    public EffectShape effectShape;

    public int range;
    public int size;

    public int baseDamage;

   [System.Serializable] 
   public class ComboDefinition
    {
        public List<string> pattern;
        public Skill result;
    }

    public List<ComboDefinition> combos = new List<ComboDefinition>();
    
}
