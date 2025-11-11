using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private UIScoreDisplay display;

    private bool isGameOver = false;

    private float score = 0f;
    private float highScore = 0f;

    public float Score => score;
    public float HighScore => highScore;

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        savePath = Path.Combine(Application.persistentDataPath, "save.json");

        LoadHighScore();
    }

    private void Start()
    {
        display?.UpdateScore(score, highScore);
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }
            
        score += Time.deltaTime * 10f;
        display?.UpdateScore(score, highScore);  
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

        gameOverUI?.Show(score, highScore);
        display?.Hide();

        Time.timeScale = 0f;
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
