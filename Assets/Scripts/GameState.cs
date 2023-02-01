using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class GameState : MonoBehaviour
{
    public delegate void OnGameOver();
    public static event OnGameOver OnGameOverAction;

    public const float HorizontalMoveDelay = .2f;
    public const int MoveUnit = 1;
    //private const float _offsetFromWorldCenter = 20.0f;
    private float _offsetFromWorldCenter = 15.0f;
    private Vector3 _cameraOffset = new(0, 14, -20);

    public static bool IsGamePaused = false;
    public static bool IsGameOver = false;
    public static int FocusedFieldIndex = 0;
    public int TotalScore = 0;
    public int PlayingFieldCount = 1;

    [SerializeField]
    private GameObject _playingFieldPrefab;
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private InputAction _action;
    [SerializeField]
    private TextMeshProUGUI _totalScoreText;
    [SerializeField]
    private TextMeshProUGUI _localScoreText;
    [SerializeField]
    private TextMeshProUGUI _difficultyLevelText;
    [SerializeField]
    private PlayingFieldSettings _settings;
    [SerializeField]
    private SoundManager _soundManager;

    private static readonly List<SpawnManager> _spawnManagers = new();

    private readonly List<PlayingFieldState> _playingFieldStates = new();
    private float _angleStep;
    private Coroutine _rotationCoroutine;
    private bool _isRotating = false;
    //private HolderController _holderController;

    // Spline variables
    private SplineContainer _cameraSplineContainer;
    private float _currentOffset = 0f;
    private float _splineLength;

    // TODO: what's this?
    //temp
    private Vector3 _defaultSpawnPos;

    public int TotalLinesCleared
    {
        get
        {
            var count = 0;
            foreach (var playingFieldState in _playingFieldStates)
            {
                count += playingFieldState.LinesCleared;
            }
            return count;
        }
    }

    public int HighestDifficulty
    {
        get
        {
            var difficulty = 0;
            foreach (var playingFieldState in _playingFieldStates)
            {
                if (playingFieldState.Level > difficulty)
                {
                    difficulty = playingFieldState.Level;
                }
            }
            return difficulty;
        }
    }

    private void Start()
    {
        FocusedFieldIndex = 0;
        IsGameOver = false;
        IsGamePaused = false;

        // Setting playing field count as chosen by the player
        PlayingFieldCount = _settings.PlayingFieldCount;

        _offsetFromWorldCenter += 13 * (PlayingFieldCount / 5);
        _cameraOffset.z -= _offsetFromWorldCenter;
        _mainCamera.transform.position = _cameraOffset;

        _defaultSpawnPos = new(0, 0, -_offsetFromWorldCenter);
        _angleStep = 360f / PlayingFieldCount;

        var audioSource = new GameObject().AddComponent<AudioSource>();
        audioSource.maxDistance = -_cameraOffset.z * 2;
        AudioSourcePoolManager.Pools.Add("AudioSources", new AudioSourcePool(audioSource, 8));

        //_holderController = GetComponent<HolderController>();

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
            // Setting difficulty level as chosen by the player
            state.SetDifficulty(_settings.DifficultyLevel);
            state.SetLocalScoreText(_localScoreText);
            state.SetDifficultyLevelText(_difficultyLevelText);
            state.OnScoreChangedEvent += IncreaseTotalScore;
            state.SetFieldFocus(i == FocusedFieldIndex);
            state.SetRotationAngles(angles);
            _playingFieldStates.Add(state);

            // Subscribing to row filled events
            var rowFilledTrigger = playingField.GetComponentInChildren<RowFilledTrigger>();
            rowFilledTrigger.OnManyRowsDestroyed += state.IncreaseLinesCleared;
            rowFilledTrigger.OnDestroyedAtPosition += _soundManager.PlayRandomClearRowClipAt;

            points.Add(
                new float3(
                    state.CameraPosition.position.x,
                    state.CameraPosition.position.y,
                    state.CameraPosition.position.z
                )
            );

            // Subscribing to player settled event
            var spawnManager = playingField.GetComponentInChildren<SpawnManager>();
            spawnManager.OnSettledWithPosition += _soundManager.PlayRandomSettleClipAt;
            _spawnManagers.Add(spawnManager);
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
        spline.SetTangentMode(TangentMode.Mirrored);
        //spline.EditType = SplineType.CatmullRom;
        _splineLength = spline.GetLength();

        GameObject anchor = new()
        {
            name = "Look Anchor",
        };
        anchor.transform.position = new Vector3(0, PlayingFieldCount >= 5 ? 9 - (PlayingFieldCount / 3f) : 9, 0);
        var lookAt = _mainCamera.AddComponent<CameraLookAt>();
        lookAt.Anchor = anchor.transform;

        _totalScoreText.text = "Total Score: 0";
    }

    public static void PauseOrResume(bool isPaused)
    {
        IsGamePaused = isPaused;
        Time.timeScale = IsGamePaused ? 0 : 1;
    }

    public static void GameOver()
    {
        IsGameOver = true;
        OnGameOverAction?.Invoke();
        Time.timeScale = 0;
    }

    public static SpawnManager GetFocusedSpawnManager() => _spawnManagers[FocusedFieldIndex];

    private void OnRotateField(InputAction.CallbackContext context)
    {
        if (!IsGamePaused && !IsGameOver)
        {
            var direction = context.ReadValue<float>();
            _playingFieldStates[FocusedFieldIndex].SetFieldFocus(false);

            ChangeIndex((int)-direction);
            // Rotating the camera and changing current focused playing field
            // If already rotating - stop current rotation before starting a new one
            if (_isRotating)
            {
                //Debug.Log("Stopped previous rotation");
                StopCoroutine(_rotationCoroutine);
            }

            var endPoint = _cameraSplineContainer.Spline.GetCurve(mod(FocusedFieldIndex - 1, PlayingFieldCount)).P3;
            _rotationCoroutine = StartCoroutine(
                RotateOnSpline(
                    new Vector3(endPoint.x, endPoint.y, endPoint.z),
                    (int)-direction,
                    _isRotating
                )
            );

            _playingFieldStates[FocusedFieldIndex].SetFieldFocus(true);
        }
    }

    private void IncreaseTotalScore(int amount)
    {
        TotalScore += amount;
        _totalScoreText.text = $"Total Score: {TotalScore}";
    }

    private void ChangeIndex(int direction)
    {
        if (FocusedFieldIndex == 0 && direction < 0)
        {
            FocusedFieldIndex = PlayingFieldCount - 1;
            return;
        }
        if (FocusedFieldIndex == PlayingFieldCount - 1 && direction > 0)
        {
            FocusedFieldIndex = 0;
            return;
        }
        FocusedFieldIndex += direction;
    }

    IEnumerator RotateOnSpline(Vector3 destination, int direction, bool shouldHurry)
    {
        _isRotating = true;
        var multiplier = shouldHurry ? 6 : 4 - (PlayingFieldCount * 0.07f) < 1 ? 1 : 4 - (PlayingFieldCount * 0.07f);
        while (Vector3.Distance(_mainCamera.transform.position, destination) >= 1f)
        {
            //Debug.Log($"Current offset: {(_currentOffset + 5f * direction * Time.deltaTime / (_splineLength / (PlayingFieldCount * multiplier))) % 1f}");
            _currentOffset = (_currentOffset + 5f * direction * Time.deltaTime / (_splineLength / (PlayingFieldCount * multiplier))) % 1f;

            var posOnSplineLocal = SplineUtility.EvaluatePosition(
                _cameraSplineContainer.Spline,
                _currentOffset > 0 ? _currentOffset : 1 + _currentOffset);
            _mainCamera.transform.position = _cameraSplineContainer.transform.TransformPoint(posOnSplineLocal);

            yield return new WaitForEndOfFrame();
        }

        _mainCamera.transform.position = destination;
        _isRotating = false;
    }

    private void OnEnable()
    {
        _action.Enable();
        _action.performed += OnRotateField;
        OnGameOverAction += _soundManager.PlayGameOverClip;
        foreach (var state in _playingFieldStates)
        {
            state.OnScoreChangedEvent += IncreaseTotalScore;
        }
    }

    private void OnDisable()
    {
        _action.Disable();
        _action.performed -= OnRotateField;
        OnGameOverAction -= _soundManager.PlayGameOverClip;
        foreach (var state in _playingFieldStates)
        {
            state.OnScoreChangedEvent -= IncreaseTotalScore;
        }
    }

    private void OnDestroy()
    {
        AudioSourcePoolManager.Clear();
        _spawnManagers.Clear();
        // If the game is over or paused restore time scale to prevent freezing the next game
        if (IsGamePaused || IsGameOver)
        {
            Time.timeScale = 1;
            IsGameOver = false;
            IsGamePaused = false;
        }
    }

    // Actual mod instead of % remainder
    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}
