using UnityEngine;

public class GameState : MonoBehaviour
{
    public const float HorizontalMoveDelay = .2f;
    public const int MoveUnit = 1;

    public static int LinesCleared = 0;

    public int Score = 0;
    public int Level = 0;
    public float Speed;
    // fixme: set this at the start/pass from menu scene
    public int PlayingFields = 1;
    public float angle;
    public bool IsGameOver = false;

    [SerializeField]
    private ConstantListOfFloats _difficultyLevels;

    void Start()
    {
        SetSpeed(0);
        angle = 360 / PlayingFields;
    }

    void Update()
    {
        if (LinesCleared >= 10 * Level + 10)
        {
            SetSpeed(++Level);
        }
    }

    public static void IncreaseLinesCleared()
    {
        LinesCleared++;
    }

    private void SetSpeed(int level)
    {
        Speed = _difficultyLevels.List[level <= 19 ? level : 19];
    }
}
