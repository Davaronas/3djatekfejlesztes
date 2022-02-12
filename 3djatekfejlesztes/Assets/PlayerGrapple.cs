using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 50f;

    [SerializeField] private LineRenderer lineRenderer = null;
    [SerializeField] private Transform grapplePos = null;
    private Vector3 grapplePoint;
    private Camera playerCamera = null;
    private PlayerMovement playerMovement = null;

    private bool isGrappling = false;


    private RaycastHit rh_;
    private SpringJoint joint_ = null;
    private Rigidbody rb_ = null;
    private CapsuleCollider capsuleCollider_ = null;
    private float grappleDistance_;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out rh_, raycastDistance))
            {
                if (rh_.transform.gameObject.layer == 10) // grapple layer
                {
                    StartGrapple(rh_.point);
                }
            }

        }
        else if(Input.GetKeyUp(KeyCode.F))
        {
            StopGrapple();
        }

        if(isGrappling)
        {
            lineRenderer.SetPosition(0, grapplePos.position);
        }
    }


    private void StartGrapple(Vector3 _pos)
    {
        isGrappling = true;

        playerMovement.StartedGrappling();

        print("Start grapple");

        rb_ = gameObject.AddComponent<Rigidbody>();
        rb_.useGravity = true;
        rb_.isKinematic = false;
        rb_.constraints = RigidbodyConstraints.FreezeRotation;

        capsuleCollider_ = gameObject.AddComponent<CapsuleCollider>();

        joint_ = gameObject.AddComponent<SpringJoint>();
        joint_.autoConfigureConnectedAnchor = false;
        joint_.connectedAnchor = _pos;

        grappleDistance_ = Vector3.Distance(transform.position, _pos);
        print(grappleDistance_ + " gd");
        joint_.maxDistance = grappleDistance_ * 0.8f;
        joint_.minDistance = grappleDistance_ * 0.25f;

        joint_.spring = 4.5f;
        joint_.damper = 14f; //7f;
        joint_.massScale = 4.5f;


        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, grapplePos.position);
        lineRenderer.SetPosition(1, _pos);
    }

   

    private void StopGrapple()
    {
        if (isGrappling)
        {
            isGrappling = false;
            playerMovement.EndedGrappling();

            Destroy(joint_);
            Destroy(rb_);
            Destroy(capsuleCollider_);

            lineRenderer.positionCount = 0;
        }
    }

    public float GetGrappleDistance()
    {
        return raycastDistance;
    }
}
