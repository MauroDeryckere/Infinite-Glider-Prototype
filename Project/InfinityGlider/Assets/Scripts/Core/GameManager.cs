using UnityEngine;
using UnityEngine.SceneManagement;
    
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isGameOver = false;

    private float score = 0f;
    private float highScore = 0f;

    public float Score => score;
    public float HighScore => highScore;

    private UIScoreDisplay display;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }

    private void Start()
    {
        display = FindAnyObjectByType<UIScoreDisplay>();
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }
            
        score += Time.deltaTime * 10f;

        if (display != null)
        {
            display.UpdateScore(score, highScore);  
        }
    }

    public void OnPlayerDied()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        Debug.Log($"Game Over! Final Score: {score:F0}");

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (display != null)
        {
            display.UpdateScore(0f, highScore);
        }

        Time.timeScale = 0f;
        StartCoroutine(RestartAfterDelay(2f));
    }

    private System.Collections.IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
