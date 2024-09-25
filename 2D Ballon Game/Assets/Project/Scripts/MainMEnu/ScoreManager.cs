using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }  // Singleton instance of ScoreManager
    public int score;  // Current score

    [SerializeField]
    private TMP_Text scoreText;  // UI Text component to display the score

    public LayerMask playerLayer;  // Layer mask for detecting regular balloons
    public LayerMask redBalloonLayer;  // Layer mask for detecting red balloons

    private bool isTouchActive = false;  // Flag to track if a touch is currently being processed

    public AudioSource audioSource;  // Audio source for playing sound effects
    public AudioClip audioClip;  // Audio clip for the touch sound effect

    public int updateScore;  // Variable to update the score elsewhere
    public TMP_Text finalScore;  // UI Text component to display the final score

    void Awake()
    {
        // Initialize singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Ensure only one instance exists
        }

        // Subscribe to the event to restart the game
        SceneLoadingManager.OnPressPlayBtn += RestartGame;
    }

    void Start()
    {
        // Initialize score and update display
        score = 100;
        UpdateScoreText();
    }

    void Update()
    {
        Cast();  // Check for touch inputs each frame
    }

    void Cast()
    {
        // Check if there's any touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);  // Process the first touch

            if (touch.phase == TouchPhase.Began)  // Process touch only when it starts
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);  // Convert touch position to world coordinates

                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, playerLayer | redBalloonLayer);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("RedBalloon"))
                    {
                        hit.collider.gameObject.SetActive(false);  // Deactivate red balloon
                        ReduceScore();  // Reduce score by 200
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Balloon"))
                    {
                        hit.collider.gameObject.SetActive(false);  // Deactivate regular balloon
                        AddScore();  // Add 100 points to score
                    }
                    audioSource.PlayOneShot(audioClip);  // Play touch sound effect

                    isTouchActive = true;  // Mark that a touch has been processed
                }
            }
        }
        else
        {
            // Reset touch status if no touch input is detected
            isTouchActive = false;
        }
    }

    void AddScore()
    {
        score += 100;  // Increase score by 100
        UpdateScoreText();  // Update the score display
        Debug.Log("Score Updated: " + score);  // Log the updated score
    }

    void ReduceScore()
    {
        // Ensure score does not go below zero
        if (score > 0)
        {
            score -= 200;  // Decrease score by 200
            if (score < 0)
            {
                score = 0;  // Clamp score to zero if it goes negative
            }
            UpdateScoreText();  // Update the score display
            Debug.Log("Score Reduced: " + score);  // Log the reduced score
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when this object is destroyed
        SceneLoadingManager.OnPressPlayBtn -= RestartGame;
    }

    void RestartGame()
    {
        score = 100;  // Reset score to 100
        UpdateScoreText();  // Update the score display
        Debug.Log("Game Restarted. Score Reset to 100.");  // Log that the game has restarted
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;  // Update score display text
            updateScore = score;  // Update the external score variable
            finalScore.text = "" + score;  // Update the final score display text
        }
        else
        {
            Debug.LogWarning("Score Text TMP component is not assigned!");  // Warn if the scoreText component is missing
        }
    }

    public int GetScore()
    {
        return score;  // Return the current score
    }
}
