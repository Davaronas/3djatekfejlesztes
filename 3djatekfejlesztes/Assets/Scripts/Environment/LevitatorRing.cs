using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitatorRing : MonoBehaviour
{
   [SerializeField] private float upSpeed = 5f;

    private Levitator levitator = null;
    void Start()
    {
        levitator = transform.parent.GetComponent<Levitator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * upSpeed * Time.deltaTime);
        if (transform.localPosition.y > levitator.y_size)
        {
            transform.position = levitator.ringStartPos.position;
        }
    }
}
