using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    [Space]
    [SerializeField] private Vector3 scaleTo;
    [SerializeField] private float lerpSpeed = 2f;


    Vector3 baseScale = Vector3.zero;


    private Coroutine lastCoroutine = null;

    private float progress_ = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(interactable == null) { return; }

        baseScale = transform.localScale;

        interactable.activated += ScaleToExtended;
        interactable.deactivated += ScaleToBase;
    }

    private void OnDestroy()
    {
        if (interactable == null)
        {
            return;
        }

        interactable.activated -= ScaleToExtended;
        interactable.deactivated -= ScaleToBase;
    }


    private void ScaleToExtended()
    {
        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
            lastCoroutine = null;
        }
        lastCoroutine = StartCoroutine(LerpBetweenScales(scaleTo));
    }

    private void ScaleToBase()
    {
        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
            lastCoroutine = null;
        }
        lastCoroutine = StartCoroutine(LerpBetweenScales(baseScale));
    }


    IEnumerator LerpBetweenScales(Vector3 _endScale)
    {
        progress_ = 0;
        while (transform.localScale != _endScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _endScale,progress_ + lerpSpeed * Time.deltaTime);
            progress_ += lerpSpeed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

   
}
