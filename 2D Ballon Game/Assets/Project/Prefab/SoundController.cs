using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour
{
    public List<AudioIdentification> audioIdentifications;
    public AudioSource audioSource;

    private List<AudioSource> additionalAudioSources;
    private string lastSceneName;

    void Awake()
    {
        DontDestroyOnLoad(this);
        additionalAudioSources = new List<AudioSource>();
        lastSceneName = ""; // Initialize the last scene name
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;  
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

    // Called every time a new scene is loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RecheckState(scene.name);  
    }

    void RecheckState(string currentSceneName)
    {
        if (lastSceneName == currentSceneName)
        {
            // If the previous scene is the same as the current scene, return early
            return;
        }
        
        Init(currentSceneName); // Initialize for the new scene
        lastSceneName = currentSceneName; // Update last scene name
    }

    void Start()
    {
        Init(SceneManager.GetActiveScene().name); // Initialize on start
        lastSceneName = SceneManager.GetActiveScene().name; // Store the starting scene name
    }

    void Init(string currentSceneName)
    {
        bool audioPlayed = false; // Track if any audio is playing

        foreach (var item in audioIdentifications)
        {
            if (item.sceneName == currentSceneName)
            {
                if (!audioSource.isPlaying || audioSource.clip != item.audioClip)
                {
                    audioSource.clip = item.audioClip;
                    audioSource.Play();
                    audioPlayed = true; // Mark that audio is playing
                }
            }
        }

        // If no audio was played for the current scene, stop the audio
        if (!audioPlayed && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (additionalAudioSources.Count > 0)
        {
            Debug.LogWarning("Additional Audio Source is found");
        }
    }
}

[System.Serializable]
public class AudioIdentification
{
    public AudioClip audioClip; 
    public string sceneName;    
}
