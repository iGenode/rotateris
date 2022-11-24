using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSettings : MonoBehaviour
{
    [SerializeField]
    private PlayingFieldSettings _settings;

    public void UpdateFieldCount(int count)
    {
        _settings.PlayingFieldCount = count;
    }

    public void UpdateFieldCount(string count)
    {
        if (count.Length != 0)
        {
            _settings.PlayingFieldCount = int.Parse(count);
        }
    }

    public void UpdateFieldCount(float count)
    {
        _settings.PlayingFieldCount = (int)count;
    }

    public void UpdateDifficultyLevel(int level)
    {
        _settings.DifficultyLevel = level;
    }

    public void UpdateDifficultyLevel(string level)
    {
        if (level.Length != 0)
        {
            _settings.DifficultyLevel = int.Parse(level);
        }
    }

    public void UpdateDifficultyLevel(float level)
    {
        _settings.DifficultyLevel = (int)level;
    }
}
