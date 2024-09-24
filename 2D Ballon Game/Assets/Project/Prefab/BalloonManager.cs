using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BalloonManager : MonoBehaviour
{
    public static BalloonManager Instance { get; private set; }
    public List<BalloonConfig> balloonConfigs; // Public to set in Inspector
    public Transform balloonPrefab;

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
    }

    // Method to instantiate balloons based on a configuration and return the instantiated GameObject
    public GameObject InstantiateBalloons(BalloonConfig config)
    {
        List<GameObject> balloonInstances = new List<GameObject>();

        for (int i = 0; i < config.balloonCount; i++)
        {
            Transform balloonInstance = Instantiate(balloonPrefab, config.path[0].position, Quaternion.identity);
            SpriteRenderer spriteRenderer = balloonInstance.GetComponent<SpriteRenderer>();
            ChangeSprite(spriteRenderer, config.sprites); // Change the sprite before starting the movement
            PathMovementPattern(balloonInstance, config); // Move the balloon along the path

            // Add to the list of instantiated balloons
            balloonInstances.Add(balloonInstance.gameObject);
        }

        // Optionally return the last balloon instantiated or any specific logic to return
        return balloonInstances.Count > 0 ? balloonInstances[0] : null;
    }

    // Select balloon configuration based on score
 public BalloonConfig GetBalloonConfigBasedOnScore(int score)
{
    // Logic to determine which balloon config to return based on the score
    // For example:
    if (score < 500)
    {
        return balloonConfigs.Find(config => config.type == BalloonType.SlowBalloon);
    }
    else if (score < 1000)
    {
        return balloonConfigs.Find(config => config.type == BalloonType.FastBalloon);
    }
    else
    {
        return balloonConfigs.Find(config => config.type == BalloonType.DangerBalloon);
    }
}


    private void ChangeSprite(SpriteRenderer spriteRenderer, List<Sprite> sprites)
    {
        if (sprites.Count > 0)
        {
            int randomSpriteIndex = Random.Range(0, sprites.Count);
            spriteRenderer.sprite = sprites[randomSpriteIndex]; // Assign a random sprite
        }
    }

    private void PathMovementPattern(Transform balloonInstance, BalloonConfig config)
    {
        Vector3[] positions = new Vector3[config.path.Count];

        for (int posIndex = 0; posIndex < config.path.Count; posIndex++)
        {
            Transform item = config.path[posIndex];
            float randomizeX = Random.Range(-2f, 2f);

            // Randomize last position more widely
            if (posIndex == config.path.Count - 1)
            {
                randomizeX = Random.Range(-5f, 5f);
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
        balloonInstance.position = startPosition; // Reset position
        balloonInstance.gameObject.SetActive(false); // Optionally deactivate for pooling
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
