using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRaycaster : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 5f;
    [SerializeField] private float interactDistance = 3f;

    [SerializeField] private Image aidIcon = null;
    [SerializeField] private Sprite interactSprite;
    [SerializeField] private Sprite forceSprite;
    [SerializeField] private Sprite grappleSprite;

    private Camera playerCamera = null;
    private PlayerAbilities playerAbilities = null;
    private PlayerGrapple playerGrapple = null;

    private Interactable hoveredInteractable = null;

    private bool raycastFoundTarget = false;
    private RaycastHit rh_;


    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        playerAbilities = GetComponent<PlayerAbilities>();
        playerGrapple = GetComponent<PlayerGrapple>();

        aidIcon.enabled = false;
    }


    void Update()
    {
        raycastFoundTarget = false;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out rh_, raycastDistance))
        {
            CheckInteractLayer();

            CheckGrappleLayer();

            CheckMovableLayer();
           
        }



        if (!raycastFoundTarget)
        {
           
            aidIcon.enabled = false;
        }
    }


    private void CheckInteractLayer()
    {
        if (rh_.transform.gameObject.layer == 9) //interactalbe layer
        {
            hoveredInteractable = rh_.transform.GetComponent<Interactable>();

            print("layer 9");

            if (rh_.distance > interactDistance) { return; }

            print("layer 9 distance good");

            if (!hoveredInteractable.isInteractable) { return; }

            print("layer 9 interactable");

            if (hoveredInteractable.isOneWayActivation && hoveredInteractable.isActivated) { return; }

            print("layer 9 not one way, and not activated");

            if (aidIcon.enabled == false)
            {
                aidIcon.sprite = interactSprite;
                aidIcon.enabled = true;
            }

            raycastFoundTarget = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                hoveredInteractable.Interacted();
            }
        }
    }

    private void CheckGrappleLayer()
    {
        if (rh_.transform.gameObject.layer == 10) //grapple layer
        {
            if (rh_.distance > playerGrapple.GetGrappleDistance()) { return; }

            if (aidIcon.enabled == false)
            {
                aidIcon.sprite = grappleSprite;
                aidIcon.enabled = true;
            }

            raycastFoundTarget = true;
        }
    }

    private void CheckMovableLayer()
    {
        if (rh_.transform.gameObject.layer == 7) // movable layer
        {
            if (rh_.distance > playerAbilities.GetForceDistance()) { return; }

            if (aidIcon.enabled == false)
            {
                aidIcon.sprite = forceSprite;
                aidIcon.enabled = true;
            }

            raycastFoundTarget = true;
        }
    }
}
