using UnityEngine;
using UnityEngine.SceneManagement;
    
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnPlayerDied()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        Debug.Log("Game Over!");

        // Freeze time or fade out if desired
        Time.timeScale = 0f;

        // Simple restart after short delay
        StartCoroutine(RestartAfterDelay(2f));
    }

    private System.Collections.IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
