using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Virsual_input : MonoBehaviour
{
   

    public Touch mytouch;
    public enum Button_Attack_State { Up,Down};
    public enum Button_Defend_State {  Up,Down};
    public Button_Attack_State Cur_Button_Attack = Button_Attack_State.Up;
    public Button_Defend_State Cur_Button_Defend = Button_Defend_State.Up;
    public enum Swipe_Gesture 
    {
        None,
        Left,
        Right,
    }
    public Swipe_Gesture Swipe = Swipe_Gesture.None;
    public float Horizontal;

    public FixedJoystick joystick;
    public Vector2 Begin_point;
    public Vector2 End_point;
    public float Duration;

    // Start is called before the first frame update
    void Start()
    {
        Cur_Button_Defend = Button_Defend_State.Up;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.InputSystem.Gamepad.current != null)
        {
            joystick.gameObject.SetActive(false);
        }
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
            if(mytouch.phase == TouchPhase.Began && mytouch.position.x >= (float)Screen.width / 2) 
            {
                Begin_point = mytouch.position;
            }
            if((mytouch.phase == TouchPhase.Stationary|| mytouch.phase == TouchPhase.Moved) && mytouch.position.x >= (float)Screen.width / 2) 
            {
                Duration += Time.deltaTime*(1/Time.timeScale);
            }
            
            if ((mytouch.phase == TouchPhase.Ended) && mytouch.position.x >= (float)Screen.width / 2)
            {                                
                End_point = mytouch.position;
                //Dash
                if (Duration <= 0.5f&&(Vector2.Distance(Begin_point,End_point)>12f))
                {
                    if (End_point.x > Begin_point.x) 
                    {
                        Swipe = Swipe_Gesture.Right;
                    }
                    else if(End_point.x < Begin_point.x) 
                    {
                        Swipe = Swipe_Gesture.Left;
                    }
                }
                //Attack
                else if (mytouch.position.x >= (float)Screen.width / 2) 
                {
                    Cur_Button_Attack = Button_Attack_State.Down;
                }
                Duration = 0;
            }
            else
            {
                Cur_Button_Attack = Button_Attack_State.Up;
                Swipe = Swipe_Gesture.None;
            }
            //Block
            if ((mytouch.phase == TouchPhase.Began || mytouch.phase == TouchPhase.Stationary|| mytouch.phase == TouchPhase.Moved ) && (joystick.Horizontal== 0&&joystick.Vertical==0) 
                && mytouch.position.x < (float)Screen.width / 2)
            {
                Cur_Button_Defend = Button_Defend_State.Down;
            }
            else
            {
                Cur_Button_Defend = Button_Defend_State.Up;
            }
            
        }
        else 
        {
            Cur_Button_Attack = Button_Attack_State.Up;
            Cur_Button_Defend = Button_Defend_State.Up;
            Swipe = Swipe_Gesture.None;
        }
        Joystick();

    }
    
    private void Joystick() 
    {
        Horizontal = joystick.Horizontal;
        

    }
    
}
