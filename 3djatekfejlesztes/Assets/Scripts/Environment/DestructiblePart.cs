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

    private BreakableSound breakAudio;

    private bool brokenOff = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        breakAudio = transform.parent.GetComponent<BreakableSound>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {


        if (!brokenOff)
        {
            if (reactionLayers == (reactionLayers | (1 << collision.gameObject.layer)))
            {
                print("collide " + collision.rigidbody.velocity.magnitude);
                if (collision.rigidbody.velocity.magnitude + (collision.rigidbody.mass / 3) > breakVelocityMag)
                {
                    print("break " + collision.rigidbody.velocity.magnitude + collision);
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    gameObject.layer = collision.gameObject.layer; // we can now manipulate the object after breaking

                    StartCoroutine(ResetVel(collision.rigidbody, collision.rigidbody.velocity));

                    brokenOff = true;

                    if(breakAudio != null)
                    {
                        breakAudio.Break();
                    }
                }
            }


        }
    }

    IEnumerator ResetVel(Rigidbody _rb,  Vector3 _vel)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _rb.velocity = _vel;
    }
}
