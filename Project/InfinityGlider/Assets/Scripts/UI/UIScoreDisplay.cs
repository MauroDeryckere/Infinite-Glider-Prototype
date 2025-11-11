using UnityEngine;
using TMPro;

public class UIScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private GameObject panel;

    private void Start()
    {

    }

    public void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void UpdateScore(float score, float highscore)
    {
        scoreText.text = $"Score: {score:F0}";
        highscoreText.text = $"Highscore: {highscore:F0}";
    }
}
