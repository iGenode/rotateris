using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _settleClips;
    [SerializeField]
    private AudioClip[] _clearRowClips;
    [SerializeField]
    private AudioClip _gameOverClip;

    private AudioSourcePool _audioPool;

    void Start()
    {
        _audioPool = AudioSourcePoolManager.GetPoolByTag("AudioSources");
    }

    public void PlayRandomSettleClipAt(Vector3 position)
    {
        _audioPool.PlayAt(_settleClips[Random.Range(0, _settleClips.Length)], position, true);
    }

    public void PlayRandomClearRowClipAt(Vector3 position)
    {
        _audioPool.PlayAt(_clearRowClips[Random.Range(0, _clearRowClips.Length)], position, true);
    }

    public void PlayGameOverClip()
    {
        _audioPool.Play(_gameOverClip);
    }
}
