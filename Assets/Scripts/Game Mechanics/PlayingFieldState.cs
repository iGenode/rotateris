using TMPro;
using UnityEngine;

public class PlayingFieldState : MonoBehaviour
{
    public delegate void OnFocusChanged(bool isFocused);
    public event OnFocusChanged OnFocusChangedEvent;

    public delegate void OnScoreChanged(int amount);
    public event OnScoreChanged OnScoreChangedEvent;

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
    public int Level = 1;
    public float Speed;
    public bool IsFocused;
    public Transform CameraPosition;

    [SerializeField]
    private ConstantListOfFloats _difficultyLevels;
    private TextMeshProUGUI _localScoreText;
    private TextMeshProUGUI _difficultyLevelText;

    public void SetFieldFocus(bool isFocused)
    {
        if (isFocused)
        {
            IsFocused = true;
            SetDifficulty(Level);
            OnFocusChangedEvent?.Invoke(true);
            UpdateScoreText();
        }
        else
        {
            IsFocused = isFocused;
            SetSpeed(GetSpeedForLevel(Level) * 3);
            OnFocusChangedEvent?.Invoke(false);
        }
    }

    // TODO: more scoring
    public void IncreaseScore(int amount)
    {
        Score += amount * Level;
        OnScoreChangedEvent?.Invoke(amount * Level);
        if (IsFocused)
        {
            UpdateScoreText();
        }
    }

    public void UpdateScoreText() => _localScoreText.text = $"Score: {Score}";

    public void UpdateDifficultyText() => _difficultyLevelText.text = $"Difficulty Level: {Level}";

    public void IncreaseLinesCleared(int count)
    {
        LinesCleared += count;
        switch (count)
        {
            case 1:
                //Debug.Log("Increasing score by 100");
                IncreaseScore(100);
                break;
            case 2:
                //Debug.Log("Increasing score by 300");
                IncreaseScore(300);
                break;
            case 3:
                //Debug.Log("Increasing score by 500");
                IncreaseScore(500);
                break;
            case 4:
                //Debug.Log("Increasing score by 800");
                IncreaseScore(800);
                break;
        }

        if (LinesCleared >= 10 * Level + 10)
        {
            SetDifficulty(++Level);
        }
    }

    public void SetLocalScoreText(TextMeshProUGUI tmp) => _localScoreText = tmp;

    public void SetDifficultyLevelText(TextMeshProUGUI tmp) => _difficultyLevelText = tmp;

    public void SetDifficulty(int level)
    {
        Level = level;
        SetSpeed(GetSpeedForLevel(level));
        if (IsFocused)
        {
            UpdateDifficultyText();
        }
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
