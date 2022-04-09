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
        // ne adjunk ki fémes hangot ha a játékoshoz ütõdik,
        // vagy ha mass detectorhoz érünk, hogy tudjuk szépen hallani annak a kattanását
        //vagy ha törhetõ objektumhoz érünk hozzá, a törést hangot haljuk

        if (rb.velocity.magnitude > 0.5f)
        {
            movableAudio.pitch = Random.Range(0.8f, 1.2f);
            print(rb.velocity.magnitude / 20);
            movableAudio.PlayOneShot(movableAudio.clip, 0.1f);
        }
    }
}
