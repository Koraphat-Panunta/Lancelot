using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public GameObject Player;
    public List<GameObject> Enemy;
    [SerializeField] private Enemy_Director Director;
    public Camera Camera;
    private Vector3 StartPos;
    void Start()
    {
        Setup_Camera();
        Enemy = Director.Enemy;       
        //EnemySpawner        

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
       
    }
    private void Update()
    {
        CombatSystem();
        CameraTransformation();
        if (Slow_Duration > 0) 
        {
            Slow_Duration -= Time.deltaTime*(1f/Time.timeScale);
            if (Slow_Duration <= 0) 
            {
                Time.timeScale = 1f;
                
                Time.fixedDeltaTime = (float)(1f / 60f)*Time.timeScale;
                //Camera.GetComponent<DepthOfField>().focusDistance.Override(Orginal_FocusDistance);
                //Camera.GetComponent<DepthOfField>().aperture.Override(Orginal_aperture);
                //Camera.GetComponent<DepthOfField>().focalLength.Override(Orginal_focalLength);
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
                if (Player.GetComponent<Main_Char>().Attack_Box.bounds.Intersects(enemy.GetComponent<Enemy_Common>().Hitted_Box.bounds))
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
                if (enemy.GetComponent<Enemy_Common>().Attack_Box.bounds.Intersects(Player.GetComponent<Main_Char>().Hitted_Box.bounds))
                {
                    Player.GetComponent<Main_Char>().GotAttack(enemy);
                    if(Player.GetComponent<Main_Char>().Cur_state == Main_Char.Char_state.Parry) 
                    {
                        enemy.GetComponent<Enemy_Common>().Cur_state = global::Enemy.State.Parried;
                        Parry_Effect(1);
                        enemy.GetComponent<Enemy_Common>().Animation_Update();
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
    private float Orginal_FocusDistance;
    private float Orginal_aperture;
    private float Orginal_focalLength;
    
    public void Parry_Effect(float Duration) 
    {
        Time.timeScale = 0.35f;
        Time.fixedDeltaTime = Time.timeScale*(float)(1f/60f);
        Slow_Duration = Duration;
        this.Camera.focusDistance = 10f;
        this.Camera.focalLength = 182f;
        this.Camera.aperture = 15f;

        Camera.GetComponent<PostProcessVolume>().GetComponent<DepthOfField>().focusDistance.value = 10f;
        Camera.GetComponent<PostProcessVolume>().GetComponent<DepthOfField>().aperture.value = 15f;
        Camera.GetComponent<PostProcessVolume>().GetComponent<DepthOfField>().focalLength.value = 182f;
    }
    
}
