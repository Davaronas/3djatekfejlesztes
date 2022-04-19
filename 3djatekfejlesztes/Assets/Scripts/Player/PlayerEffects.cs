using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private AudioSource walkAudioSource = null;
    [SerializeField] private AudioSource oneShotAudioSource = null;
    [Space]

    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioClip lavaSound;
    [Space]
    [SerializeField] private Color lavaScreenEffectBaseColor;
    [SerializeField] private Color lavaScreenEffectTurnedColor;
    [SerializeField] private float lavaScreenEffectLerpSpeed = 5f;
     private Image lavaScreenEffect;


    private void Start()
    {
        lavaScreenEffect = FindObjectOfType<Canvas>().transform.Find("LavaEffect").GetComponent<Image>();
        lavaScreenEffect.color = lavaScreenEffectBaseColor;
    }


    public void Audio_Running()
    {
        walkAudioSource.volume = 0.08f;
        walkAudioSource.pitch = 1.5f;
        if (!walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
    }

    public void Audio_WallRunning()
    {
        walkAudioSource.volume = 0.15f;
        walkAudioSource.pitch = 2.5f;
        if (!walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
    }

    public void Audio_StopRun()
    {
        walkAudioSource.Stop();
    }

    public void Audio_Jump()
    {
        oneShotAudioSource.pitch = 1.4f;
        oneShotAudioSource.PlayOneShot(jumpSound,.04f);
    }

    public void Audio_Land()
    {
        oneShotAudioSource.pitch = 1.8f;
        oneShotAudioSource.PlayOneShot(landingSound,0.03f);
    }

    public void Audio_Lava()
    {
        oneShotAudioSource.pitch = 1;
        oneShotAudioSource.PlayOneShot(lavaSound, 0.15f);
    }

    public void ScreenEffect_Lava()
    {
        StartCoroutine(LavaScreenLerpColor());
    }

    IEnumerator LavaScreenLerpColor()
    {
        
        float _progress = 0;
        while (_progress <= 1)
        {
           lavaScreenEffect.color = Color.Lerp(lavaScreenEffectBaseColor, lavaScreenEffectTurnedColor, _progress + (lavaScreenEffectLerpSpeed * Time.deltaTime));
            _progress += lavaScreenEffectLerpSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
