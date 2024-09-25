using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BalloonPath : MonoBehaviour
{
    public List<GameObject> objectPooled;
    public List<GameObject> objectRedBalloonPooled;
    public List<ListContainer> listContainers = new List<ListContainer>();

    private bool isInitialSetupComplete = false;
    private bool isPaused = false;
    private int currentScore = 0; // Example score variable; adjust as needed

    private void Awake()
    {
        // Subscribe to game over and pause 
        SceneLoadingManager.OnPressPlayBtn += StartPopingBallon;
        GameOver.OnGameOver += ResetAllBalloons;
        SceneLoadingManager.OnGamePause += HandlePause;
        SceneLoadingManager.OnGameResume += HandleResume;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the events when the object is destroyed
        SceneLoadingManager.OnPressPlayBtn -= StartPopingBallon;
        GameOver.OnGameOver -= ResetAllBalloons;
        SceneLoadingManager.OnGamePause -= HandlePause;
        SceneLoadingManager.OnGameResume -= HandleResume;
    }

    private void Start()
    {
        objectPooled = BalloonManager.Instance.GetPooledObject();
        SplitPooledObjects();
        InitializeListContainers();
        CategorizeBalloons();
        StartCoroutine(InitializePool());
    }

    public void StartPopingBallon()
    {
        StartCoroutine(InitializePool());
        ResetAllBalloons();
        Debug.Log("Play Btn Pressed");
    }

    private void Update()
    {
        if (Time.timeScale == 0 && !isPaused)
        {
            HandleInactiveBalloons();
        }
    }

    void SplitPooledObjects()
    {
        int totalObjects = objectPooled.Count;
        int splitIndex = Mathf.Min(totalObjects, 10);

        objectRedBalloonPooled = new List<GameObject>();
        for (int i = splitIndex; i < totalObjects; i++)
        {
            objectRedBalloonPooled.Add(objectPooled[i]);
        }
        objectPooled = objectPooled.GetRange(0, splitIndex); // First 10 objects for regular balloons
    }

    void HandleInactiveBalloons()
    {
        for (int i = 0; i < 4; i++)
        {
            ResetInactiveBalloons(objectPooled, i);
            ResetInactiveBalloons(objectRedBalloonPooled, i);
        }
    }

    void ResetInactiveBalloons(List<GameObject> balloonPool, int configIndex)
    {
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
        int balloonConfigCount = BalloonManager.Instance.balloonConfigs.Count;
        listContainers = new List<ListContainer>(balloonConfigCount);

        for (int i = 0; i < balloonConfigCount; i++)
        {
            listContainers.Add(new ListContainer());
        }
    }

    IEnumerator InitializePool()
    {
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
        currentScore = ScoreManager.Instance.updateScore;
        return currentScore > 500 ? 5f : 12f;
    }

    void CategorizeBalloons()
    {
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
        ResetBalloonPool(objectPooled);
        ResetBalloonPool(objectRedBalloonPooled);

        BalloonManager.Instance.amountToPool = 0;
        Debug.Log("All balloons reset");
    }

    void ResetBalloonPool(List<GameObject> balloonPool)
    {
        foreach (var balloon in balloonPool)
        {
            balloon.SetActive(false);
            BalloonManager.Instance.ResetBalloonPosition(balloon.transform, Vector3.zero); // Reset to default or appropriate position
        }
    }

    void HandlePause()
    {
        isPaused = true;
        // Pause logic for balloons
        HandleInactiveBalloons();
    }

    void HandleResume()
    {
        isPaused = false;
        // Resume logic for balloons
        StartCoroutine(InitializePool());
    }
}

[System.Serializable]
public class ListContainer
{
    public List<GameObject> balloonObjects = new List<GameObject>();
    public BalloonType balloonType;
}
