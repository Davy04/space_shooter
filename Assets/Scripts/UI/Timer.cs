using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text[] _bestTimeTexts;
    [SerializeField] private GameObject _highScorePopup;
    [SerializeField] private TMP_InputField _nameInputField;

    private float _elapsedTime;
    private bool _isRunning = true;
    private const string HIGH_SCORES_KEY = "HighScores";
    private List<HighScoreEntry> _highScores = new List<HighScoreEntry>();

    private void Start()
    {
        LoadHighScores();
        UpdateHighScoresUI();
    }

    void Update()
    {
        if (_isRunning && !IsHighScorePopupActive())
        {
            _elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void StopAndSaveTime()
    {
        if (!_isRunning) return;

        _isRunning = false;
        
        Time.timeScale = 0f;
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        _highScorePopup.SetActive(true);
        
        _nameInputField.Select();
        _nameInputField.ActivateInputField();
    }

    public void SavePlayerScore()
    {
        string playerName = string.IsNullOrEmpty(_nameInputField.text) ? "An�nimo" : _nameInputField.text;

        _highScores.Add(new HighScoreEntry(playerName, _elapsedTime));
        _highScores = _highScores.OrderBy(score => score.time).Take(5).ToList();
        SaveHighScores();
        UpdateHighScoresUI();

        _highScorePopup.SetActive(false);
        
        Time.timeScale = 1f;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LoadHighScores()
    {
        string json = PlayerPrefs.GetString(HIGH_SCORES_KEY, "");

        if (!string.IsNullOrEmpty(json))
        {
            HighScoreList wrapper = JsonUtility.FromJson<HighScoreList>(json);
            _highScores = wrapper.highScores;
        }
    }

    private void SaveHighScores()
    {
        HighScoreList wrapper = new HighScoreList { highScores = _highScores };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(HIGH_SCORES_KEY, json);
        PlayerPrefs.Save();
    }

    private void UpdateTimerUI()
    {
        if (_timerText != null)
        {
            _timerText.text = FormatTime(_elapsedTime);
        }
    }

    private void UpdateHighScoresUI()
    {
        for (int i = 0; i < _bestTimeTexts.Length; i++)
        {
            if (i < _highScores.Count)
            {
                _bestTimeTexts[i].text = $"{i + 1}. {_highScores[i].playerName} - {FormatTime(_highScores[i].time)}";
            }
            else
            {
                _bestTimeTexts[i].text = $"{i + 1}. --:--:--";
            }
        }
    }

    public static string FormatTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        int milliseconds = (int)((time * 1000f) % 1000f) / 10;
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public bool IsHighScorePopupActive()
    {
        return _highScorePopup != null && _highScorePopup.activeInHierarchy;
    }
    
    public void PauseTimer(bool pause)
    {
        _isRunning = !pause;
    }
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScoreEntry> highScores;
}