using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioMixer masterAudioMixer;
    public AudioMixer MasterAudioMixer { get { return masterAudioMixer; } }

    public const string Mixer_Master = "MasterVolume";
    public const string Mixer_Music = "MusicVolume";
    public const string Mixer_SFX = "SFXVolume";

    public const string MASTER_KEY = "masterVolume";
    public const string MUSIC_KEY = "musicVolume";
    public const string SFX_KEY = "sfxVolume";

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        LoadVolume();
    }

    void LoadVolume()
    {
        float MasterVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float MusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float SFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        masterAudioMixer.SetFloat(Mixer_Master, Mathf.Log10(MasterVolume) * 20);
        masterAudioMixer.SetFloat(Mixer_Music, Mathf.Log10(MusicVolume) * 20);
        masterAudioMixer.SetFloat(Mixer_SFX, Mathf.Log10(SFXVolume) * 20);

    }
}
