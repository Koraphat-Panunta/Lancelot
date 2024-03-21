using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Set_Text : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    private void Start()
    {
        textMeshPro.enabled = false;
    }
    public void show_text(string text) 
    {
        textMeshPro.enabled = true;
        textMeshPro.SetText(text);        
    }
    public void stop_show_text() 
    {
        textMeshPro.enabled = false;
    }
  
}
