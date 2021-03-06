using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassDetector : MonoBehaviour
{
    private Interactable interactable = null;

    private const int movableLayer = 7;


    [SerializeField] private Renderer topRenderer = null;
    [SerializeField] private Material nonActivatedMaterial = null;
    [SerializeField] private Material activatedMaterial = null;

    [Space]
    [SerializeField] private GameObject topPart = null;
    [SerializeField] private Transform notActivatedPos = null;
    [SerializeField] private Transform activatedPos = null;

    private List<GameObject> objectsInsideDetector = new List<GameObject>();

    private AudioSource massDetectorAudio;


    void Start()
    {
        interactable = GetComponent<Interactable>();
        massDetectorAudio = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != movableLayer) { return; }

        if(objectsInsideDetector.Count == 0)
        {
            interactable.Interacted(true);

            topPart.transform.position = activatedPos.position;
            topRenderer.material = activatedMaterial;

            massDetectorAudio.Play();
        }

        objectsInsideDetector.Add(other.gameObject);

        
    }

    private void OnTriggerExit(Collider other)
    {
        if(objectsInsideDetector.Contains(other.gameObject))
        {
            objectsInsideDetector.Remove(other.gameObject);
        }

        if (objectsInsideDetector.Count == 0)
        {
            interactable.Interacted(false);

            topPart.transform.position = notActivatedPos.position;
            topRenderer.material = nonActivatedMaterial;

            massDetectorAudio.Play();
        }
    }
}

