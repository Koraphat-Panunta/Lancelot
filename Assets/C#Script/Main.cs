using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    void Update()
    {
        CombatSystem();
        CameraTransformation();       
    }

    /////BackEnd
    private void Setup_Camera() 
    {
        //SetupCam
        StartPos = new Vector3(Camera.transform.position.x - Player.transform.position.x,
            Camera.transform.position.y - Player.transform.position.y,
            Camera.transform.position.z - Player.transform.position.z);
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
                }
            }
            if (enemy.GetComponent<Enemy_Common>().Cur_Attack_State != Enemy_Common.Attack_State.Attacking)
            {
                enemy.GetComponent<Enemy_Common>().Hit_able = true;
            }
        }
       
    }
    
}
