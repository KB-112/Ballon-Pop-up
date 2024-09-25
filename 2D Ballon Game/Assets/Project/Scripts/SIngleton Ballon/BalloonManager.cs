using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BalloonManager : MonoBehaviour
{
    public static BalloonManager Instance { get; private set; } // Singleton instance
    public List<BalloonConfig> balloonConfigs; // Balloon configurations
    public GameObject balloonPrefab; // Prefab for the balloon
    [HideInInspector] public List<GameObject> pooledObjects; // List of pooled balloon objects

    public int amountToPool; // Number of balloons to pool

    private void Awake()
    {
        // Ensure only one instance of BalloonManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeBalloon(); // Initialize balloon pool
    }

    public void InitializeBalloon()
    {
        // Create and initialize balloon pool
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < amountToPool; i++)
        {
            GameObject newBalloon = Instantiate(balloonPrefab);
            newBalloon.SetActive(false);
            pooledObjects.Add(newBalloon);
        }
    }

    public void AssignLayer(GameObject balloon, int configIndex)
    {
        // Set the layer for the balloon based on the configuration
        int layerIndex = Mathf.FloorToInt(Mathf.Log(balloonConfigs[configIndex].layer.value, 2));
        balloon.layer = layerIndex;
    }

    public List<GameObject> GetPooledObject()
    {
        return pooledObjects; // Return the list of pooled balloon objects
    }

    public void ChangeSprite(SpriteRenderer spriteRenderer, List<Sprite> sprites)
    {
        // Change the balloon's sprite to a random one from the list
        if (sprites == null || sprites.Count == 0) return; // Check if sprites list is valid

        int randomSpriteIndex = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[randomSpriteIndex];
    }

    public void PathMovementPattern(Transform balloonInstance, BalloonConfig config)
    {
        // Move the balloon along a path with randomization
        if (config.path == null || config.path.Count == 0) return; // Check if path is valid

        Vector3[] positions = new Vector3[config.path.Count];

        for (int posIndex = 0; posIndex < config.path.Count; posIndex++)
        {
            Transform item = config.path[posIndex];
            float randomizeX = (posIndex == 4) ? Random.Range(0f, 8f) : Random.Range(0, 4f);
            Vector3 randomizedPosition = new Vector3(item.position.x + randomizeX, item.position.y, item.position.z);
            positions[posIndex] = randomizedPosition;
        }

        // Animate balloon along the path
        balloonInstance.DOPath(positions, config.duration, config.pathType, config.pathMode, 10, Color.green)
               .SetEase(Ease.Linear)
               .SetLoops(config.loopCount, config.loopType)
               .OnKill(() => ResetBalloonPosition(balloonInstance, config.path[0].position)); // Reset position on completion
    }

    public void ResetBalloonPosition(Transform balloonInstance, Vector3 startPosition)
    {
        // Reset the balloon's position and deactivate it
        balloonInstance.position = startPosition;
        balloonInstance.gameObject.SetActive(false);
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
    public LayerMask layer; // Layer mask for balloon
    public BalloonType type; // Type of the balloon
    public int balloonCount; // Number of balloons
    public float duration; // Duration for path movement
    public PathType pathType; // Type of path
    public PathMode pathMode; // Mode of path
    public List<Sprite> sprites; // Sprites for this balloon type
    public List<Transform> path; // Path for balloon movement
    public int loopCount = 0; 
    public LoopType loopType; // Type of loop
}
