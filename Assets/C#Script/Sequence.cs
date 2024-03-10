using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : MonoBehaviour
{
    public enum Sequence_Line 
    {
        Chapter1_Part_1, Chapter1_Part_2,
        Chapter2_Part_1, Chapter2_Part_2,Chapter2_Part3,Chapter2_Part4,
        Chapter3_Part_1, Chapter3_Part_2,
    }
    public Sequence_Line Current_Sequence = Sequence_Line.Chapter1_Part_1;
    public List<GameObject> Enemys;
    public GameObject MainCharacter;
    public GameObject Enemy_Spawner_L;
    public GameObject Enemy_Spawner_R;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Current_Sequence == Sequence_Line.Chapter1_Part_1) 
        {
            if(Enter_Chapter == true) 
            {
                Enter_Chapter = false;
                Spawn_Enemy(0,1);
            }
        }
    }
   
    bool Enter_Chapter = true;
    public void Chapter_End() 
    { 
    }
    private void Spawn_Enemy(int Num_L,int Num_R) 
    {
        for(int i = 0; i < Num_R; i++) 
        {
            Enemys[i].GetComponent<Enemy_Common>().Spawn(Enemy_Spawner_R.transform.position,50,20);
        }
        for(int i = 0;i < Num_L; i++) 
        {
            
        }
    }
}
