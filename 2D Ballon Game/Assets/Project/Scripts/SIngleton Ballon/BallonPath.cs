using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BalloonPath : MonoBehaviour
{
    public List<GameObject> objectPooled; // Pool of regular balloon objects
    public List<GameObject> objectRedBalloonPooled; // Pool of red balloon objects
    public List<ListContainer> listContainers = new List<ListContainer>(); // Containers for balloon configurations

    private bool isInitialSetupComplete = false; // Flag to check if the initial setup is complete
    private bool isPaused = false; // Flag to check if the game is paused
    private int currentScore = 0; // Current score; used for determining wait time

    private void Awake()
    {
        // Subscribe to relevant game events
        SceneLoadingManager.OnPressPlayBtn += StartPopingBallon;
        GameOver.OnGameOver += ResetAllBalloons;
        SceneLoadingManager.OnGamePause += HandlePause;
        SceneLoadingManager.OnGameResume += HandleResume;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events when the object is destroyed
        SceneLoadingManager.OnPressPlayBtn -= StartPopingBallon;
        GameOver.OnGameOver -= ResetAllBalloons;
        SceneLoadingManager.OnGamePause -= HandlePause;
        SceneLoadingManager.OnGameResume -= HandleResume;
    }

    private void Start()
    {
        // Initialize balloon pools and configurations
        objectPooled = BalloonManager.Instance.GetPooledObject();
        SplitPooledObjects();
        InitializeListContainers();
        CategorizeBalloons();
        StartCoroutine(InitializePool());
    }

    public void StartPopingBallon()
    {
        // Start balloon spawning process and reset balloons
        StartCoroutine(InitializePool());
        ResetAllBalloons();
        Debug.Log("Play Btn Pressed");
    }

    private void Update()
    {
        // Handle inactive balloons when the game is paused
        if (Time.timeScale == 0 && !isPaused)
        {
            HandleInactiveBalloons();
        }
    }

    void SplitPooledObjects()
    {
        // Split the pool into regular and red balloon pools
        int totalObjects = objectPooled.Count;
        int splitIndex = Mathf.Min(totalObjects, 10);

        objectRedBalloonPooled = new List<GameObject>();
        for (int i = splitIndex; i < totalObjects; i++)
        {
            objectRedBalloonPooled.Add(objectPooled[i]);
        }
        objectPooled = objectPooled.GetRange(0, splitIndex); // First 10 objects are for regular balloons
    }

    void HandleInactiveBalloons()
    {
        // Reset inactive balloons in both pools
        for (int i = 0; i < 4; i++)
        {
            ResetInactiveBalloons(objectPooled, i);
            ResetInactiveBalloons(objectRedBalloonPooled, i);
        }
    }

    void ResetInactiveBalloons(List<GameObject> balloonPool, int configIndex)
    {
        // Deactivate and reset position of inactive balloons
        foreach (var balloon in balloonPool)
        {
            if (balloon.activeInHierarchy)
            {
                balloon.SetActive(false);
                BalloonManager.Instance.ResetBalloonPosition(balloon.transform, BalloonManager.Instance.balloonConfigs[configIndex].path[0].position);
            }
        }
    }

    void InitializeListContainers()
    {
        // Initialize list containers based on the number of balloon configurations
        int balloonConfigCount = BalloonManager.Instance.balloonConfigs.Count;
        listContainers = new List<ListContainer>(balloonConfigCount);

        for (int i = 0; i < balloonConfigCount; i++)
        {
            listContainers.Add(new ListContainer());
        }
    }

    IEnumerator InitializePool()
    {
        // Initialize balloon pool spawning for regular and red balloons
        if (objectPooled.Count > 0)
        {
            StartCoroutine(BalloonCall(objectPooled, 0, 3));
        }

        if (objectRedBalloonPooled.Count > 0)
        {
            StartCoroutine(BalloonCall(objectRedBalloonPooled, 3, 4));
        }

        yield return null;
    }

    IEnumerator BalloonCall(List<GameObject> objectPool, int minConfig, int maxConfig)
    {
        // Spawn balloons from the pool based on configurations
        if (objectPool == null || objectPool.Count == 0)
            yield break;

        int store = 0;

        for (int i = minConfig; i < maxConfig; i++)
        {
            if (i >= BalloonManager.Instance.balloonConfigs.Count)
                yield break;

            int balloonCount = BalloonManager.Instance.balloonConfigs[i].balloonCount;
            float waitTime = (minConfig == 3) ? GetWaitTimeBasedOnScore() : BalloonManager.Instance.balloonConfigs[i].duration / 2;

            for (int j = 0; j < balloonCount; j++)
            {
                if (store >= objectPool.Count)
                {
                    store = 0; // Reset to start of the pool if exceeded
                }

                GameObject balloon = objectPool[store];

                if (balloon.activeInHierarchy)
                {
                    store++;
                    continue;
                }

                BalloonManager.Instance.ResetBalloonPosition(balloon.transform, BalloonManager.Instance.balloonConfigs[i].path[0].position);
                balloon.SetActive(true);

                BalloonManager.Instance.AssignLayer(balloon, i);
                BalloonManager.Instance.PathMovementPattern(balloon.transform, BalloonManager.Instance.balloonConfigs[i]);
                BalloonManager.Instance.ChangeSprite(balloon.GetComponent<SpriteRenderer>(), BalloonManager.Instance.balloonConfigs[i].sprites);

                store++;
                yield return new WaitForSeconds(1.5f);
            }

            yield return new WaitForSeconds(waitTime);
        }

        if (!isInitialSetupComplete)
        {
            isInitialSetupComplete = true;
            StartCoroutine(InitializePool());
        }
    }

    float GetWaitTimeBasedOnScore()
    {
        // Determine the wait time based on the current score
        currentScore = ScoreManager.Instance.updateScore;
        return currentScore > 500 ? 5f : 12f;
    }

    void CategorizeBalloons()
    {
        // Categorize balloons into list containers based on configurations
        while (listContainers.Count < BalloonManager.Instance.balloonConfigs.Count)
        {
            listContainers.Add(new ListContainer());
        }

        for (int i = 0; i < BalloonManager.Instance.balloonConfigs.Count; i++)
        {
            if (i >= objectPooled.Count)
                break;

            var balloonConfig = BalloonManager.Instance.balloonConfigs[i];
            var balloonType = balloonConfig.type;

            for (int j = 0; j < balloonConfig.balloonCount; j++)
            {
                if (j >= objectPooled.Count)
                    break;

                listContainers[i].balloonObjects.Add(objectPooled[j]);
                listContainers[i].balloonType = balloonType;
            }
        }
    }

    public void ResetAllBalloons()
    {
        // Reset all balloons in the pools
        ResetBalloonPool(objectPooled);
        ResetBalloonPool(objectRedBalloonPooled);

        BalloonManager.Instance.amountToPool = 0;
        Debug.Log("All balloons reset");
    }

    void ResetBalloonPool(List<GameObject> balloonPool)
    {
        // Deactivate and reset all balloons in the specified pool
        foreach (var balloon in balloonPool)
        {
            balloon.SetActive(false);
            BalloonManager.Instance.ResetBalloonPosition(balloon.transform, Vector3.zero); // Reset to default or appropriate position
        }
    }

    void HandlePause()
    {
        // Handle game pause logic for balloons
        isPaused = true;
        HandleInactiveBalloons();
    }

    void HandleResume()
    {
        // Handle game resume logic for balloons
        isPaused = false;
        StartCoroutine(InitializePool());
    }
}

[System.Serializable]
public class ListContainer
{
    public List<GameObject> balloonObjects = new List<GameObject>(); // List of balloon objects
    public BalloonType balloonType; // Type of balloons in this container
}
