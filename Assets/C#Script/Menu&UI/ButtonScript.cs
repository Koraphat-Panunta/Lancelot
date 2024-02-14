using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] GameObject Button;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnCursorEnter()
    {
        Button.transform.position = new Vector2(this.Button.transform.position.x, this.Button.transform.position.y + 5);
    }

    public void OnCursorExit()
    {
        Button.transform.position = new Vector2(this.Button.transform.position.x, this.Button.transform.position.y - 5);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

