using UnityEngine;

public class GameState : MonoBehaviour
{
    public const float HorizontalMoveDelay = .2f;
    public const int MoveUnit = 1;

    // fixme: set this at the start/pass from menu scene
    public static int PlayingFields = 1;
    public bool IsGameOver = false;
}
