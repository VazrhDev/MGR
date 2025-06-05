using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class MenuSoundManager : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] AudioMixerGroup AudioMixerSFX;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        if(audioSource.outputAudioMixerGroup == null)
        {
            audioSource.outputAudioMixerGroup = AudioMixerSFX;
        }
    }

    void Update()
    {
        
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.clip = buttonHoverSound;
        audioSource.Play();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        audioSource.clip = buttonClickSound;
        audioSource.Play();
    }
}
