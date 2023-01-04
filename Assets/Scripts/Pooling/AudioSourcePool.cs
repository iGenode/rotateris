using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : PollingPool<AudioSource>
{
    public AudioSourcePool(AudioSource prefab, int sizeLimit) : base(prefab, sizeLimit)
    {
    }

    public void Play(AudioClip clip)
    {
        PlayAt(clip, new Vector3());
    }

    public void PlayAt(AudioClip clip, Vector3 position)
    {
        PlayAt(Get(), clip, position);
    }

    // TODO: probably just always use random pitch
    public void PlayAt(AudioClip clip, Vector3 position, bool isRandomPitch)
    {
        var source = Get();
        source.pitch = isRandomPitch ? Random.Range(0.8f, 1.2f) : 1.0f;
        PlayAt(source, clip, position);
    }

    private void PlayAt(AudioSource source, AudioClip clip, Vector3 position)
    {
        source.transform.position = position;
        source.clip = clip;
        source.Play();
    }

    protected override bool IsActive(AudioSource component)
    {
        return component.isPlaying;
    }
}

public static class AudioSourcePoolManager
{
    static AudioSourcePoolManager()
    {
        Pools = new();
    }

    public static Dictionary<string, AudioSourcePool> Pools;

    public static void Add(string tag, AudioSourcePool pool) => Pools.Add(tag, pool);

    public static AudioSourcePool GetPoolByTag(string tag) => Pools[tag];
}
