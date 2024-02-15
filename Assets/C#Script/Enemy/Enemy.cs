using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Enemy : Character
{
    [SerializeField] protected float Run_Speed = 1.7f;
    [SerializeField] protected float Dash_Speed = 7;
    [SerializeField] protected float Speed;
    [SerializeField] protected float HP = 40;
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
        Dead,
        Parried
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
    //Start
    public virtual void Start()
    {
        Cur_state = State.Idle;
        Time.fixedDeltaTime = 1 / 60f;
        base.LoadComponent();
        Speed = Run_Speed;
        ATK_Range = 2.1f;
        Change_state_enable = true;
        Player = Target.GetComponent<Main_Char>();       
        Defend = 40;
        Director.AddEnemy(gameObject);
        Cur_Role = Role.AroundFight;
        RandomDistance();

    }
    //Update
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        
        Distance = Math.Abs(base.transform.position.x - Player.transform.position.x);
        StateManagement();
        RoleManagement();      
        //Calculate_Distance                
        MovementSpeedUpdate();
        Animation_Update();
        Cur_state_string = Cur_state.ToString();
        Cur_Attackstate_string = Cur_Attack_State.ToString();
        
       
    }
    protected float Under_Pressure_Approuching = 60;
    protected float Over_Pressure_Reteating = 65;
    protected float Under_ATK_Range_Reteating = 2f;
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
        Animation_Update();
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
        
        if (Dash_CoolingDown <= 0 )
        {           
            Dash();
        }
        else
        {
            Cur_state = State.Run;
            Animation_Update();
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
        Animation_Update();
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
        Animation_Update();
        Dash_CoolingDown = Dash_Cooldown_Duration;        
    }
    protected void Attack()
    {
        Cur_state = State.Attack_I;
        Animation_Update();
        float Attack_Cooldown_Duration = 1.3f;
        Attack_CoolingDown = Attack_Cooldown_Duration;
        float AttackPressure = 50;
        Pressure += AttackPressure;
    }
    protected void Cooldown_Event()
    {
        //Pressure_Cooldown
        float Pressure_Regen_Speed = 25;
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
        //RandomDistanceCoolDown
        if (RandomCooldown > 0) 
        {
            RandomCooldown -= Time.deltaTime;
        }

    }
    private float Dashing = 0;
    protected void StateManagement()
    {
        if (HP <= 0||Cur_state == State.Dead)
        {
            Cur_state = State.Dead;
            Director.RemoveGameObg(gameObject);        
            Destroy(gameObject);
        }
        else
        {
            LookatPlayer();
            //Dashing_Duration&Moving
            if (Cur_state == State.Dash)
            {
                Animation_Update();
                //Duration
                float Dash_Duration = animator.GetCurrentAnimatorStateInfo(0).length;
                Change_state_enable = false;
                Dashing += Time.deltaTime;
                if (Dashing >= Dash_Duration)
                {
                    Change_state_enable = true;
                    Dashing = 0;
                    if (UnityEngine.Random.Range(1, 100) > 50)
                    {
                        OnFightDismiss();
                    }
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
            if (Cur_state != State.Attack_I)
            {
                Cur_Attack_State = Attack_State.None;
            }
            //Block_Duration
            if (Cur_state == State.Block)
            {              
                Change_state_enable = false;
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
                {
                    Change_state_enable = true;                   
                }
            }
            //Flinch
            if (Cur_state == State.Flinch)
            {
                
                Change_state_enable = false;
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
                {
                    Change_state_enable = true;                    
                }
            }
            if(Cur_state == State.Parried) 
            {
                OnFightDismiss();
                Change_state_enable = false;
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
                {
                    Change_state_enable = true;                   
                }
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
    public void Animation_Update()
    {
        animator.Play(Cur_state.ToString());
    }
    public bool Hit_able = true;
    
    public void Got_Attacked() 
    {
        if (Defend > 0 && Cur_state != State.Attack_I && Cur_state != State.Parried) 
        {
            Cur_state = State.Block;                        
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
            if(Cur_state == State.Parried) 
            {
                Pressure += 50;
                OnFightDismiss();
            }
            Cur_state = State.Flinch;
            Animation_Update();
            if (UnityEngine.Random.Range(1, 10) <= 2) 
            {
                //Critical
                HP -= Player.DMG*1.5f;
                Debug.Log("Critical_Hit:" + Player.DMG * 1.5f);
            }
            else 
            {
                HP -= Player.DMG;
                Debug.Log("Hit:" + Player.DMG );
            }            
            Pressure += 10;
            Push(100);
            
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
    private float[] DistanceMove = new float[3];
    private void RoleManagement() 
    {
        if (Cur_state != State.Dead)
        {
            RoleOnfight();
            RoleAroundfight();
        }
    }
    private void RoleOnfight() 
    {
        //Role_Onfight //****
        if (Cur_Role == Role.OnFight)
        {
            if (Pressure < Under_Pressure_Approuching && Distance > ATK_Range && Change_state_enable == true)
            {
                Move_to_player();
            }           
            //Reteat_Condition       
            else if (Pressure > Over_Pressure_Reteating && Distance < Under_ATK_Range_Reteating && Change_state_enable == true)
            {
                Reteat();                
            }            
            //Attack_Condition
            else if (Distance <= ATK_Range && Attack_CoolingDown <= 0 && Pressure < Under_Pressure_ATK && Change_state_enable == true)
            {
                Attack();
            }
            else if (Change_state_enable == true)
            {
                Cur_state = State.Idle;
                Animation_Update();
            }
        }
    }
    public enum front_of_enemy {Enemy,Player,None };
    public front_of_enemy front_Of_Enemy;
    private void RoleAroundfight() 
    {
        //Role_Aroundfight //****
        if (Cur_Role == Role.AroundFight)
        {
            if (Cur_Direction == Direction.Left)
            {
                if (Physics.Raycast(new Vector3(transform.position.x - 0.25f, transform.position.y - 0.5f, transform.position.z), new Vector3(-20, 0, 0), out RaycastHit HitLeft, 100, 3))
                {
                    if(HitLeft.rigidbody.tag == "Enemy") 
                    {
                        front_Of_Enemy = front_of_enemy.Enemy;
                    }
                    else if(HitLeft.rigidbody.tag == "Player") 
                    {
                        front_Of_Enemy = front_of_enemy.Player;
                    }
                    else 
                    {
                        front_Of_Enemy = front_of_enemy.None;
                    }
                    RayDistance = HitLeft.distance;
                    //Raycast Enemy
                    if (HitLeft.distance < DistanceMove[0] && HitLeft.rigidbody.tag == "Enemy" && Change_state_enable == true)
                    {
                        RandomDistance();
                        MoveBack();                       
                    }
                    else if (HitLeft.distance >= DistanceMove[1] && HitLeft.rigidbody.tag == "Enemy" && Change_state_enable == true)
                    {
                        RandomDistance();
                        Move_to_player();
                    }
                    //Raycast Player                    
                    else if (HitLeft.distance >= DistanceMove[2] && HitLeft.rigidbody.tag == "Player" && Change_state_enable == true)
                    {
                        RandomDistance();
                        Move_to_player();
                    }
                    else if (HitLeft.rigidbody.tag == "Player" && Change_state_enable == true
                        && (HitLeft.rigidbody.GetComponent<Main_Char>().Cur_Attack_State == Main_Char.Attack_state.Attacking
                        || HitLeft.rigidbody.GetComponent<Main_Char>().Cur_Attack_State == Main_Char.Attack_state.Post_Attack)&&(Cur_state != State.Run))
                    {
                        RandomDistance();
                    }
                    else if (Change_state_enable == true)
                    {
                        Cur_state = State.Idle;
                        Animation_Update();
                    }                   
                }
                else if (Change_state_enable == true)
                {
                    Cur_state = State.Idle;
                    Animation_Update();
                }

            }
            if (Cur_Direction == Direction.Right)
            {
                if (Physics.Raycast(new Vector3(transform.position.x + 0.25f, transform.position.y - 0.5f, transform.position.z), new Vector3(20, 0, 0), out RaycastHit HitRight, 100, 3))
                {
                    if (HitRight.rigidbody.tag == "Enemy")
                    {
                        front_Of_Enemy = front_of_enemy.Enemy;
                    }
                    else if (HitRight.rigidbody.tag == "Player")
                    {
                        front_Of_Enemy = front_of_enemy.Player;
                    }
                    else
                    {
                        front_Of_Enemy = front_of_enemy.None;
                    }
                    //Raycast Enemy
                    if (HitRight.distance < DistanceMove[0] && HitRight.rigidbody.tag == "Enemy" && Change_state_enable == true)
                    {
                        RandomDistance();
                        MoveBack();                        
                    }
                    else if (HitRight.distance >= DistanceMove[1] && HitRight.rigidbody.tag == "Enemy" && Change_state_enable == true)
                    {
                        RandomDistance();
                        Move_to_player();
                    }
                    //Raycast Player
                    else if (HitRight.rigidbody.tag == "Player" && Change_state_enable == true
                        && (HitRight.rigidbody.GetComponent<Main_Char>().Cur_Attack_State == Main_Char.Attack_state.Attacking
                        || HitRight.rigidbody.GetComponent<Main_Char>().Cur_Attack_State == Main_Char.Attack_state.Post_Attack) && (Cur_state != State.Run))
                    {
                        RandomDistance();
                    }
                    else if (HitRight.distance >= DistanceMove[2] && HitRight.rigidbody.tag == "Player" && Change_state_enable == true)
                    {
                        RandomDistance();
                        Move_to_player();
                    }
                    else if (Change_state_enable == true)
                    {
                        Cur_state = State.Idle;
                        Animation_Update();
                    }                   
                }
                else if (Change_state_enable == true)
                {
                    Cur_state = State.Idle;
                    Animation_Update();
                }

            }

        }
    }
    private float RandomCooldown = 0;
    private void RandomDistance() 
    {
        if(RandomCooldown <=0)
        {
        RandomCooldown = 4f;       
        //DistanceMoveBack CheckwithEnemy
        DistanceMove[0] = (float)(UnityEngine.Random.Range(100, 460)/100f);
        //DistanceMoveForward CheckwithEnemy
        DistanceMove[1] = DistanceMove[0] + 0.4f;
        //DistanceMoveForward ChecwithPlayer
        DistanceMove[2] = (float)(UnityEngine.Random.Range(170, 450)/100f);
        }
    }
    private void OnFightDismiss() 
    {
        Cur_Role = Role.AroundFight;
        Debug.Log("Dismiss");
    }
    public void UpdateState(State state) 
    {
        Cur_state = state;
        Animation_Update();
    }



}
