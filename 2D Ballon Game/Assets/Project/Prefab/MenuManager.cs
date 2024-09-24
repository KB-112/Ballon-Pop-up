using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] public Button homeBtn;
    [SerializeField] public Button closePanelBtn;

    [SerializeField] public GameObject soundManager;
    [SerializeField] public List<GameObject> nextScenePanel;
    [SerializeField] public List<GameObject> menuPanel;
    [SerializeField] public List<GameObject> settingButtonPanel;

    public void Init()
    {
        // Assign button listeners
        Init(false);
        ButtonExecution();
    }

    private void ButtonExecution()
    {
        if (playBtn != null)
            playBtn.onClick.AddListener(() => { Debug.Log("Play button clicked."); Init(true); });

        if (quitBtn != null)
            quitBtn.onClick.AddListener(QuitGame);

        if (musicBtn != null)
            musicBtn.onClick.AddListener(() => { Debug.Log("Music button clicked."); SoundManager(false); });

        if (settingBtn != null)
            settingBtn.onClick.AddListener(() => { Debug.Log("Settings button clicked."); SettingPanel(true); });

        if (resumeBtn != null)
            resumeBtn.onClick.AddListener(() => { Debug.Log("Resume button clicked."); SettingPanel(false); });

        if (restartBtn != null)
            restartBtn.onClick.AddListener(() => { Debug.Log("Restart button clicked."); RestartCurrentScene(); });

        if (homeBtn != null)
            homeBtn.onClick.AddListener(() => { Debug.Log("Main Menu button clicked."); Init(false);SettingPanel(false); });

        if (closePanelBtn != null)
            closePanelBtn.onClick.AddListener(() => { Debug.Log("Close Panel button clicked."); SettingPanel(false); });
    }

    private void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    private void SoundManager(bool state)
    {
        if (soundManager != null)
            soundManager.SetActive(state);
        else
            Debug.LogError("SoundManager is not assigned!");
    }

    private void Init(bool state)
    {
        foreach (var item in nextScenePanel)
        {
            item.SetActive(state);
        }
        foreach (var item in menuPanel)
        {
            item.SetActive(!state);
        }
    }

    private void SettingPanel(bool state)
    {
        foreach (var item in settingButtonPanel)
        {
            item.SetActive(state);
        }
    }

    private void RestartCurrentScene()
    {
        // Reloading the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
