using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class Main : MonoBehaviour
{
    
    public GameObject Player;
    public List<GameObject> Enemy;
    [SerializeField] private Enemy_Director Director;
    public Camera Camera;
    private Vector3 StartPos;
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
        Setup_Camera();
        Enemy = Director.Enemy;
        GetComponent<Sequence>().Set_List_Enemy(Enemy);
        if(Volume.GetComponent<Volume>().profile.TryGet<Vignette>(out Vg)) 
        {
            vignette = Vg;
        }       
        Application.targetFrameRate = 60;
        
    }

    // Update is called once per frame
   
    private void Update()
    {
        CombatSystem();
        CameraTransformation();
        if (Slow_Duration > 0) 
        {
            Slow_Duration -= Time.deltaTime*(1f/Time.timeScale);
            if(Player.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Parry) 
            {
                Slow_Duration = 0;
            }
            if (Slow_Duration <= 0) 
            {
                Time.timeScale = 1f;
                Effect_is_On = false;
                Time.fixedDeltaTime = (float)(1f / 60f)*Time.timeScale;               
            }            
        }
        if(Effect_is_On == true) 
        {
            if (vignette.intensity.value < 0.35f) 
            {
                vignette.intensity.value += 5*Time.deltaTime;
            }
            if (Camera.transform.position.z < 14.2f) 
            {
                Camera.transform.position += new Vector3(0, 0, 0.6f * Time.deltaTime*((float)(1f/Time.timeScale)));
                if (Camera.transform.position.z > 14.2f)
                {
                    Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, 14.2f);
                }
            }
        }
        else 
        { 
            if(vignette.intensity.value > 0) 
            {
                vignette.intensity.value -=  5*Time.deltaTime;
            }
            if (Camera.transform.position.z > 13.7f)
            {
                Camera.transform.position -= new Vector3(0, 0, 1.5f * Time.deltaTime);
                if(Camera.transform.position.z < 13.7f) 
                {
                    Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, 13.7f);
                }
            }

        }
    }

    /////BackEnd
    private void Setup_Camera() 
    {
        //SetupCam
        StartPos = new Vector3(Camera.transform.position.x - Player.transform.position.x,
            Camera.transform.position.y - Player.transform.position.y,
            Camera.transform.position.z - Player.transform.position.z);
        //Set_Original_PostProcessing
       
    } 
    private void CameraTransformation() 
    {
        Camera.transform.position = Vector3.Lerp(Camera.transform.position,
            new Vector3(StartPos.x + Player.transform.position.x, Camera.transform.position.y, Camera.transform.position.z)
            , 2f * Time.deltaTime);
    }
    public void CombatSystem() 
    {
        foreach (GameObject enemy in Enemy) 
        {
            //Player_Attack_Enemy
            if (Player.GetComponent<Main_Char>().Cur_Attack_State == Main_Char.Attack_state.Attacking && enemy.GetComponent<Enemy_Common>().GetHitted_able == true)
            {
                if (Player.GetComponent<Main_Char>().Attack_Box.bounds.Intersects(enemy.GetComponent<Enemy_Common>().Hitted_Box.bounds)
                    &&enemy.GetComponent<Enemy_Common>().Cur_state != global::Enemy.State.Dash_Forward&& enemy.GetComponent<Enemy_Common>().Cur_state != global::Enemy.State.Dash_Back)
                {
                    enemy.GetComponent<Enemy_Common>().Got_Attacked();
                    enemy.GetComponent<Enemy_Common>().GetHitted_able = false;
                   
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
                if (enemy.GetComponent<Enemy_Common>().Attack_Box.bounds.Intersects(Player.GetComponent<Main_Char>().Hitted_Box.bounds)&& 
                    Player.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Dash&& Player.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Dodge)
                {
                    Player.GetComponent<Main_Char>().GotAttack(enemy);
                    if(Player.GetComponent<Main_Char>().Cur_state == Main_Char.Char_state.Parry) 
                    {
                        enemy.GetComponent<Enemy_Common>().Got_Parried();                       
                        Parry_Effect(1);                       
                    }
                }
            }
            if (enemy.GetComponent<Enemy_Common>().Cur_Attack_State != Enemy_Common.Attack_State.Attacking)
            {
                enemy.GetComponent<Enemy_Common>().Hit_able = true;
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
