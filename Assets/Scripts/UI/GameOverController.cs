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
    // TODO: high score
    //[SerializeField]
    //private TextMeshProUGUI _highScoreText;

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

        foreach (GameObject obj in _objectsToHide)
        {
            obj.SetActive(false);
        }
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
