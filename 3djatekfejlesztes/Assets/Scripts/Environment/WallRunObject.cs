using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunObject : MonoBehaviour
{
    public Transform runDirection;
    public float breakDistance = 20f;

   

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("Col");

            if (playerMovement == null)
                playerMovement = other.gameObject.GetComponent<PlayerMovement>();

            playerMovement.StartWallRun(this, runDirection.forward);
        }
    }
    */

    /*
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
           

            
            if(playerMovement == null)
            playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

            isPlayerWallRunningThisObject = true;
            playerMovement.StartWallRun(this, runDirection.forward);
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
           

            if (isPlayerWallRunningThisObject)
            {
                playerMovement.EndWallRun();
                isPlayerWallRunningThisObject = false;
            }
        }
    }
    */

 
}
