using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private KeyCode pauseKey = KeyCode.P;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject creditsPanel;

    public static bool isPaused = false;
    private static bool returningFromMenu = false;

    private void Awake()
    {
        // Garante que o jogo comece no estado correto
        if (returningFromMenu)
        {
            returningFromMenu = false;
            ForceResumeState();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey) && !UICountdown.IsCountdownActive())
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        InputManager.EnableControls(false);
        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        isPaused = true;
    }

    public void ResumeGame()
    {
        StartCoroutine(ResumeWithDelay());
    }

    private IEnumerator ResumeWithDelay()
    {
        yield return null;
        ForceResumeState();
    }

    private void ForceResumeState()
    {
        InputManager.EnableControls(true);
        pauseMenuUI.SetActive(false);
        creditsPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;
        
        ResetAllInputs();
        
        // Reinicia todos os sistemas importantes
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = true;
            player.FreezePlayer(false);
        }
    }

    private void ResetAllInputs()
    {
        Input.ResetInputAxes();
    }

    public void ShowCredits()
    {
        pauseMenuUI.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        creditsPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        returningFromMenu = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}