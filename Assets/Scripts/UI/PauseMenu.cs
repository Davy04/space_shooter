using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private KeyCode pauseKey = KeyCode.P;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject creditsPanel;

    public static bool isPaused = false; // Static para acesso de outros scripts

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            Debug.Log("pausou");
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

        // Configurações do cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        isPaused = true;

        Debug.Log("Jogo pausado"); // Para verificar no console
    }

    public void ResumeGame()
    {
        StartCoroutine(ResumeWithDelay());
    }

    private IEnumerator ResumeWithDelay()
    {
        yield return null;
        InputManager.EnableControls(true);
        pauseMenuUI.SetActive(false);
        creditsPanel.SetActive(false);
        Time.timeScale = 1f;

        // Garante que o cursor está configurado corretamente
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;

        // Reseta todos os inputs
        ResetAllInputs();
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
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Substitua pelo nome da sua cena
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Para sair no Editor
#endif
    }
}