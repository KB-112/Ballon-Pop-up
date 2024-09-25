using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BalloonPath : MonoBehaviour
{
    public List<GameObject> objectPooled;
    public List<GameObject> objectRedBalloonPooled;
    public List<ListContainer> listContainers = new List<ListContainer>();
    
    private bool isInitialSetupComplete = false;
    private int currentScore = 0; // Example score variable; adjust as needed
   private void Awake()
    {

        // Subscribe to game over event
        GameOver.OnGameOver += ClearPool;
        SceneLoadingManager.OnGamePause +=ClearPool;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        GameOver.OnGameOver -= ClearPool;
        SceneLoadingManager.OnGamePause -=ClearPool;
    }









public void ClearPool()
{
    objectPooled.Clear();
    objectRedBalloonPooled.Clear();
    BalloonManager.Instance.amountToPool=0;
    BalloonManager.Instance.pooledObjects.Clear();
    Debug.Log("Pool clear");

}

    void Start()
    {
        objectPooled = BalloonManager.Instance.GetPooledObject();

        // Ensure we split the pool correctly
        int totalObjects = objectPooled.Count;
        int splitIndex = Mathf.Min(totalObjects, 10);

        // Split into regular and red balloon pools
        objectRedBalloonPooled = new List<GameObject>(totalObjects - splitIndex);
        for (int i = splitIndex; i < totalObjects; i++)
        {
            objectRedBalloonPooled.Add(objectPooled[i]);
        }
        objectPooled = objectPooled.GetRange(0, splitIndex); // First 10 objects for regular balloons

        InitializeListContainers();
        Categorize();
        StartCoroutine(InitializePool());
    }

    void Update()
    {
        // Check if Time.timeScale is zero and handle balloons accordingly
        if (Time.timeScale == 0)
        {
            HandleInactiveBalloons();
        }
    }

    void HandleInactiveBalloons()
    { for (int i = 0; i < 4; i++)
        {
        foreach (var balloon in objectPooled)
        {
            if (balloon.activeInHierarchy)
            {
                balloon.SetActive(false);
                BalloonManager.Instance.ResetBalloonPosition(balloon.transform, BalloonManager.Instance.balloonConfigs[i].path[0].position);
            }
        }
        

        foreach (var balloon in objectRedBalloonPooled)
        {
            if (balloon.activeInHierarchy)
            {
                balloon.SetActive(false);
                BalloonManager.Instance.ResetBalloonPosition(balloon.transform, BalloonManager.Instance.balloonConfigs[i].path[0].position);
            }
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
            StartCoroutine(BalloonCall(objectPooled, 0, 3)); // Updated indices
        }

        if (objectRedBalloonPooled.Count > 0)
        {
            StartCoroutine(BalloonCall(objectRedBalloonPooled, 3, 4)); // Updated indices
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
            float waitTime = 0;

            if (minConfig == 3)
            {
                waitTime = GetWaitTimeBasedOnScore(); // Adjust wait time based on score
            }
            else
            {
                waitTime = BalloonManager.Instance.balloonConfigs[i].duration / 2;
            }

            for (int j = 0; j < balloonCount; j++)
            {
                if (store >= objectPool.Count)
                {
                    store = 0; // Resetting to the start of the pool if exceeded
                }

                GameObject balloon = objectPool[store];

                // Check if the balloon is active in the scene
                if (balloon.activeInHierarchy)
                {
                    // Skip to the next balloon if it's already active
                    store++;
                    continue;
                }
                else
                {
                    // Reset balloon position
                    BalloonManager.Instance.ResetBalloonPosition(balloon.transform, BalloonManager.Instance.balloonConfigs[i].path[0].position);
                }

                balloon.SetActive(true);
                BalloonManager.Instance.AssignLayer(balloon, i);
                BalloonManager.Instance.PathMovementPattern(balloon.transform, BalloonManager.Instance.balloonConfigs[i]);
                BalloonManager.Instance.ChangeSprite(balloon.GetComponent<SpriteRenderer>(), BalloonManager.Instance.balloonConfigs[i].sprites);

                store++;
                yield return new WaitForSeconds(1.5f);
            }

            // Wait before processing the next configuration, based on score
            yield return new WaitForSeconds(waitTime);
        }

        // After finishing the current configuration, restart or reinitialize
        if (!isInitialSetupComplete)
        {
            isInitialSetupComplete = true;
            StartCoroutine(InitializePool()); // Reinitialize the pool or handle as needed
        }
    }

    float GetWaitTimeBasedOnScore()
    {
        currentScore = ScoreManager.Instance.updateScore;
        if (currentScore > 500)
        {
            return 5f; // Wait time if score is more than 500
        }
        else
        {
            return 12f; // Wait time if score is 500 or less
        }
    }

    void Categorize()
    {
        while (listContainers.Count < BalloonManager.Instance.balloonConfigs.Count)
        {
            listContainers.Add(new ListContainer());
        }

        for (int i = 0; i < BalloonManager.Instance.balloonConfigs.Count; i++)
        {
            if (i >= objectPooled.Count)
            {
                break;
            }

            var balloonConfig = BalloonManager.Instance.balloonConfigs[i];
            var balloonType = balloonConfig.type;

            for (int j = 0; j < balloonConfig.balloonCount; j++)
            {
                if (j >= objectPooled.Count)
                {
                    break;
                }

                listContainers[i].balloonObjects.Add(objectPooled[j]);
                listContainers[i].balloonType = balloonType;
            }
        }
    }
}

[System.Serializable]
public class ListContainer
{
    public List<GameObject> balloonObjects = new List<GameObject>();
    public BalloonType balloonType;
}
