using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableSound : MonoBehaviour
{
    private Rigidbody rb;
    private AudioSource movableAudio;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movableAudio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6 ||
            collision.gameObject.CompareTag("MassDetector") ||
            collision.gameObject.CompareTag("Breakable")) { return; }
        // ne adjunk ki f�mes hangot ha a j�t�koshoz �t�dik,
        // vagy ha mass detectorhoz �r�nk, hogy tudjuk sz�pen hallani annak a kattan�s�t
        //vagy ha t�rhet� objektumhoz �r�nk hozz�, a t�r�st hangot haljuk

        if (rb.velocity.magnitude > 0.5f)
        {
            movableAudio.pitch = Random.Range(0.8f, 1.2f);
            print(rb.velocity.magnitude / 20);
            movableAudio.PlayOneShot(movableAudio.clip, 0.1f);
        }
    }
}
