using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpReset : MonoBehaviour
{
    private PlayerMovement player = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerMovement>();
            player.GiveDoubleJump();
            print("WTF");
        }
    }

   
}
