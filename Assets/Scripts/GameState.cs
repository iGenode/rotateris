using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public const float HorizontalMoveDelay = .2f;
    public const int MoveUnit = 1;
    private const float _fieldOffset = 20.0f;

    // fixme: set this at the start/pass from menu scene
    public int PlayingFieldCount = 1;
    public bool IsGameOver = false;

    [SerializeField]
    private GameObject _playingFieldPrefab;
    [SerializeField]
    private Camera _mainCamera;
    private List<PlayingFieldState> _playingFieldStates = new();
    private int _focusedFieldIndex = 0;
    private int _angleStep;

    //temp
    private Vector3 _defaultSpawnPos;

    private void Start()
    {
        _defaultSpawnPos = new(0, 0, -_fieldOffset);
        _angleStep = 360 / PlayingFieldCount;

        for (int i = 0; i < PlayingFieldCount; i++)
        {
            var angles = new Vector3(0, _angleStep * i, 0);
            var spawnPoint = Utils.RotatePointAroundPivot(_defaultSpawnPos, Vector3.zero, angles);
            var playingField = Instantiate(_playingFieldPrefab, spawnPoint, _playingFieldPrefab.transform.rotation);
            playingField.name = $"Playing field {i}";
            playingField.transform.Rotate(angles);
            var state = playingField.GetComponent<PlayingFieldState>();
            state.SetFieldFocus(i == _focusedFieldIndex);
            state.SetRotationAngles(angles);
            _playingFieldStates.Add(state);
        }
    }

    private void OnRotateField(float direction)
    {
        // Rotating the camera and changing current focused playing field
        if (direction != 0)
        {
            _mainCamera.transform.RotateAround(Vector3.zero, Vector3.up, _angleStep * -direction);
            _playingFieldStates[_focusedFieldIndex].SetFieldFocus(false);
            ChangeIndex((int)-direction);
            _playingFieldStates[_focusedFieldIndex].SetFieldFocus(true);
        }
    }

    private void ChangeIndex(int direction)
    {
        if (_focusedFieldIndex == 0 && direction < 0)
        {
            _focusedFieldIndex = PlayingFieldCount - 1;
            return;
        }
        if (_focusedFieldIndex == PlayingFieldCount - 1 && direction > 0)
        {
            _focusedFieldIndex = 0;
            return;
        }
        _focusedFieldIndex += direction;
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
