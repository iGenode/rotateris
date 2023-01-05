using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateSettings : MonoBehaviour
{
    [SerializeField]
    private PlayingFieldSettings _settings;
    [SerializeField]
    private TMP_InputField _customDifficultyField;

    public void UpdateFieldCount(int count)
    {
        _settings.PlayingFieldCount = count;
    }

    public void UpdateFieldCount(string count)
    {
        Debug.Log($"Updating field count to {count}");
        if (count.Length != 0)
        {
            _settings.PlayingFieldCount = int.Parse(count);
        }
    }

    public void UpdateFieldCount(float count)
    {
        _settings.PlayingFieldCount = (int)count;
    }

    public void UpdateFieldCountOnToggle(bool isToggled)
    {
        Debug.Log("Updating difficulty on toggle");
        if (isToggled)
        {
            UpdateFieldCount(_customDifficultyField.text);
        }
        else
        {
            UpdateFieldCount(3);
        }
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
