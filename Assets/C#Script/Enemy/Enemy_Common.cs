using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Common : Enemy
{
    public override void Start()
    {
        base.Start();
        Under_ATK_Range_Reteating = 4f;
        Under_Pressure_Approuching = 63;
        Over_Pressure_Reteating = 70;
        Under_Pressure_ATK = 70f;
    }
    protected override void Update()
    {
        base.Update();
    }
    
}
