using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class DeathCollider : MonoBehaviour
{
    public static Action OnPlayerDeadlyCollision;

    private void OnCollisionEnter(Collision collision)
    {
        print("Col");
        if(collision.gameObject.CompareTag("Player"))
        {
            OnPlayerDeadlyCollision?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Col");
        if (other.gameObject.CompareTag("Player"))
        {
            OnPlayerDeadlyCollision?.Invoke();
        }
    }
}
