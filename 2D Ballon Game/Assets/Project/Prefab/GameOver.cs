using System;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverBoard;
    public int hitCount = 0;
    public int maxHits = 3;

    [SerializeField] private TMP_Text timerText;
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

        // Trigger the game over event
        OnGameOver?.Invoke();
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

    private void OnTimerEnd()
    {
        Debug.Log("Time's up! Game Over!");
        GameOverBoard();
    }
}
