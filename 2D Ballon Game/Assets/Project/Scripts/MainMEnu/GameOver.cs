using System;
using UnityEngine;
using UnityEngine.UI; // Make sure to add this for using the Image component
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverBoard;
    public int hitCount = 0;
    public int maxHits = 3;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image sliderImage; // Add this line for the image slider
    private float timer = 45f;
    private bool isTimerRunning = true;
    private int zeroScoreCount = 0;

    public AudioSource audioSource;

    // Define an event to notify when the game is over
    public static event Action OnGameOver;

    void Start()
    {
        gameOverBoard.SetActive(false);
        UpdateTimerText();
        UpdateSlider(); // Initialize the slider
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                isTimerRunning = false;
                OnTimerEnd();
            }
            UpdateTimerText();
        }

        CheckScoreZero();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Balloon"))
        {
            hitCount++;

            // Update slider image based on hits
            UpdateSlider();

            if (hitCount >= maxHits)
            {
                GameOverBoard();
            }
        }
    }

    void CheckScoreZero()
    {
        if (ScoreManager.Instance.updateScore == 0)
        {
            zeroScoreCount++;

            if (zeroScoreCount >= 3)
            {
                GameOverBoard();
            }
        }
        else
        {
            zeroScoreCount = 0;
        }
    }

    void GameOverBoard()
    {
        gameOverBoard.SetActive(true);
        Time.timeScale = 0;
        audioSource.Play();
          hitCount = 0;
        isTimerRunning = true;
        timer = 45f;
        zeroScoreCount = 0;

        // Reset slider
        if (sliderImage != null)
        {
            sliderImage.fillAmount = 0f;
        }
        

        // Trigger the game over event
        OnGameOver?.Invoke();
    }


    void Reset()
    {
       
       
       
          hitCount = 0;
        isTimerRunning = true;
        timer = 45f;
        zeroScoreCount = 0;

    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "" + Mathf.Ceil(timer).ToString();
        }
        else
        {
            Debug.LogWarning("Timer Text TMP component is not assigned!");
        }
    }


 void Awake()
    {
        
            SceneLoadingManager.OnPressPlayBtn += Reset;
    }


    void OnDestroy()
    {
            SceneLoadingManager.OnPressPlayBtn -= Reset;

    }

    private void OnTimerEnd()
    {
        Debug.Log("Time's up! Game Over!");
        GameOverBoard();
    }

    void UpdateSlider()
    {
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
