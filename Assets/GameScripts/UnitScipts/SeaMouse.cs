using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMouse : Unit
{
  public override void InitializeUnit()
    {
        unitHead = FindWithTagInChildrenOrSelf(gameObject, "UnitHead");
        unitName = "Sea Mouse";
        CalculateStats();
        Debug.Log("Hello");

        
    }

    private void Start()
    {
        InitializeUnit();
    }



}
