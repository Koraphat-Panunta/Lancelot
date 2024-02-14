using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Virsual_input : MonoBehaviour
{
    public enum Button_Left_State { Up, Down };
    public enum Button_Right_State { Down, Up };
    public Button_Left_State button_Left_State = Button_Left_State.Up;
    public Button_Right_State button_Right_State = Button_Right_State.Up;

    public Touch mytouch;
    public enum Button_Attack_State { Up,Down};
    public enum Button_Defend_State {  Up,Down};
    public Button_Attack_State Cur_Button_Attack = Button_Attack_State.Up;
    public Button_Defend_State Cur_Button_Defend = Button_Defend_State.Up;


    public void Set_Button_Left_Up() 
    {
        button_Left_State = Button_Left_State.Up;
    }
    public void Set_Button_Right_Up()
    {
        button_Right_State = Button_Right_State.Up;
    }
    public void Set_Button_Left_Down()
    {
        button_Left_State = Button_Left_State.Down;
    }
    public void Set_Button_Right_Down()
    {
        button_Right_State = Button_Right_State.Down;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cur_Button_Defend = Button_Defend_State.Up;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touches.Length > 0)
        {
            if (Input.touches.Length == 1)
            {
                mytouch = Input.touches[0];
            }
            else if (Input.touches.Length == 2)
            {
                mytouch = Input.touches[1];
            }
            if ((mytouch.phase == TouchPhase.Began || mytouch.phase == TouchPhase.Stationary) && mytouch.position.x >= (float)Screen.width / 2)
            {
                Cur_Button_Attack = Button_Attack_State.Down;
            }
            else
            {
                Cur_Button_Attack = Button_Attack_State.Up;
            }
            if ((mytouch.phase == TouchPhase.Began || mytouch.phase == TouchPhase.Stationary) && (button_Left_State == Button_Left_State.Up && button_Right_State == Button_Right_State.Up)
                && mytouch.position.x < (float)Screen.width / 2)
            {
                Cur_Button_Defend = Button_Defend_State.Down;
            }
            else
            {
                Cur_Button_Defend = Button_Defend_State.Up;
            }
        }

    }
}
