using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public GameSettings settings;

    [Header("Current Game State")]
    public GameData currentGame;

    [Header("UI References")]
    public GameObject mainMenuCanvas;
    public GameObject gameCanvas;
    public GameObject pauseCanvas;

    [Header("Player Reference")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    private bool isPaused = false;
    private bool isGameStarted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    void Start()
    {
        ShowMainMenu();
    }

    void Update()
    {
        if (isGameStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    #region Scene Management

    public void ShowMainMenu()
    {
        isGameStarted = false;
        isPaused = false;
        Time.timeScale = 1f;

        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (gameCanvas != null) gameCanvas.SetActive(false);
        if (pauseCanvas != null) pauseCanvas.SetActive(false);

        SceneManager.LoadScene("MainMenu");
    }

    public void StartNewGame()
    {
        currentGame = new GameData();
        isGameStarted = true;

        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (gameCanvas != null) gameCanvas.SetActive(true);

        SceneManager.LoadScene("GameScene");
    }

    public void LoadGame()
    {
        if (SaveSystem.LoadGame())
        {
            currentGame = SaveSystem.LoadGame();
            isGameStarted = true;

            if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
            if (gameCanvas != null) gameCanvas.SetActive(true);

            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.Log("No saved game found!");
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseCanvas != null) pauseCanvas.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseCanvas != null) pauseCanvas.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        isGameStarted = false;
        isPaused = false;
        Time.timeScale = 1f;
        ShowMainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    #endregion

    #region Save/Load

    public void SaveGame()
    {
        SaveSystem.SaveGame(currentGame);
    }

    public bool HasSavedGame()
    {
        return SaveSystem.HasSavedGame();
    }

    #endregion

    #region Game Logic

    public void PlayerDied()
    {
        // Handle player death
        currentGame.playerData.level = Mathf.Max(1, currentGame.playerData.level - 1);
        currentGame.playerData.xp = 0;
        currentGame.playerData.gold = Mathf.Max(0, currentGame.playerData.gold / 2);
        currentGame.playerData.health = currentGame.playerData.maxHealth / 2;
        currentGame.playerData.position = new Vector3(0, 0, 0);

        // Show death screen UI
        UIManager.Instance.ShowDeathScreen();
    }

    public void LevelUp()
    {
        currentGame.playerData.level++;
        currentGame.playerData.xp -= currentGame.playerData.xpToLevel;
        currentGame.playerData.xpToLevel = Mathf.Floor(settings.baseXpToLevel * Mathf.Pow(settings.xpMultiplier, currentGame.playerData.level));

        // Apply stat increases
        currentGame.playerData.maxHealth += 20;
        currentGame.playerData.maxMana += 10;
        currentGame.playerData.health = currentGame.playerData.maxHealth;
        currentGame.playerData.mana = currentGame.playerData.maxMana;
        currentGame.playerData.attackDamage += 2;
        currentGame.playerData.magicPower += 1;

        // Update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLevelUpEffect();
        }
    }

    public void ChangeArea(string areaName)
    {
        currentGame.currentArea = areaName;
        SceneManager.LoadScene(areaName);
    }

    #endregion
}