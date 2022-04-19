using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private bool isForcePushUnlocked = false;
    [SerializeField] private float forcePushStrenght = 5f;
    [Space]
    [SerializeField] private bool isForcePullUnlocked = false;
    [SerializeField] private float forcePullStrenght = 5f;
    [Space]
    [SerializeField] private bool isForceGrabUnlocked = false;
    [SerializeField] private float forceGrabStrenght = 5f;
    [SerializeField] private float maxForceGrabVector = 5f;
    [SerializeField] private Transform forceGrabToPosition = null;
   
    [Space]
    [SerializeField] private float raycastRange = 50f;
    [SerializeField] private LayerMask raycastLayers;


    private Camera playerCamera = null;
    private Rigidbody forceGrabTarget = null;
    private PlayerMovement playerMovement = null;


    private RaycastHit rh_;

    private bool isGamePaused = false;
    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        playerMovement = GetComponent<PlayerMovement>();

        Pause.OnGamePaused += GamePaused;
        DeathCollider.OnPlayerDeadlyCollision += GameOver;
    }

    private void OnDestroy()
    {
        Pause.OnGamePaused -= GamePaused;
        DeathCollider.OnPlayerDeadlyCollision -= GameOver;
    }

    private void GamePaused(bool _state)
    {
        isGamePaused = _state;
    }

    private void GameOver()
    {
        isGameOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isGamePaused || isGameOver) { return; }

        ForcePush();
        ForcePull();
        ForceGrab();
        
    }

    private void LateUpdate()
    {
        MoveForceGrabRigidbody();
    }

    private void FixedUpdate()
    {
       // MoveForceGrabRigidbody();
    }


    private void ForcePush()
    {
        if(!isForcePushUnlocked) { return; }

       

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (forceGrabTarget != null)
            {
                forceGrabTarget.AddForce(playerCamera.transform.forward * forcePushStrenght, ForceMode.Impulse);
                forceGrabTarget = null;
                return;
            }

            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out rh_, raycastRange, raycastLayers))
            {
                if(rh_.rigidbody == null) { Debug.LogError("The object you want to manipulate doesn't have a rigidbody attached"); return; }

                rh_.rigidbody.AddForce(playerCamera.transform.forward * forcePushStrenght,ForceMode.Impulse);

                if(forceGrabTarget != null)
                {
                    forceGrabTarget = null;
                }
            }
        }
    }

    private void ForcePull()
    {
        if (!isForcePullUnlocked || isForceGrabUnlocked) { return; }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out rh_, raycastRange, raycastLayers))
            {
                if (rh_.rigidbody == null) { Debug.LogError("The object you want to manipulate doesn't have a rigidbody attached"); return; }

                rh_.rigidbody.AddForce(-playerCamera.transform.forward * forcePullStrenght, ForceMode.Impulse);
            }
        }
    }

    private void ForceGrab()
    {
        if (!isForceGrabUnlocked) { return; }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out rh_, raycastRange, raycastLayers))
            {
                if (rh_.rigidbody == null) { Debug.LogError("The object you want to manipulate doesn't have a rigidbody attached"); return; }

                forceGrabTarget = rh_.rigidbody;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            forceGrabTarget = null;
        }

        
    }


    private void MoveForceGrabRigidbody()
    {
        if (!isForceGrabUnlocked) { return; }
        if (forceGrabTarget == null) { return; }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            //print("force grab");
            forceGrabTarget.velocity = (Vector3.ClampMagnitude((forceGrabToPosition.transform.position - forceGrabTarget.transform.position) * forceGrabStrenght, maxForceGrabVector) / forceGrabTarget.mass);
               

            //forceGrabTarget.AddForce((forceGrabToPosition.transform.position - forceGrabTarget.transform.position).normalized * forceGrabStrenght,ForceMode.Force);
        }
        
        
    }


    public float GetForceDistance()
    {
        return raycastRange;
    }
}
