using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Use only the scene name, not the path
         Debug.Log("Play button pressed! Attempting to load MainGame...");
        SceneManager.LoadScene("MainGame");  
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
