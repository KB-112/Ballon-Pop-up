using System.Collections.Generic;
using UnityEngine;

public class BalloonPath : MonoBehaviour
{
    public List<GameObject> objectPool; // Object pool to hold balloon instances

    private void Start()
    {
        // Initialize the object pool
        objectPool = new List<GameObject>();

        // Instantiate balloons based on the score
        InstantiateBalloonsBasedOnScore();
    }

    private void InstantiateBalloonsBasedOnScore()
    {
        // Check if BalloonManager has balloon configurations
        if (BalloonManager.Instance == null || BalloonManager.Instance.balloonConfigs.Count == 0)
        {
            Debug.LogError("BalloonManager or its balloonConfigs is not set up correctly.");
            return; // Exit if not properly set up
        }

        // Get the current score from ScoreManager
        int playerScore = ScoreManager.Instance.GetScore();

        // Select the balloon configuration based on score
        BalloonConfig selectedConfig = BalloonManager.Instance.GetBalloonConfigBasedOnScore(playerScore);

        // Use BalloonManager to instantiate the balloons for the selected config
        GameObject temp = BalloonManager.Instance.InstantiateBalloons(selectedConfig);
        if (temp != null)
        {
            objectPool.Add(temp);
        }
    }
}
