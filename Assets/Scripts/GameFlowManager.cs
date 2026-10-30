using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private PlayerStats playerStats;

    [Header("UI Panels")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("UI Text")]
    [SerializeField] private TMP_Text dayText;

    [Header("Winning Settings")]
    [SerializeField] private float daysToSurvive = 7f;

    private float elapsedGameHours;
    private float lastGameTime;

    private bool gameStarted;
    private bool gameEnded;

    private void Start()
    {
        if (dayNightCycle == null)
            dayNightCycle = FindFirstObjectByType<DayNightCycle>();

        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (dayNightCycle == null || playerStats == null)
        {
            Debug.LogError("GameFlowManager references are missing!");
            enabled = false;
            return;
        }

        lastGameTime = dayNightCycle.CurrentTime;

        instructionsPanel.SetActive(true);
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        Time.timeScale = 0f;
        UpdateDayText();
    }

    private void Update()
    {
        if (!gameStarted || gameEnded)
            return;

        UpdateSurvivalTime();

        if (playerStats.IsDead)
        {
            ShowGameOver();
            return;
        }

        if (elapsedGameHours >= daysToSurvive * 24f)
        {
            ShowVictory();
        }
    }

    public void StartGame()
    {
        instructionsPanel.SetActive(false);

        lastGameTime = dayNightCycle.CurrentTime;
        gameStarted = true;

        Time.timeScale = 1f;
    }

    private void UpdateSurvivalTime()
    {
        float currentGameTime = dayNightCycle.CurrentTime;

        float difference = currentGameTime - lastGameTime;

        // Handle midnight
        if (difference < 0f)
            difference += 24f;

        elapsedGameHours += difference;
        lastGameTime = currentGameTime;

        UpdateDayText();
    }

    private void UpdateDayText()
    {
        int currentDay = Mathf.Min(
            Mathf.FloorToInt(elapsedGameHours / 24f) + 1,
            Mathf.CeilToInt(daysToSurvive)
        );

        if (dayText != null)
            dayText.text = "DAY " + currentDay + " / 7";
    }

    private void ShowVictory()
    {
        gameEnded = true;
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;

        Debug.Log("Victory! Player survived 7 days!");
    }

    private void ShowGameOver()
    {
        gameEnded = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        Debug.Log("Game Over!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}