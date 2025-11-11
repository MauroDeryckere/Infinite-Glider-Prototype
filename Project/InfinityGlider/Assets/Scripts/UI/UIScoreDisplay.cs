using UnityEngine;
using TMPro;

public class UIScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;

    private void Start()
    {
        UpdateScore(0f, 0f);
    }

    public void UpdateScore(float score, float highscore)
    {
        scoreText.text = $"Score: {score:F0}";
        highscoreText.text = $"Highscore: {highscore:F0}";
    }
}
