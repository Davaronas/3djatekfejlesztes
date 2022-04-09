using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    [SerializeField] private AudioSource buttonAudio;
    [Space]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    public void HoverSound()
    {
        buttonAudio.PlayOneShot(hoverSound,0.02f);
    }

    public void ClickSound()
    {
        buttonAudio.PlayOneShot(clickSound,2f);
    }


}
