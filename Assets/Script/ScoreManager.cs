using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;

    [Header("Score Settings")]
    public int matchPoints = 100;
    public int mismatchPenalty = 20;

    private int score = 0;


    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreUI();
    }


    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void AddMatchScore()
    {
        score += matchPoints;
        UpdateScoreUI();
    }

    public void AddMismatchPenalty()
    {
        score -= mismatchPenalty;
        if (score < 0) score = 0;

        UpdateScoreUI();
    }

    public void RestoreScore(int restoredScore)
    {
        score = restoredScore;
        UpdateScoreUI();
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
}