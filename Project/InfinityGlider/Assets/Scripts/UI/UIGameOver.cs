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
        gameObject.SetActive(false);

        restartButton.onClick.AddListener(OnRestartPressed);
        quitButton.onClick.AddListener(OnQuitPressed);
    }

    public void Show(float finalScore, float highScore)
    {
        panel.SetActive(true);
        scoreText.text = $"Score: {Mathf.FloorToInt(finalScore)}";
        highScoreText.text = $"Highscore: {Mathf.FloorToInt(highScore)}";
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
