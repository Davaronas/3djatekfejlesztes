using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource walkAudioSource = null;
    [SerializeField] private AudioSource oneShotAudioSource = null;
    [Space]

    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landingSound;


    public void Running()
    {
        walkAudioSource.volume = 0.08f;
        walkAudioSource.pitch = 1.5f;
        if (!walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
    }

    public void WallRunning()
    {
        walkAudioSource.volume = 0.15f;
        walkAudioSource.pitch = 2.5f;
        if (!walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
    }

    public void StopRun()
    {
        walkAudioSource.Stop();
    }

    public void Jump()
    {
        oneShotAudioSource.pitch = 1.4f;
        oneShotAudioSource.PlayOneShot(jumpSound,.04f);
    }

    public void Land()
    {
        oneShotAudioSource.pitch = 1.8f;
        oneShotAudioSource.PlayOneShot(landingSound,0.03f);
    }

    
}
