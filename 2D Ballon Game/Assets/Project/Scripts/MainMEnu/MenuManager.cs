using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private SceneLoadingManager sceneLoadingManager;  // Reference to the SceneLoadingManager component

    private void Awake()
    {
        // Initialize SceneLoadingManager if assigned
        if (sceneLoadingManager != null)
        {
            sceneLoadingManager.Init();
        }
        else
        {
            Debug.LogError("SceneLoadingManager is not assigned!");
        }

        // Subscribe to the game over event
        GameOver.OnGameOver += HandleGameOver;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the game over event when this object is destroyed
        GameOver.OnGameOver -= HandleGameOver;
    }

    // Handle the game over event by controlling sound
    private void HandleGameOver()
    {
        if (sceneLoadingManager != null)
        {
            sceneLoadingManager.GameOverSoundControl();
        }
    }
}

[System.Serializable]
public class SceneLoadingManager
{
    [SerializeField] public Button playBtn;  // Button to start the game
    [SerializeField] public Button quitBtn;  // Button to quit the game
    [SerializeField] public Button musicBtn;  // Button to toggle music
    [SerializeField] public Button settingBtn;  // Button to open settings
    [SerializeField] public Button resumeBtn;  // Button to resume the game
    [SerializeField] public Button restartBtn;  // Button to restart the game
    [SerializeField] public Button closePanelBtn;  // Button to close any open panel
    [SerializeField] public Button closeGameOverPanelBtn;  // Button to close the game over panel
    [SerializeField] public Button closeInfoPanelBtn;  // Button to close the info panel
    [SerializeField] public Button openInfoPanelBtn;  // Button to open the info panel
    [SerializeField] public AudioSource soundManager;  // Audio source for music
    [SerializeField] public List<GameObject> nextScenePanel;  // Panels to show when transitioning to the next scene
    [SerializeField] public List<GameObject> menuPanel;  // Panels for the main menu
    [SerializeField] public List<GameObject> settingButtonPanel;  // Panels for the settings menu

    public GameObject ballonController;  // Controller for balloon-related functionality

    public static event Action OnGamePause;  // Event triggered when the game is paused
    public static event Action OnGameResume;  // Event triggered when the game is resumed
    public static event Action OnPressPlayBtn;  // Event triggered when the play button is pressed

    public GameObject closeGameOverPanel;  // Game over panel
    public GameObject closeInfoPanel;  // Info panel

    // Initialize the SceneLoadingManager
    public void Init()
    {
        ButtonExecution();  // Set up button listeners
    }

    private void ButtonExecution()
    {
        // Setup listener for the play button
        if (playBtn != null)
        {
            playBtn.onClick.AddListener(() => {
                Debug.Log("Play button clicked.");
                Init(true);  // Initialize the game state
                OnPressPlayBtn?.Invoke();  // Trigger play button event
            });
        }

        // Setup listener for the quit button
        if (quitBtn != null)
        {
            quitBtn.onClick.AddListener(QuitGame);
        }

        // Setup listener for the music button
        if (musicBtn != null)
        {
            musicBtn.onClick.AddListener(() => {
                Debug.Log("Music button clicked.");
                SoundManager();  // Toggle music playback
            });
        }

        // Setup listener for the settings button
        if (settingBtn != null)
        {
            settingBtn.onClick.AddListener(() => {
                Debug.Log("Settings button clicked.");
                SettingPanel(true);  // Show settings panel
            });
        }

        // Setup listener for the resume button
        if (resumeBtn != null)
        {
            resumeBtn.onClick.AddListener(() => {
                Debug.Log("Resume button clicked.");
                OnGameResume?.Invoke();  // Resume the game
                SettingPanel(false);  // Hide settings panel
            });
        }

        // Setup listener for the restart button
        if (restartBtn != null)
        {
            restartBtn.onClick.AddListener(() => {
                Debug.Log("Restart button clicked.");
                Init(false);  // Restart game and reset state
                SettingPanel(false);  // Hide settings panel
            });
        }

        // Setup listener for the close panel button
        if (closePanelBtn != null)
        {
            closePanelBtn.onClick.AddListener(() => {
                Debug.Log("Close Panel button clicked.");
                SettingPanel(false);  // Hide settings panel
            });
        }

        // Setup listener for the close game over panel button
        if (closeGameOverPanelBtn != null)
        {
            closeGameOverPanelBtn.onClick.AddListener(() => {
                Debug.Log("Close Game Over Panel button clicked.");
                Init(false);  // Restart game and reset state
                SettingPanel(false);  // Hide settings panel
                closeGameOverPanel.SetActive(false);  // Hide game over panel
                soundManager.UnPause();  // Resume music playback
            });
        }

        // Setup listener for the close info panel button
        if (closeInfoPanelBtn != null)
        {
            closeInfoPanelBtn.onClick.AddListener(() => {
                Debug.Log("Close Info Panel button clicked.");
                InfoPanel(false);  // Hide info panel
                soundManager.UnPause();  // Resume music playback
            });
        }

        // Setup listener for the open info panel button
        if (openInfoPanelBtn != null)
        {
            openInfoPanelBtn.onClick.AddListener(() => {
                InfoPanel(true);  // Show info panel
            });
        }
    }

    // Quit the application
    private void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    // Toggle music playback
    private void SoundManager()
    {
        if (soundManager.isPlaying)
        {
            soundManager.Pause();  // Pause music if playing
        }
        else
        {
            soundManager.UnPause();  // Resume music if paused
        }
    }

    // Get the audio source component
    public AudioSource GetAudioSource()
    {
        return soundManager;
    }

    // Initialize UI panels and game state
    private void Init(bool state)
    {
        foreach (var item in nextScenePanel)
        {
            item.SetActive(state);  // Show or hide next scene panels
        }
        foreach (var item in menuPanel)
        {
            item.SetActive(!state);  // Show or hide menu panels
        }

        ballonController.SetActive(state);  // Enable or disable balloon controller

        if (state)
        {
            OnPressPlayBtn?.Invoke();  // Trigger play button event
        }
    }

    // Toggle the visibility of setting panels and pause/resume game
    private void SettingPanel(bool state)
    {
        foreach (var item in settingButtonPanel)
        {
            item.SetActive(state);  // Show or hide setting buttons
        }

        Time.timeScale = state ? 0 : 1;  // Pause or resume game time

        if (state)
        {
            OnGamePause?.Invoke();  // Trigger game pause event
        }
    }

    // Pause the music when the game is over
    public void GameOverSoundControl()
    {
        if (soundManager != null && soundManager.isPlaying)
        {
            soundManager.Pause();
            Debug.Log("Music paused due to Game Over.");
        }
    }

    // Show or hide the info panel
    public void InfoPanel(bool state)
    {
        closeInfoPanel.SetActive(state);  // Show or hide info panel
    }
}
