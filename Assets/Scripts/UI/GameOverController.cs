using TMPro;
using UnityEngine;

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
