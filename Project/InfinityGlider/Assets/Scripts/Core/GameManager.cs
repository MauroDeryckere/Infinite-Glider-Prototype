using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isGameOver = false;

    private float score = 0f;
    private float highScore = 0f;

    public float Score => score;
    public float HighScore => highScore;

    private UIScoreDisplay display;

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");

        display = FindAnyObjectByType<UIScoreDisplay>();

        Time.timeScale = 1f;

        isGameOver = false;
        score = 0f;

        LoadHighScore();
        display?.UpdateScore(score, highScore);
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
            SaveHighScore();
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


        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SaveHighScore()
    {
        SaveData data = new SaveData { highScore = highScore };
        File.WriteAllText(savePath, JsonUtility.ToJson(data));
        Debug.Log($"Highscore saved to: {savePath}");
    }

    private void LoadHighScore()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            highScore = data.highScore;
            Debug.Log($"Loaded highscore: {highScore}");
        }
        else
        {
            Debug.Log("No save file found, starting fresh.");
        }
    }
}
