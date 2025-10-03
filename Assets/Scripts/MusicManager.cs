using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // keeps music playing between scenes
        }
        else
        {
            Destroy(gameObject); // prevent duplicate managers
        }
    }
}
