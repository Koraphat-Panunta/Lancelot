using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy_Director : MonoBehaviour
{
    public List<GameObject> Enemy = new List<GameObject>();
    public Main Game_manager;
    public int OnFight_num = 0;
    public int AroundFight_num = 0;
    public float Lowest_Distance_Enemy = 1000;
    public int Enemy_front;
    public int List_Enemy;
    private void Start()
    {
        List_Enemy = Enemy.Count;

    }
    public void AddEnemy(GameObject gameObject) 
    {
        Enemy.Add(gameObject);
    }
    bool israndom = false;
    
    float Onfight_Count = 0;
    [SerializeField] int GameOBG_ID_Last_Onfight = 0;
    public void Update()
    {
        OnFight_num = 0;
        AroundFight_num = 0;
        Enemy_front = 0;
        Lowest_Distance_Enemy = 1000;
        foreach (GameObject Enemy in Enemy) 
        {           
            if(Enemy.GetComponent<Enemy_Common>().Cur_state == global::Enemy.State.Dead) 
            {
                //this.Enemy.Remove(Enemy);
            }
            else 
            {
                if (Enemy.GetComponent<Enemy_Common>().Distance < Lowest_Distance_Enemy) 
                {
                    Lowest_Distance_Enemy = Enemy.GetComponent<Enemy_Common>().Distance;
                } 
                if (Enemy.GetComponent<Enemy_Common>().Cur_Role == global::Enemy.Role.OnFight)
                {
                    OnFight_num += 1;
                }
                if (Enemy.GetComponent<Enemy_Common>().Cur_Role == global::Enemy.Role.AroundFight)
                {
                    AroundFight_num += 1;
                }
                if(Enemy.GetComponent<Enemy_Common>().front_Of_Enemy == global::Enemy.front_of_enemy.Player) 
                {
                    Enemy_front += 1;
                }
                if ((Enemy.GetComponent<Enemy_Common>().Cur_state == global::Enemy.State.Flinch
                    || Enemy.GetComponent<Enemy_Common>().Cur_state == global::Enemy.State.Block)
                    && (Enemy.GetComponent<Enemy_Common>().Cur_Role == global::Enemy.Role.AroundFight))
                {
                    if (israndom == false)
                    {
                        israndom = true;
                        int num = Random.Range(1, 100);
                        if (num > 50)
                        {
                            Reset_To_AroundFight();
                            Enemy.GetComponent<Enemy_Common>().Cur_Role = global::Enemy.Role.OnFight;
                        }
                    }
                }
            }                        
        }
       
        if(OnFight_num == 0) 
        {
            Onfight_Count += Time.deltaTime;
            if (Onfight_Count >= Random.Range(0.1f,0.68f))
            {
                Onfight_Count = 0;
                foreach (GameObject Enemy in Enemy)
                {
                    GameOBG_ID_Last_Onfight = Enemy.GetInstanceID();
                    if (Enemy.GetComponent<Enemy_Common>().front_Of_Enemy == global::Enemy.front_of_enemy.Player && Random.Range(1, 10) > 5
                        && Enemy.GetComponent<Enemy_Common>().Cur_state != global::Enemy.State.Dead)
                    {
                        Enemy.GetComponent<Enemy_Common>().Cur_Role = global::Enemy.Role.OnFight;
                        OnFight_num += 1;
                        break;
                    }
                }
            }
            
        }
        if(OnFight_num > 1) 
        {
            OnFight_num = 0;
            Reset_To_AroundFight();
        }
        
    }
    
    private void Reset_To_AroundFight()
    {
        foreach (GameObject Enemy in Enemy) 
        {
            if (Enemy.GetComponent<Enemy_Common>().Cur_state != global::Enemy.State.Dead)
            {
                Enemy.GetComponent<Enemy_Common>().Cur_Role = global::Enemy.Role.AroundFight;
            }
        }
    }
    



}
