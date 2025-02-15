using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TetroManager : MonoBehaviour
{
    public static TetroManager Instance;

    [Header("UI Elemanları")]
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public TMP_Text gameOverScoreText;
    public TMP_Text highScoreText;

    [Header("Pause Paneli")]
    public GameObject pausePanel; 
    public Button pauseButton; 
    public Button resumeButton; 
    public Button menuButton; 

    [Header("Zorluk Seçim Paneli")]
    public GameObject difficultyPanel;
    public Slider difficultySlider;
    public TMP_Text difficultyText;

    private int currentScore = 0;
    public static float pieceStepDelay = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 0f;

        float savedDifficulty = PlayerPrefs.GetFloat("TetroDifficulty", 1);
        difficultySlider.value = savedDifficulty;

        difficultySlider.onValueChanged.AddListener(OnDifficultySliderChanged);

        UpdateDifficultyText(savedDifficulty);

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
        }

        if (pauseButton != null) pauseButton.onClick.AddListener(PauseGame);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (menuButton != null) menuButton.onClick.AddListener(ExitToMenu);
    }

    public void OnDifficultySliderChanged(float value)
    {
        UpdateDifficultyText(value);
    }

    private void UpdateDifficultyText(float value)
    {
        if (difficultyText != null)
        {
            if (value == 0)
            {
                difficultyText.text = "Easy";
                pieceStepDelay = 1.0f;
            }
            else if (value == 1)
            {
                difficultyText.text = "Medium";
                pieceStepDelay = 0.5f;
            }
            else if (value == 2)
            {
                difficultyText.text = "Hard";
                pieceStepDelay = 0.3f;
            }
        }
    }

    public void StartGame()
    {
        PlayerPrefs.SetFloat("TetroDifficulty", difficultySlider.value);
        PlayerPrefs.Save();

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void AddScore(int points)
    {
        currentScore += points;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }

    public void GameOver()
    {
        int highScore = PlayerPrefs.GetInt("TetroHighScore", 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("TetroHighScore", currentScore);
        }

        if (gameOverScoreText != null) gameOverScoreText.text = "Score: " + currentScore;
        if (highScoreText != null) highScoreText.text = "High Score: " + PlayerPrefs.GetInt("TetroHighScore", 0);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }


    public void PauseGame()
    {
        Time.timeScale = 0f; 
        if (pausePanel != null) pausePanel.SetActive(true); 
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; 
        if (pausePanel != null) pausePanel.SetActive(false); 
    }
}