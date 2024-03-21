using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{

    public GameObject Player;
    public List<GameObject> Enemy;
    [SerializeField] private Enemy_Director Director;
    public Camera Camera;
    [SerializeField] private GameObject Camera_Pos;
    public GameObject Volume;
    public VolumeProfile VolumeProfile;
    public Vignette vignette;
    private bool Effect_is_On = false;

    public enum Dificulty
    {
        Normal,
        Hard,
    }
    static public Dificulty dificulty;

    void Start()
    {
        Vignette Vg;
        Enemy = Director.Enemy;
        GetComponent<Sequence>().Set_List_Enemy(Enemy);
        if (Volume.GetComponent<Volume>().profile.TryGet<Vignette>(out Vg))
        {
            vignette = Vg;
        }
        Application.targetFrameRate = 60;
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame

    private void Update()
    {
        CombatSystem();
        CameraTransformation();
        Haptic_system();
    }

    /////BackEnd       
    Vector3 Original_Cam_Pos;
    private void CameraTransformation()
    {
        //Parry_Effect
        if (Slow_Duration > 0)
        {
            Slow_Duration -= Time.deltaTime * (1f / Time.timeScale);
            if (Player.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Parry)
            {
                Slow_Duration = 0;
            }
            if (Slow_Duration <= 0)
            {
                Time.timeScale = 1f;
                Effect_is_On = false;
                Time.fixedDeltaTime = (float)(1f / 60f) * Time.timeScale;
            }
        }
        if (Effect_is_On == true)
        {
            if (vignette.intensity.value < 0.35f)
            {
                vignette.intensity.value += 5 * Time.deltaTime;
            }
            if (Camera.transform.position.z < 14.2f)
            {
                Camera.transform.position += new Vector3(0, 0, 0.6f * Time.deltaTime * ((float)(1f / Time.timeScale)));
                if (Camera.transform.position.z > 14.2f)
                {
                    Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, 14.2f);
                }
            }
        }
        else
        {
            if (vignette.intensity.value > 0)
            {
                vignette.intensity.value -= 5 * Time.deltaTime;
            }
            if (Camera.transform.position.z > 13.7f)
            {
                Camera.transform.position -= new Vector3(0, 0, 1.5f * Time.deltaTime);
                if (Camera.transform.position.z < 13.7f)
                {
                    Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, 13.7f);
                }
            }

        }
        //Shake
        if (Shake_Duration > 0 && Effect_is_On == false) 
        {
            Camera.transform.position = Original_Cam_Pos;            
            Camera.transform.position = new Vector3(Camera.transform.position.x + (Random.Range(-1, 1) * Magnitude), Camera.transform.position.y + (Random.Range(-1, 1) * Magnitude), Camera.transform.position.z);
            if(Shake_Duration > 0) 
            {
                Shake_Duration -= Time.deltaTime * (1f / Time.timeScale);
            }
           
            if(Shake_Duration <= 0) 
            {
                Shake_Duration = 0;               
               
            }
        }
        else 
        {
            Camera.transform.position = Vector3.Lerp(Camera.transform.position, Camera_Pos.transform.position, 2f * Time.deltaTime);
            Original_Cam_Pos = Camera.transform.position;
        }
    }
    static float Shake_Duration = 0;
    static float Magnitude = 0;
    
    static public void CameraShake(float Duration,float intensity) 
    {         
        Shake_Duration = Duration;
        Magnitude = intensity;
    }
    static float Rumble_Duration = 0;
    static float frequency_R = 0,frequency_L =0;
    
    static public void Haptic_feedback_Input(float Duration,float frequency_l,float frequency_r ) 
    {
        Rumble_Duration = Duration;
        frequency_R = frequency_r;
        frequency_L = frequency_l;
    }
    private void Haptic_system() 
    {
        if(Rumble_Duration > 0) 
        {
            Gamepad.current.SetMotorSpeeds(frequency_L, frequency_R);
            Rumble_Duration -= Time.deltaTime;
        }
        else 
        {
            Rumble_Duration = 0;
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    } 
   
    public void CombatSystem() 
    {
        foreach (GameObject enemy in Enemy)
        {
            if (enemy.GetComponent<Enemy_Common>().Cur_state != global::Enemy.State.Dead && enemy.GetComponent<Enemy_Common>().Cur_Role != global::Enemy.Role.OffFight)
            {
                //Player_Attack_Enemy
                if (Player.GetComponent<Main_Char>().Cur_Attack_State == Main_Char.Attack_state.Attacking && enemy.GetComponent<Enemy_Common>().GetHitted_able == true
                    )
                {
                    if (Player.GetComponent<Main_Char>().Attack_Box.bounds.Intersects(enemy.GetComponent<Enemy_Common>().Hitted_Box.bounds)
                        && enemy.GetComponent<Enemy_Common>().Cur_state != global::Enemy.State.Dash_Forward && enemy.GetComponent<Enemy_Common>().Cur_state != global::Enemy.State.Dash_Back
                        )
                    {
                        enemy.GetComponent<Enemy_Common>().Got_Attacked();                       
                        enemy.GetComponent<Enemy_Common>().GetHitted_able = false;
                        if(enemy.GetComponent<Enemy_Common>().Cur_state == global::Enemy.State.Flinch) 
                        {
                            CameraShake(0.06f, 0.08f);
                            Main.Haptic_feedback_Input(0.02f, 0.2f, 0.5f);
                        }
                        else if(enemy.GetComponent<Enemy_Common>().Cur_state == global::Enemy.State.Block) 
                        {
                            CameraShake(0.02f, 0.01f);
                            Main.Haptic_feedback_Input(0.01f, 0.7f, 0.5f);
                        }
                    }
                }
                if (Player.GetComponent<Main_Char>().Cur_Attack_State != Main_Char.Attack_state.Attacking)
                {
                    enemy.GetComponent<Enemy_Common>().GetHitted_able = true;
                }
                //Enemy_Attack_Player
                if (enemy.GetComponent<Enemy_Common>().Cur_Attack_State == Enemy_Common.Attack_State.Attacking && enemy.GetComponent<Enemy_Common>().Hit_able == true)
                {
                    enemy.GetComponent<Enemy_Common>().Hit_able = false;
                    if (enemy.GetComponent<Enemy_Common>().Attack_Box.bounds.Intersects(Player.GetComponent<Main_Char>().Hitted_Box.bounds) &&
                        Player.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Dash && Player.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Dodge)
                    {
                        Player.GetComponent<Main_Char>().GotAttack(enemy);
                        if (Player.GetComponent<Main_Char>().Cur_state == Main_Char.Char_state.Parry)
                        {
                            enemy.GetComponent<Enemy_Common>().Got_Parried();
                            Parry_Effect(1);
                            Main.Haptic_feedback_Input(0.05f, 0.04f, 0.01f);
                        }
                        else if(Player.GetComponent<Main_Char>().Cur_state == Main_Char.Char_state.Flinch) 
                        {
                            CameraShake(0.25f, 0.07f);
                            Main.Haptic_feedback_Input(0.3f, 1f, 1f);
                        }
                        else if (Player.GetComponent<Main_Char>().Cur_state == Main_Char.Char_state.BlockReact)
                        {
                            CameraShake(0.02f, 0.1f);
                            Main.Haptic_feedback_Input(0.06f, 0.7f, 0f);
                        }
                    }
                }
                if (enemy.GetComponent<Enemy_Common>().Cur_Attack_State != Enemy_Common.Attack_State.Attacking)
                {
                    enemy.GetComponent<Enemy_Common>().Hit_able = true;
                }
            }
        }
       
    }
    public void RemoveEnemy(GameObject Enemy) 
    {
        this.Enemy.Remove(Enemy);
    }
    public float Slow_Duration = 0;
   
    
    public void Parry_Effect(float Duration) 
    {
        Time.timeScale = 0.35f;
        Time.fixedDeltaTime = Time.timeScale*(float)(1f/60f);
        Slow_Duration = Duration;
        
        Effect_is_On = true;
              
    }
    
    
}
