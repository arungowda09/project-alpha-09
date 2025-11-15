using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPlayButtonPressed()
    {
        AudioManager.Instance.PlayButtonTap();
        
        Debug.Log("Play Button Pressed - Transition to Level Screen");
        
        SceneManager.LoadScene("LevelScreen");
    }

    public void OnQuitButtonClicked()
    {
        AudioManager.Instance.PlayButtonTap();
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
