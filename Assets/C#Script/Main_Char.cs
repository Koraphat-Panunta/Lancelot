using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEngine.EventSystems.EventTrigger;

public class Main_Char : Character
{
    public float HP = 100f;
    [SerializeField] private bool Change_Behavior_enable = true;
    private float Block_CoolDown = 0.15f;
    bool Block_Enable = true;
    public string Char_CurState;
    [SerializeField] private float Speed;
    private float Original_Speed;
    [SerializeField] public BoxCollider Attack_Box;
    [SerializeField] public CapsuleCollider Hitted_Box;
    public CapsuleCollider CharacterCollider;
    public float Animation_TimeLine;
    public float Animation_Length;
    public float DMG = 15;
    [SerializeField] Virsual_input virsual_Input;
    public PlayerInput mycontroller;

    public enum Char_state
    {
        Idle,
        Run,
        Attack_I,
        Attack_II,
        Parry,
        Block,
        Flinch,
        BlockReact,
        Dash,
        Dodge
    }
    public enum Char_Direction
    {
        Left,
        Right
    }
    public Char_state Cur_state;
    public Char_Direction Cur_Direction;
    private bool Dash_able = true;

    
    public void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        base.animator = gameObject.GetComponent<Animator>();
        base.CharRenderer = gameObject.GetComponent<SpriteRenderer>();
        Cur_state = Char_state.Idle;
        Cur_Direction = Char_Direction.Right;
        Attack_Box = gameObject.GetComponentInChildren<BoxCollider>();
        Hitted_Box.transform.localPosition = new Vector3(0, 0, 0);
        Virsual_input virsual_Input = GetComponent<Virsual_input>();
        Original_Speed = Speed;
        mycontroller.currentActionMap.FindAction("Attack").IsPressed();
       
    }

    protected override void Update()
    {
        Char_CurState = Cur_state.ToString();
        Animation_Length = animator.GetCurrentAnimatorStateInfo(0).length;
        Animation_TimeLine = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if(Gamepad.current == null) 
        {
            InputManager();
        }
        else 
        {
            InputManager_withcontroller();
        }
        Cooldown();
        Update_animation();
    }
    protected override void FixedUpdate()
    {
        Update_State_and_Dicrection();
        int parry_frame = 8;
        //Defend_state
        if (Cur_state == Char_state.Block)
        {
            Block_timimg += Time.deltaTime;
            if (Block_timimg < parry_frame * Time.deltaTime)
            {
                Cur_Defend_State = Defend_state.Pre_Defend;
            }
            else if (Block_timimg >= parry_frame * Time.deltaTime)
            {
                Cur_Defend_State = Defend_state.Guard;
            }
        }
        else if (Cur_state == Char_state.BlockReact)
        {
            Block_timimg += Time.deltaTime;
            Change_Behavior_enable = false;
            Cur_Defend_State = Defend_state.Blocking;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Cur_state = Char_state.Block;
                Change_Behavior_enable = true;
            }
        }
        else if (Cur_state == Char_state.Parry)
        {
            Block_timimg += Time.deltaTime;
            Change_Behavior_enable = false;
            Cur_Defend_State = Defend_state.Parry;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Cur_state = Char_state.Block;
                Change_Behavior_enable = true;
            }
        }
        else
        {
            Block_timimg = 0;
            Cur_Defend_State = Defend_state.None;
        }
        Update_Pos();
        base.FixedUpdate();

    }
    private void Update_animation()
    {
        animator.Play(Cur_state.ToString());
    }

    public enum Attack_state
    {
        None,
        Pre_Attack,
        Attacking,
        Post_Attack,
    }
    public enum Defend_state
    {
        None,
        Pre_Defend,
        Guard,
        Blocking,
        Parry,
    }
    public Defend_state Cur_Defend_State = Defend_state.None;
    public Attack_state Cur_Attack_State = Attack_state.None;
    [SerializeField] private float Dash_CoolDown = 2f;
    private void Cooldown()
    {
        if (Block_Enable == false)
        {
            Block_CoolDown -= Time.deltaTime * (1f / Time.timeScale);
            if (Block_CoolDown <= 0)
            {
                Block_CoolDown = 0.15f;
                Block_Enable = true;
            }
        }
        if (Dash_able == false)
        {
            Dash_CoolDown -= Time.deltaTime * (1f / Time.timeScale);
            if (Dash_CoolDown <= 0)
            {
                Dash_able = true;
                Dash_CoolDown = 1f;
            }
        }
    }

    private void Update_Pos()
    {
        if (Cur_state == Char_state.Run)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Speed * Time.deltaTime, 0, 0);
        }

    }
    public float Block_timimg = 0f;
    bool Attack_step_Enable = true;
    private void Update_State_and_Dicrection()
    {
        if (HP <= 0)
        {
            //Death 
        }
        //Attack_state       
        float Pre_ATK = 0.12f;
        float ATKing = 0.28f;
        float Post_ATK = 0.45f;
        if (Cur_state == Char_state.Attack_I || Cur_state == Char_state.Attack_II)
        {
            //PreAttack_I
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) < (float)(Pre_ATK))
            {
                Cur_Attack_State = Attack_state.Pre_Attack;
            }
            //Attack_I
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) >= (float)(Pre_ATK)
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) < (float)(ATKing))
            {
                Cur_Attack_State = Attack_state.Attacking;
                if (Attack_step_Enable == true && Cur_Attack_State == Attack_state.Attacking)
                {
                    Attack_step_Enable = false;
                    Push(4.5f);
                }
            }
            //PostAttack_I
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) >= (float)(ATKing)
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) < (float)(Post_ATK))
            {
                Cur_Attack_State = Attack_state.Post_Attack;
            }
            //Finish
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Cur_Attack_State = Attack_state.None;
                Change_Behavior_enable = true;

            }
        }
        else
        {
            Cur_Attack_State = Attack_state.None;
        }
        if (Cur_Attack_State != Attack_state.Attacking)
        {
            Attack_step_Enable = true;
        }
        //Flinch
        if (Cur_state == Char_state.Flinch)
        {
            Change_Behavior_enable = false;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                Change_Behavior_enable = true;
                Cur_state = Char_state.Idle;
                Update_animation();
            }
        }
        //Direction_Change
        if (Cur_Direction == Char_Direction.Right)
        {
            base.CharRenderer.GetComponent<SpriteRenderer>().flipX = false;
            Speed = Math.Abs(Speed);
            //Set_AttackBox
            Attack_Box.transform.localPosition = new Vector3(Math.Abs(Attack_Box.transform.localPosition.x),
                Attack_Box.transform.localPosition.y,
                Attack_Box.transform.localPosition.z);
            //Set_HittedBox
            Hitted_Box.transform.localPosition = new Vector3(-Math.Abs(Hitted_Box.transform.localPosition.x),
                Hitted_Box.transform.localPosition.y,
                Hitted_Box.transform.localPosition.z);
        }
        if (Cur_Direction == Char_Direction.Left)
        {
            base.CharRenderer.GetComponent<SpriteRenderer>().flipX = true;
            //Set_AttackBox
            Attack_Box.transform.localPosition = new Vector3(-Math.Abs(Attack_Box.transform.localPosition.x),
                Attack_Box.transform.localPosition.y,
                Attack_Box.transform.localPosition.z);
            //Set_HittedBox
            Hitted_Box.transform.localPosition = new Vector3(Math.Abs(Hitted_Box.transform.localPosition.x),
                Hitted_Box.transform.localPosition.y,
                Hitted_Box.transform.localPosition.z);
            if (Speed > 0)
            {
                Speed = -Speed;
            }
        }
        //Dodge&&Dash        
        int Dash_and_Dodge = 12;
        if (Cur_state == Char_state.Dash)
        {
            Change_Behavior_enable = false;
            Speed = Original_Speed;
            CharacterCollider.isTrigger = true;
            if (Dash_able == true)
            {
                Dash_able = false;
                if (Cur_Direction == Char_Direction.Left)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(-Dash_and_Dodge, 0, 0);
                }
                if (Cur_Direction == Char_Direction.Right)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Dash_and_Dodge, 0, 0);
                }
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Speed = Original_Speed;
                Change_Behavior_enable = true;

            }
        }
        else if (Cur_state == Char_state.Dodge)
        {
            Change_Behavior_enable = false;
            Speed = Original_Speed;
            CharacterCollider.isTrigger = true;
            if (Dash_able == true)
            {
                Dash_able = false;
                if (Cur_Direction == Char_Direction.Left)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3((Dash_and_Dodge * 0.7f), 0, 0);
                }
                if (Cur_Direction == Char_Direction.Right)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(-(Dash_and_Dodge * 0.7f), 0, 0);
                }
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Speed = Original_Speed;
                Change_Behavior_enable = true;

            }
        }
        if (CharacterCollider.isTrigger == true && Cur_state != Char_state.Dash && Cur_state != Char_state.Dodge)
        {

            CharacterCollider.isTrigger = false;

        }

    }
    private bool Combo_Continue = false;


    private void InputManager_withcontroller()
    {
         
        if ((Input.GetKeyDown(KeyCode.J) || (virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down)||Gamepad.current.buttonWest.isPressed) && Change_Behavior_enable == true)
        {
            if ((Input.GetKey(KeyCode.D) || virsual_Input.Horizontal > 0.3f || Gamepad.current.dpad.right.value > 0) && Change_Behavior_enable == true)
            {
                Cur_state = Char_state.Run;
                Cur_Direction = Char_Direction.Right;
                Update_animation();
                Dash_by_swipe();
               
            }
            else if ((Input.GetKey(KeyCode.A) || (virsual_Input.Horizontal < -0.3f) || Gamepad.current.dpad.left.value > 0) && Change_Behavior_enable == true)
            {
                Cur_state = Char_state.Run;
                Cur_Direction = Char_Direction.Left;
                Update_animation();
                Dash_by_swipe();
                
            }
            Cur_state = Char_state.Attack_I;
            Update_animation();
            Change_Behavior_enable = false;
        }
        else if ((Input.GetKey(KeyCode.D) || virsual_Input.Horizontal >0.3f||Gamepad.current.dpad.right.value >0) && Change_Behavior_enable == true)
        {
            Cur_state = Char_state.Run;
            Cur_Direction = Char_Direction.Right;
            Update_animation();
            Dash_by_swipe();
            if ((Input.GetKey(KeyCode.K)|| Gamepad.current.buttonEast.isPressed) && Dash_able == true)
            {
                Cur_Direction = Char_Direction.Right;
                Cur_state = Char_state.Dash;
                Update_animation();
                Change_Behavior_enable = false;
            }
            if ((Input.GetKey(KeyCode.Space) || (virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Down) || Gamepad.current.leftShoulder.value > 0) && Change_Behavior_enable == true && Block_Enable == true)
            {
                Cur_state = Char_state.Block;
                Update_animation();
                Change_Behavior_enable = false;
            }
            if ((Input.GetKeyDown(KeyCode.J) || (virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down)|| Gamepad.current.buttonWest.isPressed) && Change_Behavior_enable == true)
            {
                Cur_state = Char_state.Attack_I;
                Update_animation();
                Change_Behavior_enable = false;
            }
        }
        else if ((Input.GetKey(KeyCode.A) || (virsual_Input.Horizontal < -0.3f)|| Gamepad.current.dpad.left.value > 0) && Change_Behavior_enable == true)
        {
            Cur_state = Char_state.Run;
            Cur_Direction = Char_Direction.Left;
            Update_animation();
            Dash_by_swipe();
            if ((Input.GetKey(KeyCode.K)|| Gamepad.current.buttonEast.isPressed) && Dash_able == true)
            {
                Cur_Direction = Char_Direction.Left;
                Cur_state = Char_state.Dash;
                Update_animation();
                Change_Behavior_enable = false;
                
            }
            if ((Input.GetKey(KeyCode.Space) || (virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Down) || Gamepad.current.leftShoulder.value > 0) && Change_Behavior_enable == true && Block_Enable == true)
            {
                Cur_state = Char_state.Block;
                Update_animation();
                Change_Behavior_enable = false;
            }
            if ((Input.GetKeyDown(KeyCode.J) || (virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down)|| Gamepad.current.buttonWest.isPressed) && Change_Behavior_enable == true)
            {
                Cur_state = Char_state.Attack_I;
                Update_animation();
                Change_Behavior_enable = false;
            }
        }
        else if ((Input.GetKey(KeyCode.Space) || (virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Down) || Gamepad.current.leftShoulder.value > 0) && Change_Behavior_enable == true && Block_Enable == true)
        {
            Cur_state = Char_state.Block;
            Update_animation();
            Change_Behavior_enable = false;
        }
        else if((Input.GetKey(KeyCode.K)||virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None|| Gamepad.current.buttonEast.isPressed) && Change_Behavior_enable == true && Dash_able == true) 
        {
            if (virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None)
            {
                Dash_by_swipe();
            }
            else
            {
                if (Input.GetKey(KeyCode.A)|| Gamepad.current.dpad.left.value > 0)
                {
                    Cur_Direction = Char_Direction.Left;
                    Cur_state = Char_state.Dash;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
                else if (Input.GetKey(KeyCode.D)|| Gamepad.current.dpad.right.value > 0)
                {
                    Cur_Direction = Char_Direction.Right;
                    Cur_state = Char_state.Dash;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
                else
                {
                    Cur_state = Char_state.Dodge;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
            }
        }                   
        //Idle(NoInput)
        else if (Change_Behavior_enable == true )
        {
            Cur_state = Char_state.Idle;
            Update_animation();
        }
        ////Bypass_State
        if ((Cur_Attack_State == Attack_state.Pre_Attack || Cur_Attack_State == Attack_state.Post_Attack) && Block_Enable == true)
        {
            if (Input.GetKey(KeyCode.Space) || virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Down)
            {
                Cur_Attack_State = Attack_state.None;
                Cur_state = Char_state.Block;
                Update_animation();
                Change_Behavior_enable = true;
            }
            if ((Input.GetKey(KeyCode.K)||virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None|| Gamepad.current.buttonEast.isPressed) && Dash_able == true)
            {
                if (virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None)
                {
                    Dash_by_swipe();
                }
                else
                {
                    if (Input.GetKey(KeyCode.A)|| Gamepad.current.dpad.left.value > 0)
                    {
                        Cur_Direction = Char_Direction.Left;
                        Cur_state = Char_state.Dash;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                    else if (Input.GetKey(KeyCode.D)||Gamepad.current.dpad.right.value > 0)
                    {
                        Cur_Direction = Char_Direction.Right;
                        Cur_state = Char_state.Dash;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                    else
                    {
                        Cur_state = Char_state.Dodge;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                }
            }
        }

        if ((Cur_Attack_State == Attack_state.Post_Attack || Cur_Attack_State == Attack_state.Attacking) && (Cur_state == Char_state.Attack_I))
        {
            if (Input.GetKeyDown(KeyCode.J) || virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down|| Gamepad.current.buttonWest.isPressed)
            {
                Combo_Continue = true;
            }
        }
        if (Cur_Attack_State == Attack_state.Post_Attack && Cur_state == Char_state.Attack_I)
        {
            if (Combo_Continue == true)
            {
                Cur_state = Char_state.Attack_II;
                Update_animation();
                Combo_Continue = false;
            }
        }
        if(Cur_state == Char_state.Parry) 
        {
            if (Input.GetKey(KeyCode.J)||virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down|| Gamepad.current.buttonWest.isPressed) 
            {
                Cur_state = Char_state.Attack_I;
                Update_animation() ;
            }
            
            if ((Input.GetKey(KeyCode.K) || virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None || Gamepad.current.buttonEast.isPressed) && Dash_able == true)
            {
                if (virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None)
                {
                    Dash_by_swipe();
                }
                else
                {
                    if (Input.GetKey(KeyCode.A) || Gamepad.current.dpad.left.value > 0)
                    {
                        Cur_Direction = Char_Direction.Left;
                        Cur_state = Char_state.Dash;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                    else if (Input.GetKey(KeyCode.D) || Gamepad.current.dpad.right.value > 0)
                    {
                        Cur_Direction = Char_Direction.Right;
                        Cur_state = Char_state.Dash;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                    else
                    {
                        Cur_state = Char_state.Dodge;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                }
            }
        }
        ////
        if ( (Input.GetKey(KeyCode.Space) == false && virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Up&& Gamepad.current.leftShoulder.value == 0) && Cur_state == Char_state.Block)
        {
            Change_Behavior_enable = true;
            Block_Enable = false;
        }
    }
    private void InputManager()
    {

        if ((Input.GetKeyDown(KeyCode.J) || (virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down) ) && Change_Behavior_enable == true)
        {
            if ((Input.GetKey(KeyCode.D) || virsual_Input.Horizontal > 0.3f ) && Change_Behavior_enable == true)
            {
                Cur_state = Char_state.Run;
                Cur_Direction = Char_Direction.Right;
                Update_animation();
                Dash_by_swipe();

            }
            else if ((Input.GetKey(KeyCode.A) || (virsual_Input.Horizontal < -0.3f)) && Change_Behavior_enable == true)
            {
                Cur_state = Char_state.Run;
                Cur_Direction = Char_Direction.Left;
                Update_animation();
                Dash_by_swipe();

            }
            Cur_state = Char_state.Attack_I;
            Update_animation();
            Change_Behavior_enable = false;
        }
        else if ((Input.GetKey(KeyCode.D) || virsual_Input.Horizontal > 0.3f) && Change_Behavior_enable == true)
        {
            Cur_state = Char_state.Run;
            Cur_Direction = Char_Direction.Right;
            Update_animation();
            Dash_by_swipe();
            if ((Input.GetKey(KeyCode.K)) && Dash_able == true)
            {
                Cur_Direction = Char_Direction.Right;
                Cur_state = Char_state.Dash;
                Update_animation();
                Change_Behavior_enable = false;
            }
            if ((Input.GetKey(KeyCode.Space) || (virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Down)) && Change_Behavior_enable == true && Block_Enable == true)
            {
                Cur_state = Char_state.Block;
                Update_animation();
                Change_Behavior_enable = false;
            }
            if ((Input.GetKeyDown(KeyCode.J) || (virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down)) && Change_Behavior_enable == true)
            {
                Cur_state = Char_state.Attack_I;
                Update_animation();
                Change_Behavior_enable = false;
            }
        }
        else if ((Input.GetKey(KeyCode.A) || (virsual_Input.Horizontal < -0.3f)) && Change_Behavior_enable == true)
        {
            Cur_state = Char_state.Run;
            Cur_Direction = Char_Direction.Left;
            Update_animation();
            Dash_by_swipe();
            if ((Input.GetKey(KeyCode.K)) && Dash_able == true)
            {
                Cur_Direction = Char_Direction.Left;
                Cur_state = Char_state.Dash;
                Update_animation();
                Change_Behavior_enable = false;

            }
            if ((Input.GetKey(KeyCode.Space) || (virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Down)) && Change_Behavior_enable == true && Block_Enable == true)
            {
                Cur_state = Char_state.Block;
                Update_animation();
                Change_Behavior_enable = false;
            }
            if ((Input.GetKeyDown(KeyCode.J) || (virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down)) && Change_Behavior_enable == true)
            {
                Cur_state = Char_state.Attack_I;
                Update_animation();
                Change_Behavior_enable = false;
            }
        }
        else if ((Input.GetKey(KeyCode.Space) || (virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Down)) && Change_Behavior_enable == true && Block_Enable == true)
        {
            Cur_state = Char_state.Block;
            Update_animation();
            Change_Behavior_enable = false;
        }
        else if ((Input.GetKey(KeyCode.K) || virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None) && Change_Behavior_enable == true && Dash_able == true)
        {
            if (virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None)
            {
                Dash_by_swipe();
            }
            else
            {
                if (Input.GetKey(KeyCode.A) )
                {
                    Cur_Direction = Char_Direction.Left;
                    Cur_state = Char_state.Dash;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    Cur_Direction = Char_Direction.Right;
                    Cur_state = Char_state.Dash;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
                else
                {
                    Cur_state = Char_state.Dodge;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
            }
        }
        //Idle(NoInput)
        else if (Change_Behavior_enable == true)
        {
            Cur_state = Char_state.Idle;
            Update_animation();
        }
        ////Bypass_State
        if ((Cur_Attack_State == Attack_state.Pre_Attack || Cur_Attack_State == Attack_state.Post_Attack) && Block_Enable == true)
        {
            if (Input.GetKey(KeyCode.Space) || virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Down)
            {
                Cur_Attack_State = Attack_state.None;
                Cur_state = Char_state.Block;
                Update_animation();
                Change_Behavior_enable = true;
            }
            if ((Input.GetKey(KeyCode.K) || virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None) && Dash_able == true)
            {
                if (virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None)
                {
                    Dash_by_swipe();
                }
                else
                {
                    if (Input.GetKey(KeyCode.A) )
                    {
                        Cur_Direction = Char_Direction.Left;
                        Cur_state = Char_state.Dash;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                    else if (Input.GetKey(KeyCode.D) )
                    {
                        Cur_Direction = Char_Direction.Right;
                        Cur_state = Char_state.Dash;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                    else
                    {
                        Cur_state = Char_state.Dodge;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                }
            }
        }

        if ((Cur_Attack_State == Attack_state.Post_Attack || Cur_Attack_State == Attack_state.Attacking) && (Cur_state == Char_state.Attack_I))
        {
            if (Input.GetKeyDown(KeyCode.J) || virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down )
            {
                Combo_Continue = true;
            }
        }
        if (Cur_Attack_State == Attack_state.Post_Attack && Cur_state == Char_state.Attack_I)
        {
            if (Combo_Continue == true)
            {
                Cur_state = Char_state.Attack_II;
                Update_animation();
                Combo_Continue = false;
            }
        }
        if (Cur_state == Char_state.Parry)
        {
            if (Input.GetKey(KeyCode.J) || virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down )
            {
                Cur_state = Char_state.Attack_I;
                Update_animation();
            }

            if ((Input.GetKey(KeyCode.K) || virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None) && Dash_able == true)
            {
                if (virsual_Input.Swipe != Virsual_input.Swipe_Gesture.None)
                {
                    Dash_by_swipe();
                }
                else
                {
                    if (Input.GetKey(KeyCode.A) )
                    {
                        Cur_Direction = Char_Direction.Left;
                        Cur_state = Char_state.Dash;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        Cur_Direction = Char_Direction.Right;
                        Cur_state = Char_state.Dash;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                    else
                    {
                        Cur_state = Char_state.Dodge;
                        Update_animation();
                        Change_Behavior_enable = false;
                    }
                }
            }
        }
        ////
        if ((Input.GetKey(KeyCode.Space) == false && virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Up) && Cur_state == Char_state.Block)
        {
            Change_Behavior_enable = true;
            Block_Enable = false;
        }
    }

    public void GotAttack(GameObject enemy) 
    {
        if(enemy.transform.position.x > gameObject.transform.position.x) 
        {
            if(Cur_Direction == Char_Direction.Right && Cur_state == Char_state.Block) 
            {
                if(Cur_Defend_State == Defend_state.Pre_Defend) 
                {
                    Cur_state = Char_state.Parry;
                    Change_Behavior_enable = false;
                    Update_animation();
                    Push(0.2f, enemy);
                }
                if(Cur_Defend_State == Defend_state.Guard) 
                {                    
                    Cur_state = Char_state.BlockReact;
                    Change_Behavior_enable = false;
                    Update_animation();                    
                    Push(2.5f, enemy);                    
                }
                
            }
            else if(Cur_state == Char_state.Dash|| Cur_state == Char_state.Dodge) 
            {
                //Break
            }
            else 
            {
                this.HP -= enemy.GetComponent<Enemy_Common>().DMG;                
                Cur_state = Char_state.Flinch;                
                Change_Behavior_enable = false;
                Push(2.5f, enemy);
                Update_animation();
            }
        }
        if (enemy.transform.position.x < gameObject.transform.position.x)
        {
            if (Cur_Direction == Char_Direction.Left && Cur_state == Char_state.Block)
            {
                if (Cur_Defend_State == Defend_state.Pre_Defend)
                {
                    Cur_state = Char_state.Parry;
                    Change_Behavior_enable = false;
                    Update_animation();
                    Push(0.2f, enemy);
                }
                if (Cur_Defend_State == Defend_state.Guard)
                {
                    Cur_state = Char_state.BlockReact;
                    Change_Behavior_enable = false;
                    Update_animation();
                    Push(2.5f, enemy);
                }
            }
            else
            {
                this.HP -= enemy.GetComponent<Enemy_Common>().DMG;
                Cur_state = Char_state.Flinch;
                Change_Behavior_enable = false;
                Push(2.5f, enemy);
                Update_animation();
                
            }
        }

    }
    private void Push(float force,GameObject enemy) 
    {
        if (enemy.transform.position.x < gameObject.transform.position.x)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(force, 0, 0);
        }
        if (enemy.transform.position.x > gameObject.transform.position.x)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(-force, 0, 0);
        }
    }
    private void Push(float force)
    {
        if (Cur_Direction == Char_Direction.Left)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(-force, 0, 0);
        }
        if (Cur_Direction == Char_Direction.Right)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(force, 0, 0);
        }
    }
    private void Dash_by_swipe() 
    {
        if(Change_Behavior_enable == true && Dash_able == true) 
        {
            if(Cur_Direction == Char_Direction.Left) 
            {
                if(virsual_Input.Swipe == Virsual_input.Swipe_Gesture.Left) 
                {
                    Cur_state = Char_state.Dash;
                    Cur_Direction = Char_Direction.Left;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
                else if(virsual_Input.Swipe == Virsual_input.Swipe_Gesture.Right) 
                {
                    Cur_state =Char_state.Dodge;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
            }
            else if (Cur_Direction == Char_Direction.Right)
            {
                if (virsual_Input.Swipe == Virsual_input.Swipe_Gesture.Right)
                {
                    Cur_state = Char_state.Dash;
                    Cur_Direction = Char_Direction.Right;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
                else if (virsual_Input.Swipe == Virsual_input.Swipe_Gesture.Left)
                {
                    Cur_state = Char_state.Dodge;
                    Update_animation();
                    Change_Behavior_enable = false;
                }
            }
        }
    }
   




}
