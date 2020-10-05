using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip clip;
    [Range(0,1)]
    public float volume = 1;
    public float pitch { get { return GetPitch(); } set { Pitch = value; } }
    [Range(0,2)]
    [SerializeField]
    private float Pitch = 1;
    public string id;
    [Range(0,2)]
    public float minPitch = 1;
    [Range(0,2)]
    public float maxPitch = 1;
    public bool randomizePitch = false;

    public Sound(AudioClip clip, float volume)
    {
        this.clip = clip;
        this.volume = volume;
    }

    public float GetPitch()
    {
        if (!randomizePitch) return Pitch;
        return UnityEngine.Random.Range(minPitch, maxPitch);
    }
}