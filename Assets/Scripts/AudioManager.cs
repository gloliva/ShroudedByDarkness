using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("List of audio clips")]
    private Sound[] sounds;

    [SerializeField]
    private AudioMixerGroup musicGroup;

    [SerializeField]
    private AudioMixerGroup sfxGroup;

    public static AudioManager instance = null;

    private void Awake()
    {

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            if (sound.group == "Music")
            {
                sound.source.outputAudioMixerGroup = musicGroup;
                sound.source.loop = true;
            }
            else if (sound.group == "SFX")
            {
                sound.source.outputAudioMixerGroup = sfxGroup;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("No music");
            return;
        }
        s.source.volume = 1f;
        s.source.Play();
    }

    public void Play(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("No music");
            return;
        }
        s.source.volume = volume;
        s.source.Play();
    }

    public void SetVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("No music");
            return;
        }
        s.source.volume = volume;
    }

    public bool IsPlaying(string audio)
    {
        Sound s = Array.Find(sounds, sound => sound.name == audio);
        if (s != null && s.source.isPlaying)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Stop(string audioClip)
    {
        Sound s = Array.Find(sounds, sound => sound.name == audioClip);
        if (s != null && s.source.isPlaying)
        {
            s.source.Stop();
        }
    }
}
