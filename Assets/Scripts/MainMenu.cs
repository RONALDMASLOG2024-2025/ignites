using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    // Call from the Play button. Validates the scene is in Build Settings before loading.
    public void PlayGame()
    {
        const string sceneName = "MainGame";
        Debug.Log($"Play button pressed! Attempting to load {sceneName}...");

        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is not in Build Settings. Add it via File > Build Settings > Scenes In Build.");
            // Still try to load to surface any runtime errors in case user prefers that.
        }

        SceneManager.LoadScene(sceneName);
    }

    // Quit button behaviour. In editor this will stop Play Mode; in a build it quits the application.
    public void QuitGame()
    {
        Debug.Log("Quit Game requested.");
#if UNITY_EDITOR
        // Stop Play Mode in the editor
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Utility: checks the project's Build Settings for a scene with the given name
    private bool IsSceneInBuildSettings(string sceneName)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
}
