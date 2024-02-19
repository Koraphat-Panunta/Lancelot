using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class Main_Char : Character
{
    public float HP = 100f;      
    [SerializeField] private bool Change_Behavior_enable = true;    
    private float Block_CoolDown = 0.45f;
    bool Block_Enable = true; 
    public string Char_CurState;
    [SerializeField] private float Speed = 120;
    [SerializeField] public BoxCollider Attack_Box;
    [SerializeField] public CapsuleCollider Hitted_Box;
    public float Animation_TimeLine;
    public float Animation_Length;
    public float DMG = 15;
    [SerializeField] Virsual_input virsual_Input;
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
    }
    public enum Char_Direction 
    {
        Left,
        Right
    }
    public Char_state Cur_state;
    public Char_Direction Cur_Direction;
    

    
    public void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        base.animator = gameObject.GetComponent<Animator>();
        base.CharRenderer = gameObject.GetComponent<SpriteRenderer>();
        Cur_state = Char_state.Idle;
        Cur_Direction = Char_Direction.Right;               
        Attack_Box = gameObject.GetComponentInChildren<BoxCollider>();   
        Hitted_Box.transform.localPosition = new Vector3(0,0,0);        
        Virsual_input virsual_Input = GetComponent<Virsual_input>();
    }

    protected override void Update()
    {                       
        Char_CurState = Cur_state.ToString();
        Animation_Length = animator.GetCurrentAnimatorStateInfo(0).length;
        Animation_TimeLine = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        InputManager();
        Block_Cooldown();       
        Update_animation();     
        
        
    }
    protected override void FixedUpdate()
    {
        Update_State_and_Dicrection();
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
   
    private void Block_Cooldown() 
    {        
        if (Block_Enable == false)
        {
            Block_CoolDown -= Time.deltaTime;
            if (Block_CoolDown <= 0)
            {
                Block_CoolDown = 0.45f;
                Block_Enable = true;
            }
        }
    }
   
    private void Update_Pos()
    {
        if (Cur_state == Char_state.Run)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Speed*Time.deltaTime, 0, 0);
            //gameObject.transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
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
        if (Cur_state == Char_state.Attack_I || Cur_state == Char_state.Attack_II)
        {
            //PreAttack_I
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.20f * (float)(100f / 60f))
            {
                Cur_Attack_State = Attack_state.Pre_Attack;
            }
            //Attack_I
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.20f * (float)(100f / 60f)
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.30f * (float)(100f / 60f))
            {
                Cur_Attack_State = Attack_state.Attacking;
                if (Attack_step_Enable == true)
                {
                    Attack_step_Enable = false;
                    Push(4.5f);
                }
            }
            //PostAttack_I
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.30f * (float)(100f / 60f)
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.58f * (float)(100f / 60f))
            {
                Cur_Attack_State = Attack_state.Post_Attack;
            }
            //Finish
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
            {
                Cur_Attack_State = Attack_state.None;
                Change_Behavior_enable = true;
                
            }
        }
        else 
        {
            Cur_Attack_State = Attack_state.None;
        }
        if(Cur_Attack_State != Attack_state.Attacking) 
        {
            Attack_step_Enable = true;
        }
        //Flinch
        if (Cur_state == Char_state.Flinch)
        {            
            Change_Behavior_enable = false;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > animator.GetCurrentAnimatorStateInfo(0).length)
            {
                Change_Behavior_enable = true;
                Cur_state = Char_state.Idle;
                Update_animation();
            }
        }
        int parry_frame = 8;
        //Defend_state
        if(Cur_state == Char_state.Block) 
        {
            Block_timimg += Time.deltaTime;
            if(Block_timimg < parry_frame*Time.deltaTime) 
            {
                Cur_Defend_State = Defend_state.Pre_Defend;
            }
            else if(Block_timimg >= parry_frame * Time.deltaTime) 
            {
                Cur_Defend_State = Defend_state.Guard;
            }
        }
        else if(Cur_state == Char_state.BlockReact) 
        {
            Block_timimg += Time.deltaTime;
            Change_Behavior_enable = false;
            Cur_Defend_State = Defend_state.Blocking;
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length) 
            {
                Cur_state = Char_state.Block;
                Change_Behavior_enable = true;
            }
        }
        else if(Cur_state == Char_state.Parry) 
        {
            Block_timimg += Time.deltaTime;
            Change_Behavior_enable = false;
            Cur_Defend_State = Defend_state.Parry;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
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
    }
    private bool Combo_Continue = false;

    
    private void InputManager() 
    {
        
        if ((Input.GetKeyDown(KeyCode.J)||(virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down)) && Change_Behavior_enable == true)
        {
            Cur_state = Char_state.Attack_I;
            Update_animation();
            Change_Behavior_enable = false;
        }
        //Input
              
        if ((Input.GetKey(KeyCode.Space)||( virsual_Input.Cur_Button_Defend == Virsual_input.Button_Defend_State.Down)) && Change_Behavior_enable == true && Block_Enable == true)
        {
            Cur_state = Char_state.Block;
            Update_animation();
            Change_Behavior_enable = false;
        }
         if((Input.GetKey(KeyCode.D)||virsual_Input.button_Right_State == Virsual_input.Button_Right_State.Down) && Change_Behavior_enable == true )
        {
            Cur_state = Char_state.Run;
            Cur_Direction = Char_Direction.Right;
            Update_animation();
        }
         if ((Input.GetKey(KeyCode.A)||(virsual_Input.button_Left_State == Virsual_input.Button_Left_State.Down)) && Change_Behavior_enable == true )
        {
            Cur_state = Char_state.Run;
            Cur_Direction = Char_Direction.Left;
            Update_animation();
        }
           
        //Idle(NoInput)
         if (Change_Behavior_enable == true && Input.anyKey == false)
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
        }

        if ((Cur_Attack_State == Attack_state.Post_Attack || Cur_Attack_State == Attack_state.Attacking) && (Cur_state == Char_state.Attack_I))
        {
            if (Input.GetKeyDown(KeyCode.J) || virsual_Input.Cur_Button_Attack == Virsual_input.Button_Attack_State.Down)
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
        ////
        if (Input.GetKey(KeyCode.Space) == false && Cur_state == Char_state.Block)
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





}
