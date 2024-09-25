using System;
using UnityEngine;
using UnityEngine.UI; // For using the Image component
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverBoard;  // UI element to display when the game is over
    public int hitCount = 0;  // Count of hits before game over
    public int maxHits = 3;  // Maximum number of hits allowed before game over

    [SerializeField] private TMP_Text timerText;  // UI text element for displaying timer
    [SerializeField] private Image sliderImage;  // UI image element for the slider
    private float timer = 45f;  // Timer duration
    private bool isTimerRunning = true;  // Flag to check if the timer is running
    private int zeroScoreCount = 0;  // Count of frames with a score of zero

    public AudioSource audioSource;  // Audio source to play sound effects

    // Event to notify when the game is over
    public static event Action OnGameOver;

    void Start()
    {
        // Initialize the game state
        gameOverBoard.SetActive(false);
        UpdateTimerText();  // Display initial timer value
        UpdateSlider();  // Initialize slider display
    }

    void Update()
    {
        if (isTimerRunning)
        {
            // Update timer and check for time end
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                isTimerRunning = false;
                OnTimerEnd();
            }
            UpdateTimerText();  // Update timer display
        }

        CheckScoreZero();  // Check if score is zero and handle accordingly
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is a balloon
        if (collision.gameObject.layer == LayerMask.NameToLayer("Balloon"))
        {
            hitCount++;  // Increment hit count
            UpdateSlider();  // Update slider based on current hit count

            // Check if hit count has reached maximum
            if (hitCount >= maxHits)
            {
                GameOverBoard();  // Trigger game over
            }
        }
    }

    void CheckScoreZero()
    {
        // Check if the score is zero and update zero score count
        if (ScoreManager.Instance.updateScore == 0)
        {
            zeroScoreCount++;

            if (zeroScoreCount >= 3)
            {
                GameOverBoard();  // Trigger game over
            }
        }
        else
        {
            zeroScoreCount = 0;  // Reset zero score count if score is not zero
        }
    }

    void GameOverBoard()
    {
        // Display game over board and stop the game
        gameOverBoard.SetActive(true);
        Time.timeScale = 0;
        audioSource.Play();  // Play game over sound
        hitCount = 0;
        isTimerRunning = true;
        timer = 45f;
        zeroScoreCount = 0;

        // Reset slider to initial state
        if (sliderImage != null)
        {
            sliderImage.fillAmount = 0f;
        }

        // Trigger game over event
        OnGameOver?.Invoke();
    }

    void Reset()
    {
        // Reset game state for a new game
        hitCount = 0;
        isTimerRunning = true;
        timer = 45f;
        zeroScoreCount = 0;
    }

    private void UpdateTimerText()
    {
        // Update the timer text display
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(timer).ToString();
        }
        else
        {
            Debug.LogWarning("Timer Text TMP component is not assigned!");
        }
    }

    void Awake()
    {
        // Subscribe to the event for resetting the game
        SceneLoadingManager.OnPressPlayBtn += Reset;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneLoadingManager.OnPressPlayBtn -= Reset;
    }

    private void OnTimerEnd()
    {
        // Handle actions when the timer ends
        Debug.Log("Time's up! Game Over!");
        GameOverBoard();  // Trigger game over
    }

    void UpdateSlider()
    {
        // Update the slider based on hit count
        if (sliderImage != null)
        {
            float fillAmount = (float)hitCount / maxHits;
            sliderImage.fillAmount = fillAmount;
        }
        else
        {
            Debug.LogWarning("Slider Image component is not assigned!");
        }
    }
}
