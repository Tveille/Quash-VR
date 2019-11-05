using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public SoundClass[] sounds;

    private void Awake()
    {

        foreach (SoundClass s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            //s.source.outputAudioMixerGroup = s.output;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
            s.source.panStereo = s.panStereo;
        }
    }

    public void Play(string name)
    {
        SoundClass s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("SOUND NOT FOUND");
            return;
        }

        s.source.Play();
    }

    public void Stop(string name)
    {
        SoundClass s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("SOUND NOT FOUND");
            return;
        }

        s.source.Stop();
    }

    public bool isPlaying(string name)
    {
        SoundClass s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("SOUND NOT FOUND");
            return false;
        }

        return s.source.isPlaying;
    }

    // TO PLAY A SOUND IN ANOTHER SCRIPT :
    //
    // FindObjectOfType<AudioManager>().Play("name");
    // FindObjectOfType<AudioManager>().Stop("name");
    //
    //"name" is equal to the string of the sound you want to play
}