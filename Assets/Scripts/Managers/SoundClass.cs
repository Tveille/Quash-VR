using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName = "Sound List.asset", menuName = "Custom/Sound List", order = 100)]
public class SoundClass : ScriptableObject
{
    public SoundSettings[] sounds;
}


[System.Serializable]
public struct SoundSettings
{
    public enum Tags { Brick, Impact, Wall, Floor, Racket, FrontWall }
    public Tags tag;

    public AudioClip clip;
    public float cooldown;
    //public AudioMixerGroup output;            // ça aussi?

    [Range(0.0f, 1.0f)]
    public float volume;
    [Range(0.1f, 3.0f)]
    public float pitch;

    public bool loop;

    [Range(0.0f, 1.0f)]
    public float spatialBlend;

    [Range(-1.0f, 1.0f)]
    public float panStereo;

    public float maxHitMagnitude;

    [Range(0.0f, 1.0f)]
    public float minVolume;

    [Range(0.0f, 1.0f)]
    public float maxVolume;

    [HideInInspector]
    public float lastPlayTime;
}
