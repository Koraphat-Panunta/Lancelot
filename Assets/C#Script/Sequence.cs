using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tutorial_Sequence;

public class Sequence : MonoBehaviour
{
    public enum Sequence_Line 
    {
        Chatper0_Part_1, 
        Chapter1_Part_1, Chapter1_Part_2,
        Chapter2_Part_1, Chapter2_Part_2,Chapter2_Part3,Chapter2_Part4,
        Chapter3_Part_1, Chapter3_Part_2,
    }
    [SerializeField] public Sequence_Line Current_Sequence ;
    [SerializeField] private Tutorial_Sequence Tutorial ;
    [SerializeField] private List<GameObject> Enemys;
    [SerializeField] private GameObject MainCharacter;
    [SerializeField] private GameObject Enemy_Spawner_L;
    [SerializeField] private GameObject Enemy_Spawner_R;
    [SerializeField] protected bool Collider_enable = true;
    [SerializeField] GameObject[] Dialog;
    [SerializeField] GameObject Player;
    public bool GameisEnd = false;
    void Start()
    {
        Tutorial.Tutorial = Tutorial_Sequence.tutorial.Movement;        
    }

    // Update is called once per frame
    private void Update()
    {
        Count = 0;
        //Tutorial
        if (Current_Sequence == Sequence_Line.Chatper0_Part_1) 
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;   
                Tutorial.StopTime();
            }
        }       
        //Tutorial_Combat
        else if(Current_Sequence == Sequence_Line.Chapter1_Part_1) 
        {
            if(Enter_Chapter == true) 
            {
                Enter_Chapter = false;
                Spawn_Enemy(0,1);
            }            
            if(Tutorial.Tutorial == Tutorial_Sequence.tutorial.Block ) 
            {
                foreach(GameObject enemy in Enemys) 
                {
                    if(enemy.GetComponent<Enemy_Common>().Distance < 2.1f&&enemy.GetComponent<Enemy_Common>().Cur_Role == Enemy.Role.OnFight && Tutorial.FristTime_Block == true && MainCharacter.GetComponent<Main_Char>().Change_Behavior_enable == true) 
                    {
                        Tutorial.StopTime();                       
                    }
                    if(enemy.GetComponent<Enemy_Common>().Cur_Attack_State == Enemy.Attack_State.Post_Attack) 
                    {
                        Tutorial.Tutorial = Tutorial_Sequence.tutorial.Parry;
                    }
                }
            }
            if (Tutorial.Tutorial == Tutorial_Sequence.tutorial.Parry && MainCharacter.GetComponent<Main_Char>().Change_Behavior_enable == true)
            {
                foreach (GameObject enemy in Enemys)
                {
                    if (enemy.GetComponent<Enemy_Common>().Cur_Attack_State == Enemy.Attack_State.Pre_Attack && enemy.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime>0.23f
                        &&Tutorial.FristTime_Parry==true&&(MainCharacter.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Block|| MainCharacter.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.BlockReact))
                    {                        
                        Tutorial.StopTime();
                    }
                }
            }
            if(Tutorial.Tutorial == Tutorial_Sequence.tutorial.Attack ) 
            {
                foreach (GameObject enemy in Enemys)
                {
                    if (enemy.GetComponent<Enemy_Common>().Cur_state == Enemy.State.Parried && enemy.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f 
                        && Tutorial.FristTime_ATK == true && MainCharacter.GetComponent<Main_Char>().Cur_state == Main_Char.Char_state.Parry)
                    {
                        Tutorial.StopTime();
                    }
                }
                if(Tutorial.FristTime_ATK == false&& MainCharacter.GetComponent<Main_Char>().Cur_state != Main_Char.Char_state.Attack_I) 
                {
                    Tutorial.Tutorial = tutorial.Dash;
                }
            }
            if(Tutorial.Tutorial == Tutorial_Sequence.tutorial.Dash && MainCharacter.GetComponent<Main_Char>().Change_Behavior_enable == true) 
            {
                foreach (GameObject enemy in Enemys)
                {
                    if (enemy.GetComponent<Enemy_Common>().Cur_Attack_State == Enemy.Attack_State.Pre_Attack && Tutorial.FristTime_Dash == true)
                    {
                        Tutorial.StopTime();
                    }
                }
            }
            if(Tutorial.Tutorial == Tutorial_Sequence.tutorial.None)
            {
                Tutorial.text.stop_show_text();
            }
            foreach (GameObject enemy in Enemys)
            {
                if (enemy.GetComponent<Enemy_Common>().Cur_state == Enemy.State.Dead)
                {
                    Count++;
                }
            }
            if (Count == Enemys.Count)
            {
                Spawn_Dialog(0);
                
            }
        }
        else if(Current_Sequence == Sequence_Line.Chapter1_Part_2) 
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(0, 2);              
            }
            foreach (GameObject enemy in Enemys)
            {
                if (enemy.GetComponent<Enemy_Common>().Cur_state == Enemy.State.Dead)
                {
                    Count++;
                }
            }
            if (Count == Enemys.Count)
            {
                Spawn_Dialog(1);
            }
        }
        else if(Current_Sequence == Sequence_Line.Chapter2_Part_1) 
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(1, 2);
            }
            foreach (GameObject enemy in Enemys)
            {
                if (enemy.GetComponent<Enemy_Common>().Cur_state == Enemy.State.Dead)
                {
                    Count++;
                }
            }
            if (Count == Enemys.Count)
            {
                Spawn_Dialog(2);
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter2_Part_2)
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(2, 3);
            }
            foreach (GameObject enemy in Enemys)
            {
                if (enemy.GetComponent<Enemy_Common>().Cur_state == Enemy.State.Dead)
                {
                    Count++;
                }
            }
            if (Count == Enemys.Count)
            {
                Spawn_Dialog(3);
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter2_Part3)
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(3, 3);
            }
            foreach (GameObject enemy in Enemys)
            {
                if (enemy.GetComponent<Enemy_Common>().Cur_state == Enemy.State.Dead)
                {
                    Count++;
                }
            }
            if (Count == Enemys.Count)
            {
                Spawn_Dialog(4);
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter2_Part4)
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(2, 3);
            }
            foreach (GameObject enemy in Enemys)
            {
                if (enemy.GetComponent<Enemy_Common>().Cur_state == Enemy.State.Dead)
                {
                    Count++;
                }
            }
            if (Count == Enemys.Count)
            {
                Spawn_Dialog(5);
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter3_Part_1)
        {
            if (Enter_Chapter == true)
            {
                Enter_Chapter = false;
                Spawn_Enemy(5, 3);
            }
            foreach (GameObject enemy in Enemys)
            {
                if (enemy.GetComponent<Enemy_Common>().Cur_state == Enemy.State.Dead)
                {
                    Count++;
                }
            }
            if (Count > Enemys.Count-3)
            {
                Spawn_Dialog(6);
            }
        }
        else if (Current_Sequence == Sequence_Line.Chapter3_Part_2)
        {
            if (Enter_Chapter == true)
            {
                Spawn_Dialog(7);
                Enter_Chapter = false;
                Spawn_Enemy(6, 7);
            }            
            foreach (GameObject enemy in Enemys)
            {
                if (enemy.GetComponent<Enemy_Common>().Cur_state == Enemy.State.Dead)
                {
                    Count++;
                }
            }
            if (Count == Enemys.Count)
            {
                Spawn_Dialog(8);
                GameisEnd = true;
            }
            
            if(GameisEnd == true) 
            {              
                Ending_Time += Time.deltaTime;
                if(Ending_Time > 7) 
                {
                    Tutorial.text.show_text("Lancelot will return");                     
                }              
            }
        }

        if (index >= Enemys.Count) 
        {
            index = 0;
        }
    }
    public int Count;
    public float Ending_Time = 0;
   
    public bool Enter_Chapter = true;
    public void Chapter_End() 
    {
        
    }
    
    [SerializeField] int index = 0;
    private void Spawn_Enemy(int Num_L,int Num_R) 
    {
        
        for(int i = 0; i < Num_R; i++) 
        {
            if(Main.dificulty == Main.Dificulty.Normal)
            {
                Enemys[index].GetComponent<Enemy_Common>().Spawn(new Vector3(Enemy_Spawner_R.transform.position.x + (i * 2), Enemy_Spawner_R.transform.position.y, Enemy_Spawner_R.transform.position.z)
                , 40,10,10);
                index++;
            }
            else 
            {
                Enemys[index].GetComponent<Enemy_Common>().Spawn(new Vector3(Enemy_Spawner_R.transform.position.x + (i * 2), Enemy_Spawner_R.transform.position.y, Enemy_Spawner_R.transform.position.z)
               , 70, 60, 20);
                index++;
            }
            
        }
        for(int i = 0;i < Num_L; i ++) 
        {
            
            if (Main.dificulty == Main.Dificulty.Normal)
            {
                Enemys[index].GetComponent<Enemy_Common>().Spawn(new Vector3(Enemy_Spawner_L.transform.position.x - (i * 2), Enemy_Spawner_L.transform.position.y, Enemy_Spawner_L.transform.position.z)
                , 40, 10,10);
                index++;
            }
            else
            {
                Enemys[index].GetComponent<Enemy_Common>().Spawn(new Vector3(Enemy_Spawner_L.transform.position.x - (i * 2), Enemy_Spawner_L.transform.position.y, Enemy_Spawner_L.transform.position.z)
                 , 70, 60,20);
                index++;
            }
        }
    }
    int Dialog_Num =0;
    private void Spawn_Dialog(int num) 
    {
       
        Dialog[num].transform.position = new Vector3(Enemy_Spawner_R.transform.position.x - 7.36f, Enemy_Spawner_R.transform.position.y, Enemy_Spawner_R.transform.position.z);       
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
