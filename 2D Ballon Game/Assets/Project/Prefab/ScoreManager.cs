using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private int score;

    // Reference to the TextMeshProUGUI components
    [SerializeField]
    private TMP_Text scoreText; // Drag and drop your TMP Text component in the inspector
    [SerializeField]
    private TMP_Text timerText; // Drag and drop your Timer Text component in the inspector

    private float timer; // Timer in seconds
    private bool isTimerRunning;

    private void Awake()
    {
        // Ensure only one instance of ScoreManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this manager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        score = 0; // Initialize score
        timer = 45f; // Set the timer to 45 seconds
        isTimerRunning = true; // Start the timer
        UpdateScoreText(); // Update the score text on start
        UpdateTimerText(); // Update the timer text on start
    }

    private void Update()
    {
        // Update the timer if it's running
        if (isTimerRunning)
        {
            timer -= Time.deltaTime; // Decrease timer
            if (timer <= 0)
            {
                timer = 0; // Clamp timer to zero
                isTimerRunning = false; // Stop the timer
                OnTimerEnd(); // Call method when timer ends
            }
            UpdateTimerText(); // Update the timer display
        }
    }

    public void AddScore(BalloonType balloonType)
    {
        switch (balloonType)
        {
            case BalloonType.SlowBalloon:
                score += 100; // Increase score for hitting a Slow Balloon
                break;
            case BalloonType.FastBalloon:
                score += 200; // Increase score for hitting a Fast Balloon
                break;
            case BalloonType.DangerBalloon:
                score -= 500; // Decrease score for hitting a Danger Balloon
                break;
            case BalloonType.ComboBalloon:
                // Define logic for Combo Balloon if needed
                break;
            default:
                break;
        }

        UpdateScoreText(); // Update the text after score change
        Debug.Log("Score Updated: " + score);
    }

    public void MissBalloon()
    {
        score -= 50; // Penalty for missing a balloon
        UpdateScoreText(); // Update the text after penalty
        Debug.Log("Score Updated: " + score);
    }

    public int GetScore()
    {
        return score; // Return the current score
    }

    // Method to update the TMP text with the current score
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score; // Set the TMP text to show the current score
        }
        else
        {
            Debug.LogWarning("Score Text TMP component is not assigned!");
        }
    }

    // Method to update the TMP text with the current timer
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString(); // Display the remaining time
        }
        else
        {
            Debug.LogWarning("Timer Text TMP component is not assigned!");
        }
    }

    // Method to call when the timer reaches zero
    private void OnTimerEnd()
    {
        // Logic for when the timer ends (e.g., end game, show final score, etc.)
        Debug.Log("Time's up! Final Score: " + score);
        // You can trigger any end game logic here
    }
}
