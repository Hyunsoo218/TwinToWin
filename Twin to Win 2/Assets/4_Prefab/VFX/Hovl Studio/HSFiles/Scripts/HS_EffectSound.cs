using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_EffectSound : MonoBehaviour
{
    private AudioSource soundComponent;
    private AudioClip clip;

	private void Awake()
    {
        soundComponent = GetComponent<AudioSource>();
        clip = soundComponent.clip;
    }
	private void OnEnable()
	{
        soundComponent.PlayOneShot(clip);
    }
}