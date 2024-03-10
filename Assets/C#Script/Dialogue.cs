using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public GameObject dialoguePannel;
    public TextMeshProUGUI text;
    public float textSpeed;
    public string[] dialogue;

    public int index;
    public bool PlayDialogue;

    public GameObject nextButton;    
    void Start()
    {
        
        
    }

    void Update()
    {
       /* if(PlayDialogue) 
        {
            *//*if (dialoguePannel.activeInHierarchy)
            {
                zeroText();
            }
            else
            {
                dialoguePannel.SetActive(true);
                StartCoroutine(Typing());
            }*//*

            dialoguePannel.SetActive(true);
            StartCoroutine(Typing());
        }*/

        if (text.text == dialogue[index])
        {
            nextButton.SetActive(true);
        }
    }

    public void zeroText()
    {
        text.text = "";
        index = 0;
        dialoguePannel.SetActive(false);
    }

    IEnumerator Typing()
    {
        foreach(char letter in dialogue[index].ToCharArray())
        {
            text.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void NextLine()
    {
        Debug.Log("Next");

        nextButton.SetActive(false);

        if(index < dialogue.Length - 1)
        {
            index++;
            text.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayDialogue = true;

            dialoguePannel.SetActive(true);
            StartCoroutine(Typing());            
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayDialogue = false;
            zeroText();
        }
    }

}
