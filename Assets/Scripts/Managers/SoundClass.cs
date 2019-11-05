using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundClass
{
    public string name;

    public AudioClip clip;
    //public AudioMixerGroup output;

    [Range(0.0f, 1.0f)]
    public float volume = 1;
    [Range(0.1f, 3.0f)]
    public float pitch = 1.0f;

    public bool loop;

    [Range(0.0f, 1.0f)]
    public float spatialBlend = 0.0f;

    [Range(-1.0f, 1.0f)]
    public float panStereo = 0.0f;

    [HideInInspector]
    public AudioSource source;

}
