using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [Header("Game Over Menu")]
    [SerializeField]
    private GameObject _gameOverMenu;

    [Header("Text Fields")]
    [SerializeField]
    private TextMeshProUGUI _totalScoreText;
    [SerializeField]
    private TextMeshProUGUI _highestDifficultyText;
    [SerializeField]
    private TextMeshProUGUI _linesClearedText;
    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    [Header("Game State")]
    [SerializeField]
    private GameState _gameState;

    [Header("Button to select")]
    [SerializeField]
    private Button _firstSelectedButton;

    [Header("Game Objects to hide")]
    [SerializeField]
    private GameObject[] _objectsToHide;


    public void GoToMainMenu()
    {
        Utils.GameToMenu();
    }

    public void RestartGame()
    {
        Utils.RestartCurrentScene();
    }

    public void SetupGameOverMenu()
    {
        _gameOverMenu.SetActive(true);

        _firstSelectedButton.Select();
        _totalScoreText.text = _gameState.TotalScore.ToString();
        _highestDifficultyText.text = _gameState.HighestDifficulty.ToString();
        _linesClearedText.text = _gameState.TotalLinesCleared.ToString();

        var encryptedSaveData = SaveManager.ReadSaveData();
        // If save file exists and is not empty
        if (!string.IsNullOrEmpty(encryptedSaveData))
        {
            var saveDataJson = EncryptionManager.EncryptDecrypt(encryptedSaveData, 739);
            var saveData = JsonUtility.FromJson<SaveData>(saveDataJson);
            // If totals score is bigger than the previous high score, then save it as new high score
            if (_gameState.TotalScore > saveData.HighiScore)
            {
                NewHighScore();
            }
            else
            {
                _highScoreText.text = $"High Score: {saveData.HighiScore}";
            }
        }
        else
        {
            NewHighScore();
        }

        foreach (GameObject obj in _objectsToHide)
        {
            obj.SetActive(false);
        }
    }

    private void NewHighScore()
    {
        _highScoreText.text = $"New High Score! {_gameState.TotalScore}";
        var saveDataJson = JsonUtility.ToJson(new SaveData(_gameState.TotalScore));
        var encryptedSaveData = EncryptionManager.EncryptDecrypt(saveDataJson, 739);
        SaveManager.WriteSaveData(encryptedSaveData);
    }

    private void OnEnable()
    {
        GameState.OnGameOverAction += SetupGameOverMenu;
    }

    private void OnDisable()
    {
        GameState.OnGameOverAction -= SetupGameOverMenu;
    }

    [Serializable]
    private struct SaveData
    {
        public int HighiScore;

        public SaveData(int score)
        {
            HighiScore = score;
        }
    }
}
