using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBackButtonPressed()
    {
        AudioManager.Instance.PlayButtonTap();

        Debug.Log("Back Button Pressed - Transition to Menu Screen");

        SceneManager.LoadScene("MenuScreen");
    }
    
    public void OnLevelButtonPressed(int levelIndex)
    {
        AudioManager.Instance.PlayButtonTap();

        Debug.Log("Level " + levelIndex + " Selected");

        // Set game settings based on level
        switch (levelIndex)
        {
            case 1:
                GameSettings.rows = 3;
                GameSettings.cols = 2;
                break;
            case 2:
                GameSettings.rows = 3;
                GameSettings.cols = 4;
                break;
            case 3:
                GameSettings.rows = 5;
                GameSettings.cols = 4;
                break;
            default:
                Debug.LogError("Invalid level index: " + levelIndex);
                return;
        }

        // Load the game scene
        SceneManager.LoadScene("GamePlay");
    }
}
