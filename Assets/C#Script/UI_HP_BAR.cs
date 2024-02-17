using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HP_BAR : MonoBehaviour
{
    public Image HP_bar;
    public Main_Char Main_Char;
    private float Max_HP;
    private void Start()
    {
        Max_HP = Main_Char.HP;
    }
    private void Update()
    {
        HP_bar.fillAmount = (float)(Main_Char.HP / Max_HP);
    }
}
