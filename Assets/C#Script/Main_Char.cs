using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class Main_Char : Character
{
           
    [SerializeField] private bool Change_Behavior_enable = true;    
    private float Block_CoolDown = 0.45f;
    bool Block_Enable = true; 
    public string Char_CurState;
    [SerializeField] private float Speed = 2f;
    [SerializeField] public BoxCollider Attack_Box;
    [SerializeField] public CapsuleCollider Hitted_Box;
    public float Animation_TimeLine;
    public float Animation_Length;
    public float DMG = 15;
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
        base.animator = gameObject.GetComponent<Animator>();
        base.CharRenderer = gameObject.GetComponent<SpriteRenderer>();
        Cur_state = Char_state.Idle;
        Cur_Direction = Char_Direction.Right;               
        Attack_Box = gameObject.GetComponentInChildren<BoxCollider>();   
        Hitted_Box.transform.localPosition = new Vector3(-0.25f,0,0);        
    }

    public void Update()
    {                       
        Char_CurState = Cur_state.ToString();
        Animation_Length = animator.GetCurrentAnimatorStateInfo(0).length;
        Animation_TimeLine = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        InputManager();
        Block_Cooldown();       
        Update_animation();        
    }
    public void FixedUpdate()
    {
        Update_State_and_Dicrection();
        Update_Pos();
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
            gameObject.transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));                     
        }                       
    }
    public float Block_timimg = 0f;
    private void Update_State_and_Dicrection()
    {
            
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
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.36f * (float)(100f / 60f))
            {
                Cur_Attack_State = Attack_state.Attacking;

            }
            //PostAttack_I
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.36f * (float)(100f / 60f)
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
        //Flinch
        if (Cur_state == Char_state.Flinch)
        {            
            Change_Behavior_enable = false;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > animator.GetCurrentAnimatorStateInfo(0).length)
            {
                Change_Behavior_enable = true;
            }
        }
        //Defend_state
        if(Cur_state == Char_state.Block) 
        {
            Block_timimg += Time.deltaTime;
            if(Block_timimg < 0.5f) 
            {
                Cur_Defend_State = Defend_state.Pre_Defend;
            }
            else if(Block_timimg >= 0.5f ) 
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
        
        if (Input.GetKeyDown(KeyCode.J) && Change_Behavior_enable == true)
        {
            Cur_state = Char_state.Attack_I;
            Change_Behavior_enable = false;
        }
        //Input
        ////Bypass_State
        if (Cur_Attack_State == Attack_state.Pre_Attack && Block_Enable == true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Cur_Attack_State = Attack_state.None;
                Cur_state = Char_state.Block;
                Change_Behavior_enable = true;
            }
        }
        
        if((Cur_Attack_State == Attack_state.Post_Attack|| Cur_Attack_State == Attack_state.Attacking)&&(Cur_state == Char_state.Attack_I)) 
        {
            if (Input.GetKeyDown(KeyCode.J)) 
            {
                Combo_Continue = true;
            }
        }
        if (Cur_Attack_State == Attack_state.Post_Attack && Cur_state == Char_state.Attack_I)
        {            
            if (Combo_Continue == true)
            {
                Cur_state = Char_state.Attack_II;
                Combo_Continue = false;
            }
        }
        /////
        if (Input.GetKey(KeyCode.Space) && Change_Behavior_enable == true && Block_Enable == true)
        {
            Cur_state = Char_state.Block;
            Change_Behavior_enable = false;
        }
        if (Input.GetKey(KeyCode.D) && Change_Behavior_enable == true)
        {
            Cur_state = Char_state.Run;
            Cur_Direction = Char_Direction.Right;
        }
        if (Input.GetKey(KeyCode.A) && Change_Behavior_enable == true)
        {
            Cur_state = Char_state.Run;
            Cur_Direction = Char_Direction.Left;
        }
        if (Input.GetKey(KeyCode.Space)==false && Cur_state == Char_state.Block)
        {
            Change_Behavior_enable = true;
            Block_Enable = false;
        }
       
        //Idle(NoInput)
        if (Change_Behavior_enable == true && Input.anyKey == false)
        {
            Cur_state = Char_state.Idle;
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
                    Push(60, enemy);
                }
                if(Cur_Defend_State == Defend_state.Guard) 
                {
                    Cur_state = Char_state.BlockReact;
                    Change_Behavior_enable = false;
                    Update_animation();
                    Push(150, enemy);
                }
                
            }
            else 
            {
                Cur_state = Char_state.Flinch;  
                Change_Behavior_enable = false;
                Push(150, enemy);
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
                    Push(60, enemy);
                }
                if (Cur_Defend_State == Defend_state.Guard)
                {
                    Cur_state = Char_state.BlockReact;
                    Change_Behavior_enable = false;
                    Update_animation();
                    Push(150, enemy);
                }
            }
            else
            {
                Cur_state = Char_state.Flinch;
                Change_Behavior_enable = false;
                Push(150, enemy);
                Update_animation();
                
            }
        }

    }
    private void Push(float force,GameObject enemy) 
    {
        if (enemy.transform.position.x < gameObject.transform.position.x)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(force, 0, 0));
        }
        if (enemy.transform.position.x > gameObject.transform.position.x)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(-force, 0, 0));
        }
    }
    
   
    

}
