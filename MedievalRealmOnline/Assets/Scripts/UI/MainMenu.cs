using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject characterCreationPanel;
    public GameObject optionsPanel;

    [Header("Main Menu Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Character Creation")]
    public CharacterCreator characterCreator;

    [Header("Visual")]
    public Image backgroundImage;
    public float transitionSpeed = 0.5f;

    void Start()
    {
        SetupButtons();
        CheckSavedGame();
    }

    void SetupButtons()
    {
        if (newGameButton != null)
            newGameButton.onClick.AddListener(StartNewGame);

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (optionsButton != null)
            optionsButton.onClick.AddListener(OpenOptions);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void CheckSavedGame()
    {
        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.HasSavedGame();
        }
    }

    public void StartNewGame()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (characterCreationPanel != null)
            characterCreationPanel.SetActive(true);
    }

    void ContinueGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame();
        }
    }

    void OpenOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void BackToMainMenu()
    {
        if (characterCreationPanel != null)
            characterCreationPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }
}