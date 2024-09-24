using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BalloonManager : MonoBehaviour
{
    public static BalloonManager Instance { get; private set; }
    public List<BalloonConfig> balloonConfigs; // Public to set in Inspector
    public GameObject balloonPrefab; // Prefab for the balloon
    public List<GameObject> pooledObjects;

    public int amountToPool;
  

    private void Awake()
    {
        // Ensure only one instance of BalloonManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this manager across scenes
          
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeBalllon();
    }

  public void InitializeBalllon()
{
   
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject newBallon = Instantiate(balloonPrefab); 
            newBallon.SetActive(false); 
            pooledObjects.Add(newBallon); 
        }
    
}




public List<GameObject> GetPooledObject()
{
   
    return pooledObjects;
}
  
    

    // Method to change the balloon's sprite
    public void ChangeSprite(SpriteRenderer spriteRenderer, List<Sprite> sprites)
    {
        if (sprites == null || sprites.Count == 0) return; // Handle empty sprite list

        int randomSpriteIndex = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[randomSpriteIndex]; // Assign a random sprite
    }

    // Path movement for the balloon
    public void PathMovementPattern(Transform balloonInstance, BalloonConfig config)
    {
        if (config.path == null || config.path.Count == 0) return; // Ensure the path is valid

        Vector3[] positions = new Vector3[config.path.Count];

        for (int posIndex = 0; posIndex < config.path.Count; posIndex++)
        {
            Transform item = config.path[posIndex];
            float randomizeX = Random.Range(-2f, 2f);

            // Randomize last position more widely
            if (posIndex == config.path.Count - 1)
            {
                randomizeX = Random.Range(-1f, 8f);
            }

            Vector3 randomizedPosition = new Vector3(item.position.x + randomizeX, item.position.y, item.position.z);
            positions[posIndex] = randomizedPosition;
        }

        // Move the balloon instance along the path and set the OnKill event to reset the balloon
        balloonInstance.DOPath(positions, config.duration, config.pathType, config.pathMode, 10, Color.green)
                       .SetLoops(config.loopCount, config.loopType)
                       .OnKill(() => ResetBalloonPosition(balloonInstance, config.path[0].position)); // Reset position on complete
    }

    // Reset balloon position to the start position
    private void ResetBalloonPosition(Transform balloonInstance, Vector3 startPosition)
    {
        balloonInstance.position = startPosition;
        balloonInstance.gameObject.SetActive(false); // Deactivate for pooling
    }
}

// Enum for different types of balloons
public enum BalloonType
{
    SlowBalloon,
    FastBalloon,
    DangerBalloon,
    ComboBalloon
}

// Configuration class for balloon properties
[System.Serializable]
public class BalloonConfig
{
    public BalloonType type; // Type of the balloon
    public int balloonCount; // Number of balloons to instantiate
    public float duration; // Duration for the path movement
    public PathType pathType; // Type of path for movement
    public PathMode pathMode; // Mode of the path for movement
    public List<Sprite> sprites; // Sprites specific to this balloon type
    public List<Transform> path; // List of paths for balloons
    public int loopCount = -1; // Default loop count
    public LoopType loopType; // Default loop type
}
