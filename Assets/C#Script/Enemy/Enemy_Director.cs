using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy_Director : MonoBehaviour
{
    public List<GameObject> Enemy = new List<GameObject>();
    public int OnFight_num = 0;
    public int AroundFight_num = 0;
    public int Enemy_ID_Onfight;
    public float Lowest_Distance_Enemy = 1000;
    
    public void AddEnemy(GameObject gameObject) 
    {
        Enemy.Add(gameObject);
    }
    bool israndom = false;
    float randomCooldown  =0;
    public void Update()
    {
        OnFight_num = 0;
        AroundFight_num = 0;
        Lowest_Distance_Enemy = 1000;
        foreach (GameObject Enemy in Enemy) 
        {
            if(Enemy.GetComponent<Enemy_Common>().Cur_state == global::Enemy.State.Dead) 
            {
                this.Enemy.Remove(Enemy);                
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
                if((Enemy.GetComponent<Enemy_Common>().Cur_state == global::Enemy.State.Flinch
                    || Enemy.GetComponent<Enemy_Common>().Cur_state == global::Enemy.State.Block)
                    &&(Enemy.GetComponent<Enemy_Common>().Cur_Role == global::Enemy.Role.AroundFight)) 
                {
                    if (israndom == false)
                    {
                        israndom = true;
                        int num = UnityEngine.Random.Range(1, 100);
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
            foreach(GameObject Enemy in Enemy) 
            {
                if(Enemy.GetComponent<Enemy_Common>().Distance <= Lowest_Distance_Enemy) 
                {
                    Enemy.GetComponent<Enemy_Common>().Cur_Role = global::Enemy.Role.OnFight;
                    OnFight_num += 1;
                    break;
                }
            }
        }
        if(OnFight_num > 1) 
        {
            OnFight_num = 0;
            Reset_To_AroundFight();
        }
        if(israndom == true) 
        {
            randomCooldown += Time.deltaTime;
            if(randomCooldown >= 1) 
            {
                israndom = false;
                randomCooldown = 0;
            }
        }
    }
    
    private void Reset_To_AroundFight()
    {
        foreach (GameObject Enemy in Enemy) 
        {
            Enemy.GetComponent<Enemy_Common>().Cur_Role = global::Enemy.Role.AroundFight;
        }
    }


}
