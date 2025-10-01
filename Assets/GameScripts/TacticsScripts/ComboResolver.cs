using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboResolver
{

    public Queue<Skill> ResolveCombos(Queue<Skill> inputQueue)
    {
        List<Skill> workingList = new List<Skill>(inputQueue);
        bool comboFound = true;

        while (comboFound)
        {
            comboFound = false;


            for (int i = 0; i < workingList.Count; i++)
            {
                Skill skill = workingList[i];

                foreach (var combo in skill.combos)
                {
                    if (combo.pattern.Count == 0 || combo.result == null)
                        continue;


                    if (MatchesPattern(workingList, i, combo.pattern))
                    {

                        workingList.RemoveRange(i, combo.pattern.Count);
                        workingList.Insert(i, combo.result);

                        comboFound = true;
                        break;
                    }
                }

                if (comboFound)
                    break;
            }
        }

        return new Queue<Skill>(workingList);
    }

    private static bool MatchesPattern(List<Skill> list, int startIndex, List<string> pattern)
    {
        if (startIndex + pattern.Count > list.Count)
            return false;

        for (int j = 0; j < pattern.Count; j++)
        {
            if (list[startIndex + j].skillName != pattern[j])
                return false;
        }
        return true;
    }
}


 
