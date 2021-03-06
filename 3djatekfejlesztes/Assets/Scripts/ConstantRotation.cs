using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    void Update()
    {
        transform.Rotate(transform.up * Time.deltaTime * speed);
    }
}
