using UnityEngine;
using UnityEngine.InputSystem;

public class GameState : MonoBehaviour
{
    public const float HorizontalMoveDelay = .2f;
    public const int MoveUnit = 1;

    // fixme: set this at the start/pass from menu scene
    public static int PlayingFields = 1;
    public bool IsGameOver = false;

    [SerializeField]
    private Camera _mainCamera;
    //private Vector2 _cameraMove;

    private void OnRotateField(float direction)
    {
        if (direction != 0)
        {
            _mainCamera.transform.RotateAround(Vector3.zero, Vector3.up, 45.0f * direction);
        }
    }

    private void OnEnable()
    {
        InputController.OnRotateFieldEvent += OnRotateField;
    }

    private void OnDisable()
    {
        InputController.OnRotateFieldEvent -= OnRotateField;
    }
}
