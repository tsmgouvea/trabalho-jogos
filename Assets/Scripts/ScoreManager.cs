using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Referência ao componente de texto
    private int score;

    void Start()
    {
        score = 0;
        UpdateScoreText();
        score = score - 3;
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
        score = score - 3;
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public int GetScore()
    {
        return score;
    }
}
