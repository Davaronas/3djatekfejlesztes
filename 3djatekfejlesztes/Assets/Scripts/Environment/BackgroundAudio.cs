using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudio : MonoBehaviour
{
    private AudioSource bgAudio;

    [SerializeField] private AudioClip[] ambients;
    [SerializeField] private float[] ambientVolumes;
   

    int random_ = 0;
    int lastRandom_ = 0;
    float time_ = 0;

    void Start()
    {
        bgAudio = GetComponent<AudioSource>();

        random_ = Random.Range(0, ambients.Length);
        lastRandom_ = random_;

         bgAudio.PlayOneShot(ambients[random_],ambientVolumes[random_]);
       // bgAudio.PlayOneShot(ambients[1], ambientVolumes[1]);
        time_ = ambients[random_].length;
        StartCoroutine(PlayAudio(time_));
    }

    IEnumerator PlayAudio(float _time)
    {
        yield return new WaitForSeconds(_time + 5f);

        rollRandom:
        random_ = Random.Range(0, ambients.Length);
        if(random_ == lastRandom_)
        {
            goto rollRandom;
        }

        lastRandom_ = random_;

        bgAudio.PlayOneShot(ambients[random_]);
        time_ = ambients[random_].length;
        StartCoroutine(PlayAudio(time_));
    }
   
}
