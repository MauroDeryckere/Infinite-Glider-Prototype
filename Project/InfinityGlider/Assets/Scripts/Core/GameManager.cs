using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private UIScoreDisplay display;

    private bool isGameOver = false;

    public bool IsGameOver()
    {
        return isGameOver;
    }

    private float score = 0f;
    private float highScore = 0f;

    public float Score => score;
    public float HighScore => highScore;

    private string savePath;
    private bool hasBeatHighScore = false;

    [Header("Sound")]
    [SerializeField] private AudioClip highScoreBeatSound;
    [SerializeField] private float highScoreBeatVolume = 0.8f;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // ensure 2D sound

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

        if (score > highScore)
        {
            if (!hasBeatHighScore)
            {
                // Play sound, display visuals
                if (highScoreBeatSound)
                {
                    audioSource.PlayOneShot(highScoreBeatSound, highScoreBeatVolume);
                }

                Debug.Log("New High Score!");

            }

            hasBeatHighScore = true;
            highScore = score;
        }

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
