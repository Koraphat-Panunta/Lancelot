using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Director : MonoBehaviour
{
    public List<GameObject> Enemy = new List<GameObject>();
    public int OnFight_num = 0;
    public int AroundFight_num = 0;
    public void AddEnemy(GameObject gameObject) 
    {
        Enemy.Add(gameObject);
    }
    public void Update()
    {
        OnFight_num = 0;
        AroundFight_num = 0;
        foreach (GameObject Enemy in Enemy) 
        {
            if(Enemy.GetComponent<Enemy_Common>().Cur_state == global::Enemy.State.Dead) 
            {
                this.Enemy.Remove(Enemy);
            }
            if (Enemy.GetComponent<Enemy_Common>().Cur_Role == global::Enemy.Role.OnFight)
            {
                OnFight_num += 1;
            }
            if (Enemy.GetComponent<Enemy_Common>().Cur_Role == global::Enemy.Role.AroundFight)
            {
                AroundFight_num += 1;
            }
        }
    }
    
    
}
