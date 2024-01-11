using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Enemy : Character
{
    [SerializeField] protected float Run_Speed = 1.7f;
    [SerializeField] protected float Dash_Speed = 7;
    [SerializeField] protected float Speed;
    [SerializeField] protected float HP = 100;
    [SerializeField] protected float Defend;
    [SerializeField] protected float Pressure;
    [SerializeField] public float Distance;
    [SerializeField] protected float RayDistance;
    [SerializeField] protected GameObject Target;
     protected Main_Char Player;
    [SerializeField] protected float ATK_Range;
    [SerializeField] protected float Attack_CoolingDown;
    [SerializeField] protected float Dash_CoolingDown;
    public BoxCollider Attack_Box;
    public CapsuleCollider Hitted_Box;
    public bool Change_state_enable;
    public string Cur_state_string;
    public string Cur_Attackstate_string;    
    public bool GetHitted_able;
    [SerializeField] private Enemy_Director Director;

    public enum Direction
    {
        Left,
        Right
    }
    public enum State
    {
        Run,
        Dash,
        Attack_I,
        Idle,
        Block,
        Flinch,
        Dead
    }
    public enum Attack_State 
    {
        None,
        Pre_Attack,
        Attacking,
        Post_Attack
    }
    public Attack_State Cur_Attack_State = Attack_State.None;
    public enum Role
    {
        OffFight,
        AroundFight,
        OnFight
    }
    public Role Cur_Role = Role.OnFight;
    public State Cur_state;
    protected Direction Cur_Direction;
    public virtual void Start()
    {
        Cur_state = State.Idle;
        Time.fixedDeltaTime = 1 / 60f;
        base.LoadComponent();
        Speed = Run_Speed;
        ATK_Range = 2.1f;
        Change_state_enable = true;
        Player = Target.GetComponent<Main_Char>();       
        Defend = 100;
        Director.AddEnemy(gameObject);
        Cur_Role = Role.AroundFight;
    }
    public virtual void Update()
    {
        
    }
    public void FixedUpdate()
    {
        
        Distance = Math.Abs(base.transform.position.x - Player.transform.position.x);
        StateManagement();
        //Role_Aroundfight
        if (Cur_Role == Role.AroundFight)  
        {
            if (Cur_Direction == Direction.Left)
            {
                if (Physics.Raycast(new Vector3(transform.position.x - 0.25f, transform.position.y - 0.5f, transform.position.z), new Vector3(-1, 0, 0), out RaycastHit HitLeft, 100, 3))
                {
                    RayDistance = HitLeft.distance;
                    //Raycast Enemy
                    if (HitLeft.distance < 1f && HitLeft.rigidbody.tag == "Enemy"&& Change_state_enable == true)
                    {
                        MoveBack();
                        Debug.Log("HitEnemy");
                    }
                    else if(HitLeft.distance >= 4 && HitLeft.rigidbody.tag == "Enemy" && Change_state_enable == true) 
                    {
                        Move_to_player();
                    }
                    //Raycast Player
                    else if(HitLeft.distance < 0.1f && HitLeft.rigidbody.tag == "Player" && Change_state_enable == true)
                    {
                        MoveBack();
                        Debug.Log("HitPlayer");
                    }
                    else if (HitLeft.distance >= 4 && HitLeft.rigidbody.tag == "Player" && Change_state_enable == true)
                    {
                        Move_to_player();
                    }
                    else if(Change_state_enable == true)
                    {
                        Cur_state = State.Idle;
                        Animation_Update();
                    }
                   
                }
                else if(Change_state_enable == true)
                {
                    Cur_state = State.Idle;
                    Animation_Update();
                }
                
            }
            if (Cur_Direction == Direction.Right)
            {
                if (Physics.Raycast(new Vector3(transform.position.x + 0.25f, transform.position.y - 0.5f, transform.position.z), new Vector3(1, 0, 0), out RaycastHit HitRight, 100, 3))
                {
                    //Raycast Enemy
                    if (HitRight.distance < 1f && HitRight.rigidbody.tag == "Enemy"&&Change_state_enable == true)
                    {
                        MoveBack();
                        Debug.Log("HitEnemy");
                    }
                    else if (HitRight.distance >= 4 && HitRight.rigidbody.tag == "Enemy" && Change_state_enable == true)
                    {
                        Move_to_player();
                    }
                    //Raycast Player
                    else if (HitRight.distance < 0.1f && HitRight.rigidbody.tag == "Player" && Change_state_enable == true)
                    {
                        MoveBack();
                        Debug.Log("HitPlayer");
                    }
                    else if (HitRight.distance >= 4 && HitRight.rigidbody.tag == "Player" && Change_state_enable == true)
                    {
                        Move_to_player();
                    }
                    else if(Change_state_enable == true)
                    {
                        Cur_state = State.Idle;
                        Animation_Update();
                    }
                }
                else if(Change_state_enable == true)
                { 
                    Cur_state = State.Idle;
                    Animation_Update();
                }
                
            }
            if (Pressure > Over_Pressure_Reteating && Distance < Under_ATK_Range_Reteating && Change_state_enable == true)
            {
                MoveBack();
            }
        }
        //Role_Onfight
        if (Cur_Role == Role.OnFight)
        {
            if (Pressure < Under_Pressure_Approuching && Distance > ATK_Range && Change_state_enable == true)
            {
                Move_to_player();
            }
            else if (Change_state_enable == true)
            {
                Cur_state = State.Idle;
            }
            //Reteat_Condition       
            if (Pressure > Over_Pressure_Reteating && Distance < Under_ATK_Range_Reteating && Change_state_enable == true)
            {
                Reteat();
            }
            //Attack_Condition
            if (Distance <= ATK_Range && Attack_CoolingDown <= 0 && Pressure < Under_Pressure_ATK && Change_state_enable == true)
            {
                Attack();
            }
        }
        //Calculate_Distance                
        MovementSpeedUpdate();
        Animation_Update();
        Cur_state_string = Cur_state.ToString();
        Cur_Attackstate_string = Cur_Attack_State.ToString();
        if(Cur_state == State.Dead) 
        {
            gameObject.SetActive(false);
        }
    }
    protected float Under_Pressure_Approuching = 60;
    protected float Over_Pressure_Reteating = 65;
    protected float Under_ATK_Range_Reteating = 5.1f;
    protected float Under_Pressure_ATK;
    
    protected void LookatPlayer()
    {
        if (base.transform.position.x > Player.transform.position.x)
        {
            Cur_Direction = Direction.Left;
            animator.GetComponent<SpriteRenderer>().flipX = true;
            //Set_AttackBox
            Attack_Box.transform.localPosition = new Vector3(-Math.Abs(Attack_Box.transform.localPosition.x),
                Attack_Box.transform.localPosition.y,
                Attack_Box.transform.localPosition.z);
            //Set_HittedBox
            Hitted_Box.transform.localPosition = new Vector3(Math.Abs(Hitted_Box.transform.localPosition.x),
                Hitted_Box.transform.localPosition.y,
                Hitted_Box.transform.localPosition.z);
        }
        if (base.transform.position.x < Player.transform.position.x)
        {
            Cur_Direction = Direction.Right;
            animator.GetComponent<SpriteRenderer>().flipX = false;

        }
    }
    protected void Move_to_player()
    {
        Cur_state = State.Run;
        if (Cur_Direction == Direction.Left)
        {
            base.transform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));
        }
        if (Cur_Direction == Direction.Right)
        {
            base.transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
        }
    }
    protected void Reteat()
    {
        if (Dash_CoolingDown <= 0 && Cur_Role == Role.OnFight)
        {           
            Dash();
        }
        else
        {
            Cur_state = State.Run;
        }
        if (base.transform.position.x > Player.transform.position.x)
        {
            base.transform.transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
        }
        else if (base.transform.position.x < Player.transform.position.x)
        {
            base.transform.transform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));
        }
    }
    protected void MoveBack() 
    {
        Cur_state = State.Run;
        if (base.transform.position.x > Player.transform.position.x)
        {
            base.transform.transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
        }
        else if (base.transform.position.x < Player.transform.position.x)
        {
            base.transform.transform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));
        }
    }
    protected void Dash()
    {
        float Dash_Cooldown_Duration = 4;
        Cur_state = State.Dash;
        Dash_CoolingDown = Dash_Cooldown_Duration;
    }
    protected void Attack()
    {
        Cur_state = State.Attack_I;
        float Attack_Cooldown_Duration = 1.8f;
        Attack_CoolingDown = Attack_Cooldown_Duration;
        float AttackPressure = 50;
        Pressure += AttackPressure;
    }
    protected void Cooldown_Event()
    {
        //Pressure_Cooldown
        float Pressure_Regen_Speed = 19;
        if (Pressure > 0)
        {
            Pressure -= Pressure_Regen_Speed * Time.deltaTime;
            if (Pressure > 100)
            {
                Pressure = 100;
            }
        }
        //Attack_Cooldown
        if (Attack_CoolingDown > 0)
        {
            Attack_CoolingDown -= Time.deltaTime;
        }
        //Dash_Cooldown
        if (Dash_CoolingDown > 0)
        {
            Dash_CoolingDown -= Time.deltaTime;
        }

    }
    private float Dashing = 0;
    protected void StateManagement()
    {
        LookatPlayer();
        //Dashing_Duration&Moving
        if (Cur_state == State.Dash)
        {
            //Duration
            float Dash_Duration = animator.GetCurrentAnimatorStateInfo(0).length;
            Change_state_enable = false;
            Dashing += Time.deltaTime;
            if (Dashing >= Dash_Duration)
            {
                Change_state_enable = true;
                Dashing = 0;
            }
            //Moving
            if (base.transform.position.x > Player.transform.position.x)
            {
                base.transform.transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
            }
            else if (base.transform.position.x < Player.transform.position.x)
            {
                base.transform.transform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));
            }
        }        
        //Attack_Duration
        if (Cur_state == State.Attack_I)
        {
            Change_state_enable = false;
            //PreAttack_I
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.20f * (float)(100f / 60f))
            {
                Cur_Attack_State = Attack_State.Pre_Attack;
            }
            //Attack_I
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.20f * (float)(100f / 60f)
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.36f * (float)(100f / 60f))
            {
                Cur_Attack_State = Attack_State.Attacking;
            }
            //PostAttack_I
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.36f * (float)(100f / 60f)
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.58f * (float)(100f / 60f))
            {
                Cur_Attack_State = Attack_State.Post_Attack;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
            {
                Change_state_enable = true;
            }
        }
        if(Cur_state != State.Attack_I) 
        {
            Cur_Attack_State = Attack_State.None;
        }
        //Block_Duration
        if(Cur_state == State.Block) 
        {
            Debug.Log("Block");
            Change_state_enable = false;            
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
            {
                Change_state_enable = true;
                Debug.Log("Return");
            }
        }
        if(Cur_state == State.Flinch) 
        {
            Debug.Log("Flinch");
            Change_state_enable = false;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
            {
                Change_state_enable = true;
                Debug.Log("Return");
            }
        }
        Cooldown_Event();
    }
    protected void MovementSpeedUpdate()
    {
        if (Cur_state == State.Dash)
        {
            Speed = Dash_Speed;
        }
        else
        {
            Speed = Run_Speed;
        }
    }
    protected void Animation_Update()
    {
        animator.Play(Cur_state.ToString());
    }
    public bool Hit_able = true;
    
    public void Got_Attacked() 
    {
        if (Defend > 0 && Cur_state != State.Attack_I) 
        {
            Cur_state = State.Block;            
            Debug.Log("EnemyBlock");
            Animation_Update();
        }
        if(Cur_state == State.Block) 
        {
            Defend -= Player.DMG * 0.7f;
            Pressure += 5;           
            Push(200);
        }
        else 
        {
            Cur_state = State.Flinch;
            Animation_Update();
            HP -= Player.DMG;
            Pressure += 10;
            Push(100);
            Debug.Log("EnemyHited");
        }
    }
    public void Push(float force) 
    {
        if (Cur_Direction == Direction.Left)
        {
            rb.GetComponent<Rigidbody>().AddForce(new Vector3(force, 0, 0));
        }
        if (Cur_Direction == Direction.Right)
        {
            rb.GetComponent<Rigidbody>().AddForce(new Vector3(-force, 0, 0));
        }
    }



}
