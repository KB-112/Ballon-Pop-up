using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverBoard;
    public int hitCount = 0;
    public int maxHits = 3;

    [SerializeField]
    private TMP_Text timerText;

    private float timer = 45f;
    private bool isTimerRunning = true;
    private int zeroScoreCount = 0; // Track how many times the score has been zero

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

        CheckScoreZero(); // Check the score in the update loop
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
            zeroScoreCount = 0; // Reset zero score count if score is not zero
        }
    }

    void GameOverBoard()
    {
        gameOverBoard.SetActive(true);
        Time.timeScale = 0;
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString();
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
