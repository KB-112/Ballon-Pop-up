using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public int score;

    [SerializeField]
    private TMP_Text scoreText;

    public LayerMask playerLayer; // Layer for the regular balloons
    public LayerMask redBalloonLayer; // Layer for the red balloons

    private bool isTouchActive = false; // Track if a touch is currently being processed

    public AudioSource audioSource;
    public AudioClip audioClip;

    public int updateScore;
    public TMP_Text finalScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        score = 100;
        UpdateScoreText();
    }

    void Update()
    {
        Cast();  // Check for touch inputs
    }

    void Cast()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Process the first touch only

            if (touch.phase == TouchPhase.Began) // Only process the touch when it starts
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);

                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, playerLayer | redBalloonLayer);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("RedBalloon"))
                    {
                        hit.collider.gameObject.SetActive(false);  // Deactivate red balloon
                        ReduceScore();  // Reduce score by 100
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Balloon"))
                    {
                        hit.collider.gameObject.SetActive(false);  // Deactivate regular balloon
                        AddScore();  // Add 100 points
                    }
                    audioSource.PlayOneShot(audioClip);

                    isTouchActive = true; // Mark that a touch has been processed
                }
            }
        }
        else
        {
            // No touch input, reset touch active status
            isTouchActive = false;
        }
    }

    void AddScore()
    {
        score += 100;
        UpdateScoreText();
        Debug.Log("Score Updated: " + score);
    }

    void ReduceScore()
    {
        // Ensure the score does not go below zero
        if (score > 0)
        {
            score -= 100;
            if (score < 0)
            {
                score = 0; // Ensure the score stays at zero if it goes negative
            }
            UpdateScoreText();
            Debug.Log("Score Reduced: " + score);
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
            updateScore =score;
            finalScore.text=""+score;
        }
        else
        {
            Debug.LogWarning("Score Text TMP component is not assigned!");
        }
    }

    public int GetScore()
    {
        return score;
    }
}
