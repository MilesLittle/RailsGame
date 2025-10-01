using Sirenix.OdinValidator.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionChain 
{

    private List<Skill> chosenSkills = new List<Skill>();
    private int currentAP;
    private int maxAP;

    public ActionChain(int maxAP)
    {
        this.maxAP = maxAP;
        currentAP = maxAP;

    }

    public bool TryAddSkill(Skill skill)
    {
        if (skill.apCost <= currentAP)
        {
            chosenSkills.Add(skill);
            currentAP -= skill.apCost;
            return true;
        }
        else
        {
            return false;
        }
            
        }
    public List<Skill> GetChosenSkills() => chosenSkills;
    public int GetRemainingAp() => currentAP;

    public void Clear()
    {
        chosenSkills.Clear();
        currentAP = maxAP;
    }


    
    }

