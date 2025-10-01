using System.Collections.Generic;
using UnityEngine;

public class UnitBeastiary : MonoBehaviour
{
    [System.Serializable]
    public class UnitEntry
    {
        public string unitName;
        public UnitInfo unitInfo;
    }

    public List<UnitEntry> unitEntries = new List<UnitEntry>();

    [HideInInspector]
    public Dictionary<string, UnitInfo> Beastiary = new Dictionary<string, UnitInfo>();

    private void Awake()
    {
        foreach (var entry in unitEntries)
        {
            if (!Beastiary.ContainsKey(entry.unitName))
            {
                Beastiary.Add(entry.unitName, entry.unitInfo);
            }
        }
    }
}