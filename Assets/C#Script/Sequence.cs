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
    [SerializeField] private Sequence_Line Current_Sequence ;
    
    [SerializeField] private List<GameObject> Enemys;
    [SerializeField] private GameObject MainCharacter;
    [SerializeField] private GameObject Enemy_Spawner_L;
    [SerializeField] private GameObject Enemy_Spawner_R;
    [SerializeField] protected bool Collider_enable = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if(Current_Sequence == Sequence_Line.Chapter1_Part_1) 
        {
            if(Enter_Chapter == true) 
            {
                Enter_Chapter = false;
                Spawn_Enemy(0,1);
            }
        }
        else if(Current_Sequence == Sequence_Line.Chapter1_Part_2) 
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(0, 2);
            }
        }
        else if(Current_Sequence == Sequence_Line.Chapter2_Part_1) 
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(1, 2);
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter2_Part_2)
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(2, 3);
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter2_Part3)
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(3, 3);
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter2_Part4)
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(2, 3);
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter3_Part_1)
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
               
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter3_Part_2)
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(5, 5);
            }
        }
        if (index >= Enemys.Count) 
        {
            index = 0;
        }
    }
   
    public bool Enter_Chapter = true;
    public void Chapter_End() 
    {
        
    }
    
    [SerializeField] int index = 0;
    private void Spawn_Enemy(int Num_L,int Num_R) 
    {
        
        for(int i = 0; i < Num_R; i++) 
        {
            Enemys[index].GetComponent<Enemy_Common>().Spawn(Enemy_Spawner_R.transform.position, 50, 20);
            index++;
        }
        for(int i = 0;i < Num_L; i ++) 
        {
            Enemys[index].GetComponent<Enemy_Common>().Spawn(Enemy_Spawner_L.transform.position, 50, 20);
            index++;
        }
    }
    public void SetCur_Sequence(Sequence_Line Set_Sequence) 
    {
        Current_Sequence = Set_Sequence;
    }
    public void Set_List_Enemy(List<GameObject> Enemy) 
    {
        this.Enemys = Enemy;
    }
}
