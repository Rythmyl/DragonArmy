using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    private void Start()
    {
       
        {
            audioManager.Instance?.PlayMenuMusic();
        }
    }

    public void Play()
    {
        if (audioManager.Instance != null)
        {
            audioManager.Instance.PlayGameMusic();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    public void OpenCredits()
    {
        if (audioManager.Instance != null)
        {
           audioManager.Instance.PlayCreditsMusic();
        }

        SceneManager.LoadScene("Credits");
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Quit Game");
    }
}
