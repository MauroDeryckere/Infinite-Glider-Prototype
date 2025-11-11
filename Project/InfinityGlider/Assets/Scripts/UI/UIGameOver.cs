using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        panel.SetActive(false);

        restartButton.onClick.AddListener(OnRestartPressed);
        quitButton.onClick.AddListener(OnQuitPressed);
    }

    public void Show(float finalScore, float highScore)
    {
        panel.SetActive(true);
        scoreText.text = $"Score: {finalScore:F0}";
        highScoreText.text = $"Highscore: {highScore:F0}";
    }

    public void OnRestartPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
