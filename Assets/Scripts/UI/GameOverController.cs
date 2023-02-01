using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameOverMenu;
    [SerializeField]
    private TextMeshProUGUI _totalScoreText;
    [SerializeField]
    private TextMeshProUGUI _highestDifficultyText;
    [SerializeField]
    private TextMeshProUGUI _linesClearedText;
    // TODO: high score
    //[SerializeField]
    //private TextMeshProUGUI _highScoreText;
    [SerializeField]
    private GameState _gameState;
    [SerializeField]
    private Button _firstSelectedButton;


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
    }

    private void OnEnable()
    {
        GameState.OnGameOverAction += SetupGameOverMenu;
    }

    private void OnDisable()
    {
        GameState.OnGameOverAction -= SetupGameOverMenu;
    }
}
