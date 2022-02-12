using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DestructiblePart : MonoBehaviour
{
    [SerializeField] private LayerMask reactionLayers;
    [SerializeField] private float breakVelocityMag = 5f;
    private Rigidbody rb = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        

        if(reactionLayers == (reactionLayers | (1 << collision.gameObject.layer)))
        {
            print("collide " + collision.rigidbody.velocity.magnitude);
            if (collision.rigidbody.velocity.magnitude > breakVelocityMag)
            {
                print("break "+ collision.rigidbody.velocity.magnitude + collision);
                rb.isKinematic = false;
                rb.useGravity = true;
                gameObject.layer = collision.gameObject.layer; // we can now manipulate the object after breaking
            }
        }
    }
}
