using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHandle : MonoBehaviour
{

    private Interactable interactable = null;

    [SerializeField] private GameObject handle;
    [SerializeField] private Transform notActivatedPos;
    [SerializeField] private Transform activatedPos;
    [Space]
    [SerializeField] private float lerpSpeed = 2f;

    private Coroutine lastCoroutine = null;

    private float progress_ = 0;

    private AudioSource panelAudio;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        panelAudio = GetComponent<AudioSource>();

        if(interactable == null) { return; }

        interactable.activated += PullDown;
        interactable.deactivated += PullBack;
    }

    private void OnDestroy()
    {
        if (interactable == null) { return; }

        interactable.activated -= PullDown;
        interactable.deactivated -= PullBack;
    }


    private void PullDown()
    {
        if(lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
            lastCoroutine = null;
        }
        lastCoroutine = StartCoroutine(LerpBetweenPositions( activatedPos.position));

        panelAudio.Play();
    }

    private void PullBack()
    {
        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
            lastCoroutine = null;
        }
        lastCoroutine = StartCoroutine(LerpBetweenPositions( notActivatedPos.position));

        panelAudio.Play();
    }


    IEnumerator LerpBetweenPositions( Vector3 _end)
    {
        progress_ = 0;
        while (handle.transform.position != _end)
        {
            handle.transform.position = Vector3.Lerp(handle.transform.position, _end, progress_ + lerpSpeed * Time.deltaTime);
            progress_ += lerpSpeed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

}
