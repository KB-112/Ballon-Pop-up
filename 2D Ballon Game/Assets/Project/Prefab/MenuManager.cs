using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MenuManager : MonoBehaviour
{    [SerializeField] private SceneLoadingManager sceneLoadingManager;
      



     private void Awake()
    {
        if (sceneLoadingManager != null)
        {
            sceneLoadingManager.Init();
        }
        else
        {
            Debug.LogError("SceneLoadingManager is not assigned!");
        }

        // Subscribe to game over event
        GameOver.OnGameOver += HandleGameOver;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        GameOver.OnGameOver -= HandleGameOver;
    }

    // This will be called when the game over event is triggered
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
    [SerializeField] public Button playBtn;
    [SerializeField] public Button quitBtn;
    [SerializeField] public Button musicBtn;
    [SerializeField] public Button settingBtn;
    [SerializeField] public Button resumeBtn;
    [SerializeField] public Button restartBtn;
    [SerializeField] public Button closePanelBtn;

      [SerializeField] public Button closeGameOverPanelBtn;
     

 [SerializeField] public Button closeInfoPanelBtn;
 [SerializeField] public Button openInfoPanelBtn;
    [SerializeField] public AudioSource soundManager;
    [SerializeField] public List<GameObject> nextScenePanel;
    [SerializeField] public List<GameObject> menuPanel;
    [SerializeField] public List<GameObject> settingButtonPanel;

    public GameObject ballonController;

    public static event Action OnGamePause;
    public static event Action OnGameResume;
    public static event Action OnPressPlayBtn;

    public GameObject closeGameOverPanel;
     public GameObject closeInfoPanel;

    public void Init()
    {
        ButtonExecution();
    }

    private void ButtonExecution()
    {
        if (playBtn != null)
            playBtn.onClick.AddListener(() => {
                Debug.Log("Play button clicked.");
                Init(true);
                OnPressPlayBtn?.Invoke();  // Ensure play button triggers the event here
            });

        if (quitBtn != null)
            quitBtn.onClick.AddListener(QuitGame);

        if (musicBtn != null)
            musicBtn.onClick.AddListener(() => {
                Debug.Log("Music button clicked.");
                SoundManager();
            });

        if (settingBtn != null)
            settingBtn.onClick.AddListener(() => {
                Debug.Log("Settings button clicked.");
                SettingPanel(true);
            });

        if (resumeBtn != null)
            resumeBtn.onClick.AddListener(() => {
                Debug.Log("Resume button clicked.");
                OnGameResume?.Invoke();
                SettingPanel(false);
            });

        if (restartBtn != null)
            restartBtn.onClick.AddListener(() => {
                Debug.Log("Restart button clicked.");
                Init(false);  // Restart game, reset everything
                SettingPanel(false);
            });

        if (closePanelBtn != null)
            closePanelBtn.onClick.AddListener(() => {
                Debug.Log("Close Panel button clicked.");
                SettingPanel(false);
            });


            if (closeGameOverPanelBtn != null)
            closeGameOverPanelBtn.onClick.AddListener(() => {
                Debug.Log("Close Panel button clicked.");
                  Init(false);  // Restart game, reset everything
                SettingPanel(false);
                closeGameOverPanel.SetActive(false);
                 soundManager.UnPause();
            });

              if (closeInfoPanelBtn != null)
            closeInfoPanelBtn.onClick.AddListener(() => {
                Debug.Log("Close Panel button clicked.");
                InfoPanel(false);
                 soundManager.UnPause();
            });


              if (openInfoPanelBtn != null)
            openInfoPanelBtn.onClick.AddListener(() => {
                
                  InfoPanel(true);
            });



    }

    private void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    private void SoundManager()
    {
        if (soundManager.isPlaying)
        {
            // Stop the music
            soundManager.Pause();
        }
        else
        {
            // Play the music
            soundManager.UnPause();
        }
    }

    public AudioSource GetAudioSource()
    {
        return soundManager;
    }

    private void Init(bool state)
    {
        // Toggle next scene and menu panels based on the state
        foreach (var item in nextScenePanel)
        {
            item.SetActive(state);
        }
        foreach (var item in menuPanel)
        {
            item.SetActive(!state);
        }

        // Enable or disable the balloon controller
        ballonController.SetActive(state);

        // Trigger the OnPressPlayBtn event when the play button is pressed
        if (state)
        {
            OnPressPlayBtn?.Invoke();  // Ensure event is triggered after UI is updated
        }
    }

    private void SettingPanel(bool state)
    {
        // Toggle the visibility of setting buttons
        foreach (var item in settingButtonPanel)
        {
            item.SetActive(state);
        }

        // Pause or resume the game
        Time.timeScale = state ? 0 : 1;

        // Optionally invoke game pause event when settings are shown
        if (state)
        {
            OnGamePause?.Invoke();
        }
    }

    public void GameOverSoundControl()
    {
        if (soundManager != null && soundManager.isPlaying)
        {
            soundManager.Pause();
            Debug.Log("Music paused due to Game Over.");
        }
    }

    public void InfoPanel(bool state)
    {
        closeInfoPanel.SetActive(state);

    }
}
