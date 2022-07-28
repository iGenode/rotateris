using UnityEngine;

public class GameState : MonoBehaviour
{
    public float Speed = 1;
    // fixme: set this at the start/pass from menu scene
    public int PlayingFields = 1;
    public const int MoveUnit = 1;
    public int Score = 0;
    public float angle;
    public bool IsGameOver = false;

    void Start()
    {
        angle = 360 / PlayingFields;
    }

    void Update()
    {

    }
}
