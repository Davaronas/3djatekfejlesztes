using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableSound : MonoBehaviour
{
     private AudioSource breakableAudio;
    
    [SerializeField] private AudioClip breakSound;

    private int breaks;
    [SerializeField] private int maxBreaksAtOneTime = 4;


    private void Start()
    {
        breakableAudio = GetComponent<AudioSource>();
    }

    public void Break()
    {
        StartCoroutine(DelayBreak());
    }

    IEnumerator DelayBreak()
    {
        breaks++;

        if (breaks < maxBreaksAtOneTime)
        {
            yield return new WaitForSeconds(Random.Range(0, 0.4f));
            breakableAudio.pitch = Random.Range(0.65f, 1.35f);
            breakableAudio.PlayOneShot(breakSound);
        }

            yield return new WaitForSeconds(1f);
            breaks = Mathf.Clamp(breaks - 1, 0, 99);
        
    }
}
