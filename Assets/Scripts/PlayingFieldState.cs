using UnityEngine;

public class PlayingFieldState : MonoBehaviour
{
    public delegate void OnFocusChanged(bool isFocused);
    public event OnFocusChanged OnFocusChangedEvent;

    private Vector3 _rotationAngles;

    public Vector3 GetRotationAngles()
    {
        return _rotationAngles;
    }

    public void SetRotationAngles(Vector3 value)
    {
        _rotationAngles = value;
    }

    public int Size = 13;
    public int LinesCleared = 0;
    public int Score = 0;
    public int Level = 0;
    public float Speed;
    public bool IsFocused;

    [SerializeField]
    private ConstantListOfFloats _difficultyLevels;

    void Start()
    {
        SetSpeed(0);
    }

    void Update()
    {
        if (LinesCleared >= 10 * Level + 10)
        {
            SetSpeed(++Level);
        }
    }

    public void SetFieldFocus(bool isFocused)
    {
        // TODO: Speed can only go so low, when actually should always be currentSpeed / 2;
        if (isFocused)
        {
            IsFocused = true;
            SetSpeed(Level);
            OnFocusChangedEvent?.Invoke(true);
        } 
        else
        {
            IsFocused = isFocused;
            SetSpeed(Level / 2);
            OnFocusChangedEvent?.Invoke(false);
        }
    }

    public void IncreaseLinesCleared()
    {
        LinesCleared++;
    }

    private void SetSpeed(int level)
    {
        Speed = _difficultyLevels.List[level <= 19 ? level : 19];
    }
}
