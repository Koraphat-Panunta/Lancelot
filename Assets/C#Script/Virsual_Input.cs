using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Virsual_Input : MonoBehaviour
{
    
    public enum Button_state 
    {
        Down, Up
    }
    public Button_state Cur_Button_state = Button_state.Up;
   public void Set_Button_Up() 
    {
        Cur_Button_state = Button_state.Up;
    }
   public void Set_Button_Down()
    {
        Cur_Button_state = Button_state.Down;
    }



}
