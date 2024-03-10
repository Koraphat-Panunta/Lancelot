using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set_Sequence : Sequence
{
    public Sequence Cur_Sequence;
    public Sequence_Line Set_sequence;
    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Collider_enable == true)
            {
                base.Collider_enable = false;
                Cur_Sequence.SetCur_Sequence(Set_sequence);
                Cur_Sequence.Enter_Chapter = true;
            }

        }
    }
}
