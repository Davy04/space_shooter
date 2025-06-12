using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Referência UI")]
    [SerializeField] private TMP_Text _timerText; // Arraste seu objeto Text aqui no Inspector!

    private float _elapsedTime;
    private bool _isRunning = true;

    void Start()
    {
        _elapsedTime = 0f;
    }

    void Update()
    {
        if (_isRunning)
        {
            _elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        if (_timerText != null)
        {
            // Formatação: MM:SS:MS (ex.: 01:23:45)
            int minutes = (int)(_elapsedTime / 60f);
            int seconds = (int)(_elapsedTime % 60f);
            int milliseconds = (int)((_elapsedTime * 1000f) % 1000f) / 10; // 2 dígitos

            _timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
    }

    // ==== Métodos Úteis (opcionais) ====
    public void PauseTimer() => _isRunning = false;
    public void ResumeTimer() => _isRunning = true;
    public void ResetTimer() => _elapsedTime = 0f;
}