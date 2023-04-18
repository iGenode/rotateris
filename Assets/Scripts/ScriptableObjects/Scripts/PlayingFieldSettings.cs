using UnityEngine;

[CreateAssetMenu(fileName = "New Playing Field Settings Object", menuName = "ScriptableObjects/PlayingFieldSettings", order = 1)]
public class PlayingFieldSettings : ScriptableObject
{
    [SerializeField]
    private int _playingFieldCount;
    [SerializeField]
    private int _difficultyLevel;
    [SerializeField]
    private string _lastControlScheme;

    public int PlayingFieldCount
    {
        get { return _playingFieldCount; }
        set { _playingFieldCount = value; }
    }

    public int DifficultyLevel
    {
        get { return _difficultyLevel; }
        set { _difficultyLevel = value; }
    }

    public string LastControlScheme
    {
        get { return _lastControlScheme; }
        set { _lastControlScheme = value; }
    }
}
