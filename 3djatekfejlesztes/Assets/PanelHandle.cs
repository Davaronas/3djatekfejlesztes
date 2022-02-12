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

    private void Start()
    {
        interactable = GetComponent<Interactable>();

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
    }

    private void PullBack()
    {
        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
            lastCoroutine = null;
        }
        lastCoroutine = StartCoroutine(LerpBetweenPositions( notActivatedPos.position));
    }


    IEnumerator LerpBetweenPositions( Vector3 _end)
    {
        while(handle.transform.position != _end)
        {
            handle.transform.position = Vector3.Lerp(handle.transform.position, _end, lerpSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

}
