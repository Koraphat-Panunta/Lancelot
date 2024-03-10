using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Enemy : Character
{
    [SerializeField] protected float Run_Speed;
    protected float Run_Speed_Onfight;
    protected float Run_Speed_Offfight;
    [SerializeField] protected float HP;
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
    [SerializeField] protected CapsuleCollider Character_Collider;
    public bool Change_state_enable;
    [SerializeField] private bool Counter_Enable;
    public string Cur_state_string;
    public string Cur_Attackstate_string;
    public bool GetHitted_able;
    [SerializeField] private Enemy_Director Director;
    public float DMG = 8;
    [SerializeField] float animation_leght;
    [SerializeField] float animation_curtime;

    public enum Direction
    {
        Left,
        Right
    }
    public enum State
    {
        Run,
        Dash_Back,
        Dash_Forward,
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
        Player = Target.GetComponent<Main_Char>();
        base.LoadComponent();
        ATK_Range = 2.1f;
        Director.AddEnemy(gameObject);
        Cur_state = State.Idle;
        Spawn(gameObject.transform.position,50,20);          
        gameObject.SetActive(false);
    }
    public void Spawn(Vector3 Postion, float HP, float Defend)
    {
        gameObject.transform.position = Postion;
        this.HP = HP;
        this.Defend = Defend;
        Counter_Enable = true;
        Cur_Role = Role.AroundFight;
        Cur_state = State.Idle;
        Change_state_enable = true;
        gameObject.SetActive(true);
        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        Hitted_Box.gameObject.SetActive(true);
        gameObject.GetComponent<Rigidbody>().detectCollisions = true;
        RandomDistance();
        Run_Speed_Onfight = UnityEngine.Random.Range(99, 117);
        Run_Speed_Offfight = UnityEngine.Random.Range(69, 96);

    }
    //Update
    public double Deltatime;
    protected override void Update()
    {
        base.Update();
        animation_leght = animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f);
        animation_curtime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime* (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f));
        This_Uptate();
        
    }
    protected override void FixedUpdate()
    {
        MovingUpdate();
        Cooldown_Event();
        Deltatime = Time.deltaTime;
    }
    bool Dash_able = true;
    private void MovingUpdate() 
    {
        if (Cur_state == State.Dash_Back)
        {
            float Dash_n_Dodge = 12;
            Animation_Update();
            rb.drag = 3.7f;
            //Duration            
            Change_state_enable = false;
            Dashing += Time.deltaTime;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Cur_state = State.Idle;
                Change_state_enable = true;
                Dashing = 0;
                OnFightDismiss();
            }
            if (Dash_able == true)
            {
                Dash_able = false;
                //Moving
                if (base.transform.position.x > Player.transform.position.x)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Dash_n_Dodge, 0, 0);
                }
                else if (base.transform.position.x < Player.transform.position.x)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(-Dash_n_Dodge, 0, 0);
                }
            }
        }
        else if (Cur_state == State.Dash_Forward)
        {
            float Dash_n_Dodge = 10;
            Animation_Update();
            rb.drag = 3.7f;
            Character_Collider.isTrigger = true;
            //Duration
            Change_state_enable = false;
            Dashing += Time.deltaTime;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Cur_state = State.Idle;
                Change_state_enable = true;
                Dashing = 0;
                OnFightDismiss();
            }
            if (Dash_able == true)
            {
                Dash_able = false;
                //Moving
                if (base.transform.position.x > Player.transform.position.x)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(-Dash_n_Dodge, 0, 0);
                }
                else if (base.transform.position.x < Player.transform.position.x)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Dash_n_Dodge, 0, 0);
                }
            }
        }
        else 
        {
            Character_Collider.isTrigger = false;
            rb.drag = 6f;
        }
        if (Cur_behavior == Move_behavior.Move_to_player)
        {
            if (Cur_Direction == Direction.Left)
            {
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(-Run_Speed * Time.deltaTime, 0, 0);
            }
            if (Cur_Direction == Direction.Right)
            {
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Run_Speed * Time.deltaTime, 0, 0);
            }
        }
        if (Cur_behavior == Move_behavior.Moveback)
        {
            if (base.transform.position.x > Player.transform.position.x)
            {
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Run_Speed * Time.deltaTime, 0, 0);
            }
            else if (base.transform.position.x < Player.transform.position.x)
            {
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(-Run_Speed * Time.deltaTime, 0, 0);
            }
        }
        Cur_behavior = Move_behavior.none;
    }
    private void This_Uptate()
    {
        if (Target.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Dash && Target.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Dodge)
        {
            Distance = Math.Abs(base.transform.position.x - Player.transform.position.x);
        }
        StateManagement();
        RoleManagement();
        //Calculate_Distance                

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
        if (base.transform.position.x > Player.transform.position.x && Change_state_enable == true)
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
        if (base.transform.position.x < Player.transform.position.x && Change_state_enable == true)
        {
            Cur_Direction = Direction.Right;
            animator.GetComponent<SpriteRenderer>().flipX = false;
            //Set_AttackBox
            Attack_Box.transform.localPosition = new Vector3(Math.Abs(Attack_Box.transform.localPosition.x),
                Attack_Box.transform.localPosition.y,
                Attack_Box.transform.localPosition.z);
            //Set_HittedBox
            Hitted_Box.transform.localPosition = new Vector3(Math.Abs(Hitted_Box.transform.localPosition.x),
                Hitted_Box.transform.localPosition.y,
                Hitted_Box.transform.localPosition.z);
        }
    }
    enum Move_behavior 
    {
        Move_to_player,
        Dash,
        Moveback,
        none
    }
    Move_behavior Cur_behavior = Move_behavior.none;
    protected void Move_to_player()
    {
        Cur_state = State.Run;        
        Animation_Update();
        Cur_behavior = Move_behavior.Move_to_player;
        
    }
    protected void Reteat()
    {
        
        if (Dash_CoolingDown <= 0 )
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                Dash(false);
            }
            else 
            {
                Dash(true);
            }
        }
        else
        {
            Cur_state = State.Run;
            Animation_Update();
            Cur_behavior = Move_behavior.Moveback;
        }
       
        
    }
    protected void MoveBack() 
    {
        if (front_Of_Enemy == front_of_enemy.Player)
        {
            Reteat();
        }
        else
        {
            Cur_state = State.Run;
            Animation_Update();
            Cur_behavior= Move_behavior.Moveback;
            
        }
    }
    protected void Dash(bool Dash_is_Forward)
    {
        if (Dash_is_Forward == false && Dash_able == true)
        {
            float Dash_Cooldown_Duration = 4;
            Cur_state = State.Dash_Back;
            Animation_Update();
            Dash_CoolingDown = Dash_Cooldown_Duration;
            Change_state_enable = false;
        }
        else if(Dash_is_Forward == true && Dash_able == true)
        {
            float Dash_Cooldown_Duration = 4;
            Cur_state = State.Dash_Forward;
            Animation_Update();
            Dash_CoolingDown = Dash_Cooldown_Duration;
            Change_state_enable = false;
        }
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
        float Pressure_Regen_Speed = 32;
        if (Pressure > 0)
        {
            if (Pressure < Under_Pressure_Approuching)
            {
                Pressure -= Pressure_Regen_Speed*1f * Time.deltaTime * Time.timeScale;
            }
            else if(Pressure >= Under_Pressure_Approuching && Pressure< Over_Pressure_Reteating)
            {
                Pressure -= Pressure_Regen_Speed*1f * Time.deltaTime * Time.timeScale;
            }
            else 
            {
                Pressure -= Pressure_Regen_Speed * Time.deltaTime * Time.timeScale;
            }
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
        else 
        {
            Dash_able = true;
        }
        //RandomDistanceCoolDown
        if (RandomCooldown > 0) 
        {
            RandomCooldown -= Time.deltaTime;
        }

    }
    private float Dashing = 0;
    private bool Regen_HP_for_Player = true;
    protected void StateManagement()
    {
        if (HP <= 0||Cur_state == State.Dead)
        {
            if (Regen_HP_for_Player == true)
            {
                Regen_HP_for_Player = false;
                Player.HP += 15;
                if (Player.HP > 100)
                {
                    Player.HP = 100;
                }
            }
            Cur_state = State.Dead;
            Animation_Update();                                   
        }
        else
        {
            LookatPlayer();
            //Dashing_Duration&Moving
           
            //Attack_Duration
            if (Cur_state == State.Attack_I)
            {
                float Time_preATK = 0.22f;
                float Time_ATKing = 0.33f;
                float Time_PostATK = 0.67f;
                Change_state_enable = false;
                //PreAttack_I
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) < (float)(Time_preATK/*/animator.GetCurrentAnimatorStateInfo(0).length) * (float)(60f / 100f)*/))
                {
                    Cur_Attack_State = Attack_State.Pre_Attack;
                }
                //Attack_I
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) >= (float)(Time_preATK /*/ animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)*/)
                    && animator.GetCurrentAnimatorStateInfo(0).normalizedTime* (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) < (float)(Time_ATKing /*/ animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)*/))
                {                   
                    Cur_Attack_State = Attack_State.Attacking;
                }
                //PostAttack_I
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) >= (float)(Time_ATKing /*/ animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)*/)
                    && animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)) < (float)(Time_PostATK /*/ animator.GetCurrentAnimatorStateInfo(0).length * (float)(60f / 100f)*/))
                {
                    Cur_Attack_State = Attack_State.Post_Attack;
                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >=1)
                {
                    OnFightDismiss(0.15f);
                    Change_state_enable = true;
                    if (UnityEngine.Random.value > 0.6f) 
                    {
                        Dash(true);
                    }
                    else if(UnityEngine.Random.value > 0.6f) 
                    {
                        Dash(false);
                    }
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
                //Counter_Attack
                if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f&&Counter_Enable == true) 
                {
                    Counter_Enable = false;
                    if (UnityEngine.Random.value <= 0.4f) 
                    {
                        Attack();
                    }
                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    Counter_Enable = true;
                    Change_state_enable = true;                   
                }
            }
            //Flinch
            if (Cur_state == State.Flinch)
            {                
                Change_state_enable = false;
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    Change_state_enable = true;                    
                }
            }
            if(Cur_state == State.Parried) 
            {               
                Change_state_enable = false;                                
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    Change_state_enable = true;                          
                }
            }
        }
       
    }  
    public void Animation_Update()
    {
        animator.Play(Cur_state.ToString());
    }
    public bool Hit_able = true;
    
    public void Got_Attacked() 
    {
        if (Cur_state != State.Dead)
        {
            OnFightDismiss(0.2f);
            if ((Cur_Direction == Direction.Left && (base.transform.position.x > Player.transform.position.x))
                || (Cur_Direction == Direction.Right && (base.transform.position.x < Player.transform.position.x)))
            {
                if (Defend > 0 && Cur_state != State.Parried)
                {
                    Cur_state = State.Block;
                    Animation_Update();
                }
                else if (Cur_state == State.Attack_I)
                {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) >= 0.5f && Cur_Attack_State == Attack_State.Post_Attack)
                    {
                        Cur_state = State.Block;
                        Animation_Update();
                    }
                }
            }

            if (Cur_state == State.Block)
            {
                Defend -= Player.DMG * 0.7f;
                Pressure += 15;
                Push(3.0f);
            }
            else
            {
                if (Cur_state == State.Parried)
                {
                    Pressure += 40;
                    HP -= Player.DMG * 1.5f;
                }
                else
                {
                    if (UnityEngine.Random.Range(1, 10) <= 2)
                    {
                        //Critical
                        HP -= Player.DMG * 1.5f;
                        Debug.Log("Critical_Hit:" + Player.DMG * 1.5f);
                    }
                    else
                    {
                        HP -= Player.DMG;
                        Debug.Log("Hit:" + Player.DMG);
                    }
                }
                Cur_state = State.Flinch;
                Animation_Update();
                Pressure += 10;
                Push(3.0f);

            }
        }
    }
    public void Got_Parried() 
    {
        if (Cur_state != State.Dead)
        {
            Cur_state = State.Parried;
            Animation_Update();
            OnFightDismiss();
        }
    }
    public void Push(float force) 
    {
        if (base.transform.position.x > Player.transform.position.x)
        {
            rb.GetComponent<Rigidbody>().velocity = new Vector3(force, 0, 0);
        }
        if (base.transform.position.x < Player.transform.position.x)
        {
            rb.GetComponent<Rigidbody>().velocity = new Vector3(-force, 0, 0);
        }
    }
    
    private float[] DistanceMove = new float[4];
    private float Timing_Set_Active = 2;
    private void RoleManagement() 
    {
        if (Cur_state != State.Dead)
        {
            RoleOnfight();
            RoleAroundfight();
        }
        else //Mean Cur_state == Dead
        {
            Cur_Role = Role.OffFight;            
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            Hitted_Box.gameObject.SetActive(false);
            gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            Timing_Set_Active -= Time.deltaTime;
            if(Timing_Set_Active <= 0) 
            {
                gameObject.SetActive(false);
                
            }
        }
    }
    private void RoleOnfight()
    {
        //Role_Onfight //****
        if (Cur_Role == Role.OnFight)
        {
            Run_Speed = Run_Speed_Onfight;
            if (Cur_Direction == Direction.Left)
            {
                if (Physics.Raycast(new Vector3(transform.position.x - (gameObject.GetComponent<CapsuleCollider>().radius / 2f), transform.position.y - 0.5f, transform.position.z), new Vector3(-1, 0, 0), out RaycastHit HitLeft, 600, 3))
                {
                    if (HitLeft.rigidbody.tag == "Enemy")
                    {
                        front_Of_Enemy = front_of_enemy.Enemy;
                    }
                    if (HitLeft.rigidbody.tag == "Player")
                    {
                        front_Of_Enemy = front_of_enemy.Player;
                    }
                }
            }
            if (Cur_Direction == Direction.Right)
            {
                if (Physics.Raycast(new Vector3(transform.position.x + (gameObject.GetComponent<CapsuleCollider>().radius / 2f), transform.position.y - 0.5f, transform.position.z), new Vector3(1, 0, 0), out RaycastHit HitRight, 600, 3))
                {
                    if (HitRight.rigidbody.tag == "Enemy")
                    {
                            front_Of_Enemy = front_of_enemy.Enemy;
                    }
                    if (HitRight.rigidbody.tag == "Player")
                    {
                            front_Of_Enemy = front_of_enemy.Player;
                    }    
                }                
            }               
            if (front_Of_Enemy != front_of_enemy.Player)                
            {                   
                OnFightDismiss();               
            }                
            if (Pressure < Under_Pressure_Approuching && Distance > ATK_Range && Change_state_enable == true)                
            {                 
                Move_to_player();              
            }                
            //Reteat_Condition                       
            else if (Pressure > Over_Pressure_Reteating && Distance < Under_ATK_Range_Reteating && Change_state_enable == true)               
            {                 
                Reteat();               
            }                
            //Dash_Forward               
            else if(Convert.ToInt32(Distance)==Convert.ToInt32(ATK_Range)&& Pressure < Under_Pressure_Approuching && Change_state_enable == true
                && Dash_Check_able == true)            
            {
                Dash_Check_able = false;
                if (UnityEngine.Random.value > 0.7)
                {
                    Dash(true);
                }
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
                Dash_Check_able = true;
            }           
        }
    }
    bool Dash_Check_able = true;
    public enum front_of_enemy {Enemy,Player,None };
    public front_of_enemy front_Of_Enemy;
    private void RoleAroundfight() 
    {      
        //Role_Aroundfight //****
        if (Cur_Role == Role.AroundFight)
        {
            Run_Speed = Run_Speed_Offfight;
            if (Cur_Direction == Direction.Left)
            {
                if (Physics.Raycast(new Vector3(transform.position.x - (gameObject.GetComponent<CapsuleCollider>().radius/2f), transform.position.y - 0.5f, transform.position.z), new Vector3(-1,0, 0), out RaycastHit HitLeft, 600, 3))
                {
                    if(HitLeft.rigidbody.tag == "Enemy") 
                    {
                        front_Of_Enemy = front_of_enemy.Enemy;
                    }
                    if(HitLeft.rigidbody.tag == "Player") 
                    {
                        front_Of_Enemy = front_of_enemy.Player;
                    }
                    if (math.abs(HitLeft.rigidbody.velocity.x) > 1.5f)
                    {
                        RayDistance = HitLeft.distance;
                    }
                    else if (HitLeft.distance > 3)
                    {
                        RayDistance = HitLeft.distance;
                    }
                    RayDistance = HitLeft.distance;
                    //Raycast Enemy
                    if (RayDistance < DistanceMove[0] && HitLeft.rigidbody.tag == "Enemy" && Change_state_enable == true)
                    {                       
                        RandomDistance();
                        MoveBack();                       
                    }
                    else if (RayDistance >= DistanceMove[1] && HitLeft.rigidbody.tag == "Enemy" && Change_state_enable == true)
                    {
                        RandomDistance();
                        Move_to_player();
                    }
                    //Raycast Player                    
                    else if (RayDistance >= DistanceMove[2] && HitLeft.rigidbody.tag == "Player" && Change_state_enable == true)
                    {
                        RandomDistance();
                        Move_to_player();
                    }
                    else if(RayDistance <= DistanceMove[3] && HitLeft.rigidbody.tag == "Player" && Change_state_enable == true&& Pressure > 0) 
                    {
                        RandomDistance();
                        MoveBack();
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
                if (Physics.Raycast(new Vector3(transform.position.x + (gameObject.GetComponent<CapsuleCollider>().radius / 2f), transform.position.y - 0.5f, transform.position.z), new Vector3(1, 0, 0), out RaycastHit HitRight, 600, 3))
                {
                    if (HitRight.rigidbody.tag == "Enemy")
                    {
                        front_Of_Enemy = front_of_enemy.Enemy;
                    }
                    if (HitRight.rigidbody.tag == "Player")
                    {
                        front_Of_Enemy = front_of_enemy.Player;
                    }

                  
                    //Get RayDistance
                    if (math.abs(HitRight.rigidbody.velocity.x) > 1.5f) 
                    {
                        RayDistance = HitRight.distance;
                    }
                    else if(HitRight.distance > 3) 
                    {
                        RayDistance = HitRight.distance;
                    }
                    RayDistance = HitRight.distance;
                    //Raycast Enemy
                    if (RayDistance < DistanceMove[0] && HitRight.rigidbody.tag == "Enemy" && Change_state_enable == true)
                    {
                        RandomDistance();
                        MoveBack();                        
                    }
                    else if (RayDistance >= DistanceMove[1] && HitRight.rigidbody.tag == "Enemy" && Change_state_enable == true)
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
                    else if (RayDistance >= DistanceMove[2] && HitRight.rigidbody.tag == "Player" && Change_state_enable == true)
                    {
                        RandomDistance();
                        Move_to_player();
                    }
                    else if (RayDistance <= DistanceMove[3] && HitRight.rigidbody.tag == "Player" && Change_state_enable == true && Pressure > 0)
                    {
                        RandomDistance();
                        MoveBack();
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
            DistanceMove[0] = (float)(UnityEngine.Random.Range(100, 300)/100f);      
            //DistanceMoveForward CheckwithEnemy       
            DistanceMove[1] = DistanceMove[0] + 0.5f;      
            //DistanceMoveForward ChecwithPlayer       
            DistanceMove[2] = (float)(UnityEngine.Random.Range(200, 310)/100f);
            //DistanceMoveBack CheckwithPlayer
            DistanceMove[3] = DistanceMove[2] - 0.5f;
        }
    }
    private void OnFightDismiss() 
    {
        Cur_Role = Role.AroundFight;
        
    }
    private void OnFightDismiss(float RandomChance)
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) >= RandomChance)
        {
            Cur_Role = Role.AroundFight;
        }

    }


    public void UpdateState(State state) 
    {
        Cur_state = state;
        Animation_Update();
    }



}
