using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    public GameObject dialoguePannel;
    public TextMeshProUGUI text;
    public float textSpeed;
    public string[] dialogue;

    public int index;
    public bool PlayDialogue;
    public bool canPlayDialogue = true;
    public bool canPlayNextLine = true;
    public bool startCounter = false;

    public float counter = 0;
    public float waitTime = 0;
    public float dialogueCloseTime = 2;

    public GameObject nextButton;
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void Update()
    {
       
        //Do below after finished dialogue
        if (text.text == dialogue[index])
        {
            nextButton.SetActive(true);
            if(startCounter)
            {
                counter += 1 * Time.deltaTime;
            }

            if (index <= dialogue.Length/* && canPlayNextLine*/)
            {
                //Invoke("NextLine", 5);

                NextLine();
                canPlayNextLine = false;
            }
            
            //zeroText();

            /*if (index == dialogue.Length)
            {
                Debug.Log("closing");
                PlayDialogue = false;
                Invoke("zeroText", 10);
                timer = 0;
            }*/

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
        canPlayNextLine = true;
        if(counter >= waitTime)
        {
            Debug.Log("NextLine");

            nextButton.SetActive(false);

            if (index < dialogue.Length - 1)
            {
                index++;
                text.text = "";
                StartCoroutine(Typing());
            }
            else
            {
                //zeroText();
                index = 0;
                Debug.Log("closing");
                PlayDialogue = false;
                Invoke("zeroText", dialogueCloseTime);
                counter = 0;
                GameObject.Destroy(gameObject);
            }
            counter = 0;
        }
    }
    public void CloseText() 
    {
        index = 0;
        Debug.Log("closing");
        PlayDialogue = false;
        Invoke("zeroText", dialogueCloseTime);
        counter = 0;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canPlayDialogue == true) 
        {
            zeroText();
            PlayDialogue = true;
            startCounter = true;
            dialoguePannel.SetActive(true);
            StartCoroutine(Typing());
            Debug.Log(gameObject.name);            
            canPlayDialogue = false;
        }

    }
    
    public void OnTriggerExit(Collider other)
    {
        /*if (other.CompareTag("Player"))
        {
            PlayDialogue = false;
            zeroText();
        }*/
    }

}
