using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class GameState : MonoBehaviour
{
    public const float HorizontalMoveDelay = .2f;
    public const int MoveUnit = 1;
    private const float _offsetFromWorldCenter = 20.0f;

    public int TotalScore = 0;
    public int PlayingFieldCount = 1;
    public bool IsGameOver = false;

    [SerializeField]
    private GameObject _playingFieldPrefab;
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private InputAction _action;
    private List<PlayingFieldState> _playingFieldStates = new();
    private int _focusedFieldIndex = 0;
    private int _angleStep;
    private Coroutine _rotationCoroutine;
    private bool _isRotating;

    // Spline variables
    private SplineContainer _cameraSplineContainer;
    private float _currentOffset = 0f;
    private float _splineLength;

    // TODO: what's this?
    //temp
    private Vector3 _defaultSpawnPos;

    private void Start()
    {
        _defaultSpawnPos = new(0, 0, -_offsetFromWorldCenter);
        _angleStep = 360 / PlayingFieldCount;

        GameObject splineObject = new()
        {
            name = "Camera Spline"
        };
        _cameraSplineContainer = splineObject.AddComponent<SplineContainer>();
        Spline spline = _cameraSplineContainer.Spline;

        var points = new List<float3>();

        for (int i = 0; i < PlayingFieldCount; i++)
        {
            var angles = new Vector3(0, _angleStep * i, 0);
            var spawnPoint = Utils.RotatePointAroundPivot(_defaultSpawnPos, Vector3.zero, angles);
            var playingField = Instantiate(_playingFieldPrefab, spawnPoint, _playingFieldPrefab.transform.rotation);
            playingField.name = $"Playing field {i}";
            playingField.transform.Rotate(angles);

            var state = playingField.GetComponent<PlayingFieldState>();
            state.OnScoreChangedEvent += IncreaseTotalScore;
            state.SetFieldFocus(i == _focusedFieldIndex);
            state.SetRotationAngles(angles);
            _playingFieldStates.Add(state);

            points.Add(
                new float3(
                    state.CameraPosition.position.x,
                    state.CameraPosition.position.y,
                    state.CameraPosition.position.z
                )
            );
        }

        // Calculating tangents for each BezierKnot to make a circle for camera to rotate on
        // https://forum.unity.com/threads/interpolating-cubic-splines.1315860/
        // https://www.cubic.org/docs/hermite.htm
        for (int i = 0; i < points.Count; i++)
        {
            var iPrev = mod(i - 1, points.Count);
            var iNext = mod(i + 1, points.Count);

            var c = 0.3f;
            var tangent = (1 - c) * (points[iNext] - points[iPrev]) / 3;

            spline.Add(new BezierKnot(points[i], -tangent, tangent, quaternion.identity));
        }

        spline.Closed = true;
        spline.EditType = SplineType.CatmullRom;
        _splineLength = spline.GetLength();

        GameObject anchor = new()
        {
            name = "Look Anchor",
        };
        anchor.transform.position = new Vector3(0, 7, 0);
        var lookAt = _mainCamera.AddComponent<CameraLookAt>();
        lookAt.Anchor = anchor.transform;
    }

    private void OnRotateField(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<float>();
        _playingFieldStates[_focusedFieldIndex].SetFieldFocus(false);
        ChangeIndex((int)-direction);
        // Rotating the camera and changing current focused playing field
        // If already rotating - stop current rotation before starting a new one
        if (_isRotating)
        {
            //Debug.Log("Stopped previous rotation");
            StopCoroutine(_rotationCoroutine);
        }

        _rotationCoroutine = StartCoroutine(
            RotateOnSpline(
                _cameraSplineContainer.Spline.GetCurve(mod(_focusedFieldIndex - 1, PlayingFieldCount)),
                (int)-direction
            )
        );

        _playingFieldStates[_focusedFieldIndex].SetFieldFocus(true);
    }

    private void IncreaseTotalScore(int amount)
    {
        TotalScore += amount;
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

    IEnumerator RotateOnSpline(BezierCurve curve, int direction)
    {
        _isRotating = true;
        for (var t = 0f; t <= 1; t += Time.deltaTime / 15f)
        {
            _currentOffset = (_currentOffset + 5f * direction * Time.deltaTime / (_splineLength / (PlayingFieldCount * 2))) % 1f;

            var posOnSplineLocal = SplineUtility.EvaluatePosition(
                _cameraSplineContainer.Spline,
                _currentOffset > 0 ? _currentOffset : 1 + _currentOffset);
            _mainCamera.transform.position = _cameraSplineContainer.transform.TransformPoint(posOnSplineLocal);

            // Not fail safe, can skip the time when camera arives and will keep going
            if (Vector3.Distance(_mainCamera.transform.position, curve.P3) <= 0.1f)
            {
                //Debug.Log($"Arrived in {t * 15f} seconds");
                _isRotating = false;
                _mainCamera.transform.position = curve.P3;
                yield break;
            }

            yield return null;
        }

        //Debug.Log("Time passed");
        _mainCamera.transform.position = curve.P3;
        _isRotating = false;
    }

    private void OnEnable()
    {
        _action.Enable();
        _action.performed += OnRotateField;
        foreach (var state in _playingFieldStates)
        {
            state.OnScoreChangedEvent += IncreaseTotalScore;
        }
    }

    private void OnDisable()
    {
        _action.Disable();
        _action.performed -= OnRotateField;
        foreach (var state in _playingFieldStates)
        {
            state.OnScoreChangedEvent -= IncreaseTotalScore;
        }
    }

    // Actual mod instead of % remainder
    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}
