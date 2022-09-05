using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldState : MonoBehaviour
{
    public int Size = 13;
    public int LinesCleared = 0;
    public int Score = 0;
    public int Level = 0;
    public float Speed;
    public float angle;

    [SerializeField]
    private ConstantListOfFloats _difficultyLevels;

    void Start()
    {
        SetSpeed(0);
        angle = 360 / GameState.PlayingFields;
    }

    void Update()
    {
        if (LinesCleared >= 10 * Level + 10)
        {
            SetSpeed(++Level);
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
