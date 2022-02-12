using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDraft : MonoBehaviour
{
    private PlayerMovement player = null;

    [SerializeField] private bool releaseFromGround = false;
    [SerializeField] private float strenght = 2f;
  

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "Player")
        {
            print("TriggerEn");
            player = other.GetComponent<PlayerMovement>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (player != null)
        {
            player.UpDraft(strenght * Time.deltaTime,releaseFromGround);
        }
    }

    private void OnTriggerExit(Collider other)
    {
       
        if (other.tag == "Player")
        {
            print("TriggerEx");
            player = null;
        }
    }
}
