using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private int score;

    [SerializeField]
    private TMP_Text scoreText;

    public LayerMask playerLayer; // Layer for the regular balloons
    public LayerMask redBalloonLayer; // Layer for the red balloons
    public GameObject balloonPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        score = 0;
        UpdateScoreText();

        Vector2 pos = new Vector2(0, Random.Range(0, 4));
       
    }

    void Update()
    {
        Cast();  // Check for touch inputs
    }

    void Cast()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);

            if (Input.GetTouch(i).phase == TouchPhase.Stationary)
            {
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
                }
            }
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
        score -= 100;
        UpdateScoreText();
        Debug.Log("Score Reduced: " + score);
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
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
