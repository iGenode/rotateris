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

    void Update()
    {
        if (LinesCleared >= 10 * Level + 10)
        {
            SetSpeed(++Level);
        }
    }

    public void SetFieldFocus(bool isFocused)
    {
        if (isFocused)
        {
            IsFocused = true;
            SetSpeed(Level);
            OnFocusChangedEvent?.Invoke(true);
        } 
        else
        {
            IsFocused = isFocused;
            SetSpeed(GetSpeedForLevel(Level) * 3);
            OnFocusChangedEvent?.Invoke(false);
        }
    }

    public void IncreaseLinesCleared()
    {
        LinesCleared++;
    }

    private void SetSpeed(int level)
    {
        SetSpeed(GetSpeedForLevel(level));
    }

    private void SetSpeed(float speed)
    {
        Speed = speed;
    }

    private float GetSpeedForLevel(int level)
    {
        return _difficultyLevels.List[level <= 19 ? level : 19];
    }
}
