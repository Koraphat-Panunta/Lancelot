using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial_Sequence : MonoBehaviour
{
    [SerializeField] private  Virsual_input Virsual_Input;
    [SerializeField] private Main_Char Main_Char;
    public Set_Text text;
    public bool FristTime_Move = true;
    public bool FristTime_Block = true;
    public bool FristTime_Parry = true;
    public bool FristTime_ATK = true;
    public bool FristTime_Dash = true;
    public enum tutorial
    {
        Movement,
        Block,
        Parry,
        Attack,
        Dash,
        None
    }
    public tutorial Tutorial = tutorial.None;
    private void Start()
    {
     FristTime_Move = true;
     FristTime_Block = true;
     FristTime_Parry = true;
     FristTime_ATK = true;
     FristTime_Dash = true;
}
    private void Update()
    {
        Tutorial_Update();
    }
    public void Tutorial_Update() 
    {
        if(Time.timeScale == 0)
        {           
            if (Tutorial == tutorial.Movement && FristTime_Move == true)
            {
                text.show_text("Press A,D(Keyboard) or Move Analog(Mobile) \n *Move your Chatacter*");
                if (Main_Char.Cur_state == Main_Char.Char_state.Run)
                {
                    StartTime();
                    text.stop_show_text();
                    Tutorial = tutorial.Block;
                    FristTime_Move = false;
                }
            }
            else if(Tutorial == tutorial.Block && FristTime_Block == true) 
            {
                text.show_text("Press Space(Keyboard) or Touch Left side(Mobile) \n *To Defend the attack*");
                if (Main_Char.Cur_state == Main_Char.Char_state.Block)
                {
                    StartTime();                    
                    text.stop_show_text();
                    FristTime_Block= false;
                }
            }
            else if (Tutorial == tutorial.Parry && FristTime_Parry == true)
            {
                text.show_text("Press Defend Before The attack hit you\n *To parry the attack*");
                if (Main_Char.Cur_state == Main_Char.Char_state.Block|| Main_Char.Cur_state == Main_Char.Char_state.Parry )
                {
                    StartTime();
                    text.stop_show_text();
                    Tutorial = tutorial.Attack;
                    FristTime_Parry = false;
                }
            }
            else if (Tutorial == tutorial.Attack && FristTime_ATK == true)
            {
                text.show_text("Press J(Keyboard) or Tap Right side(Mobile)\n *To attack*");
                if (Main_Char.Cur_state == Main_Char.Char_state.Attack_I)
                {
                    StartTime();
                    text.stop_show_text();
                    FristTime_ATK = false;
                    Tutorial = tutorial.Dash;
                }
            }
            else if(Tutorial == tutorial.Dash && FristTime_Dash == true) 
            {
                text.show_text("Press K(Keyboard) or Swipe Right side(Mobile)\n *To Dash or Dogde*");
                if (Main_Char.Cur_state == Main_Char.Char_state.Dash|| Main_Char.Cur_state == Main_Char.Char_state.Dodge)
                {
                    StartTime();
                    text.stop_show_text();
                    Tutorial = tutorial.None;
                    FristTime_Dash = false;
                }
            }

        }
    }
    public void StopTime() 
    {
        Time.timeScale = 0f;       
        Tutorial_Update();
    }
    public void StartTime() 
    {
       
        Time.timeScale = 1;
    }
}
