using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float gravity = -9.81f;
    [Space]
    [SerializeField] private float baseMovementSpeed = 20f;
    [SerializeField] private float jumpStrenght = 40f;
    [SerializeField] private float loseJumpLevelSpeed = 0.1f;
    [Space]
    
    [SerializeField] private float rotateSensitivity = 50f;
    [SerializeField] private int maxY_Rotation = 80;
    [SerializeField] private int minY_Rotation = -80;
    [Space(10)]
    [SerializeField] private bool isDoubleJumpUnlocked = false;
    [SerializeField] private bool isDoubleJumpResetable = false;
    [Space]
    [SerializeField] private bool isWallRunUnlocked = false;
    [SerializeField] private float wallRunSpeed = 5f;
    [SerializeField] private float wallRunTurnSpeed = 2f;
    [SerializeField] private float wallRunCameraTiltAngle = 45f;
    [SerializeField] private float wallRunCameraTurnSpeed = 2f;

    [Space]
    [SerializeField] private float grapplingMovementForce = 0.2f;
    [Space]
     private Image doubleJumpIcon = null;
    


    private CharacterController cc = null;
    private Camera playerCamera = null;
    private PlayerGrapple playerGrappling = null;

    private int stopMovement = 0;
    private bool isWallRunning = false;
    private bool isGrappling = false;


    private Vector3 movementVector_;
    private float horizontalInput_;
    private float verticalInput_;
    private Vector3 horizontalVector_;
    private Vector3 verticalVector_;
    private float movementMultiplier_;

    private Vector3 jumpVector_;
    private float jumpLevel_ = 0;
    private int doubleJumpCounter = 1;


    private float x_MouseInput_ = 0;
    private float y_MouseInput_ = 0;
    private Vector3 rotationVector_ = Vector3.zero;
    private float yLook_ = 0;
    private float xLook_ = 0;
    private float currentCameraRotation_X = 0;

    private WallRunObject wallRunObject = null;
    private Vector3 wallRunDir;
    private int wallRunCameraTiltMultiplier_ = 1;
    private float wallRunAngle_ = 0;

    private bool isGamePaused = false;
    private bool isGameOver = false;

    private PlayerEffects playerEffects = null;
    

    private bool lastFrameGroundedState_ = true;
    private Coroutine headBobCoroutine = null;

    private Vector3 cameraBasePos;
    private Quaternion cameraBaseRot;
    private float headBobProgress = 0f;
    [SerializeField] private Vector3 y_cameraMoveHeadBob = new Vector3(0,0.2f,0);
    [SerializeField] private float headBobDownSpeed = 5f;
    [SerializeField] private float headBobUpSpeed = 5f;
    [Space]
    [SerializeField] private Transform deathCameraPosRot = null;
    [SerializeField] private float deathCameraLerpSpeed = 5f;


    private bool firstMoveDone = false;
    public static Action OnFirstMovement;

    void Start()
    {
        Application.targetFrameRate = 120;

        cc = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        playerGrappling = GetComponent<PlayerGrapple>();
        jumpLevel_ = gravity;

        playerEffects = GetComponent<PlayerEffects>();
        cameraBasePos = playerCamera.transform.localPosition;
        cameraBaseRot = playerCamera.transform.localRotation;

        doubleJumpIcon = FindObjectOfType<Canvas>().transform.Find("DoubleJumpIcon").GetComponent<Image>();

        Cursor.lockState = CursorLockMode.Locked;

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
        

        if(_state == true)
        {
            playerEffects.Audio_StopRun();
        }
        else
        {
            lastFrameGroundedState_ = true;
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        StartCoroutine(LerpToCameraDeathPosition());

        doubleJumpIcon.enabled = false;
        playerEffects.Audio_StopRun();
        playerEffects.Audio_Lava();
        playerEffects.ScreenEffect_Lava();
    }

    void Update()
    {
        
        if (isGamePaused || isGameOver)
        {
            return; 
        }
     

        if (IsMovementAllowed())
        {
            if (!isWallRunning)
            {
                if (!isGrappling)
                {
                    cc.Move(DetermineMovementVector());
                }
                else
                {
                    playerGrappling.GetRb().AddForce(DetermineMovementVector() * grapplingMovementForce);
                }
            }

            Rotation();

            if(cc.isGrounded)
            {
                jumpLevel_ = gravity;
                doubleJumpCounter = 1;
            }

            jumpLevel_ -= Time.deltaTime * loseJumpLevelSpeed;
            jumpLevel_ = Mathf.Clamp(jumpLevel_, gravity, Mathf.Infinity);
          //  print(jumpLevel_);
        }

        if(isWallRunning)
        {
            WallRun();
            CheckWallRunBreak();
        }



        if(!cc.isGrounded && doubleJumpCounter > 0 && !isWallRunning && !isGrappling)
        {
            if (!doubleJumpIcon.enabled)
            {
                doubleJumpIcon.enabled = true;
            }
        }
        else
        {
            if (doubleJumpIcon.enabled)
            {
                doubleJumpIcon.enabled = false;
            }
        }


        if (cc.isGrounded)
        {
            if (lastFrameGroundedState_ != cc.isGrounded)
            {
              //  if (jumpLevel_ <  jumpStrenght /2)
                {
                    if (headBobCoroutine != null)
                        StopCoroutine(headBobCoroutine);
                    headBobCoroutine = StartCoroutine(HeadBob());
                }

                playerEffects.Audio_Land();
            }
        }

        lastFrameGroundedState_ = cc.isGrounded;

    }



    IEnumerator HeadBob()
    {
        headBobProgress = 0;
        while (headBobProgress < 1)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(cameraBasePos, cameraBasePos - y_cameraMoveHeadBob, headBobProgress + (headBobDownSpeed * Time.deltaTime));
            headBobProgress += headBobDownSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
           
        }

        headBobProgress = 0;
        while (headBobProgress < 1)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(cameraBasePos - y_cameraMoveHeadBob, cameraBasePos, headBobProgress + (headBobUpSpeed * Time.deltaTime));
            headBobProgress += headBobUpSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }


        yield return null;
    }


    IEnumerator LerpToCameraDeathPosition()
    {

        if(headBobCoroutine != null)
        {
            StopCoroutine(headBobCoroutine);
            headBobCoroutine = null;
        }

        float _deathCamLerpProgress = 0;

        while (_deathCamLerpProgress < 1)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(cameraBasePos, deathCameraPosRot.localPosition, _deathCamLerpProgress + (deathCameraLerpSpeed * Time.deltaTime));
            playerCamera.transform.localRotation = Quaternion.Lerp(cameraBaseRot, deathCameraPosRot.localRotation, _deathCamLerpProgress + (deathCameraLerpSpeed * Time.deltaTime));

            _deathCamLerpProgress += deathCameraLerpSpeed * Time.deltaTime;

            yield return new WaitForEndOfFrame();

        }

        yield return null;
    }
        

    private Vector3 DetermineMovementVector()
    {
        

        horizontalInput_ = Input.GetAxis("Horizontal");
        verticalInput_ = Input.GetAxis("Vertical");

        movementMultiplier_ = Time.deltaTime * baseMovementSpeed;

        horizontalVector_ = transform.right * (horizontalInput_ * movementMultiplier_);
        verticalVector_ = transform.forward * (verticalInput_ * movementMultiplier_);

        if((horizontalVector_ != Vector3.zero || verticalVector_ != Vector3.zero) && cc.isGrounded)
        {
            playerEffects.Audio_Running();
        }
        else
        {
            playerEffects.Audio_StopRun();
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cc.isGrounded)
            {
                jumpLevel_ = jumpStrenght;

                playerEffects.Audio_Jump();
            }
            else
            {
                if(isDoubleJumpUnlocked)
                {
                    if(doubleJumpCounter > 0)
                    {
                        jumpLevel_ = jumpStrenght;
                        doubleJumpCounter--;

                        playerEffects.Audio_Jump();
                    }
                }
            }
        }

        jumpVector_ = transform.up * jumpLevel_ * Time.deltaTime;

        movementVector_ = horizontalVector_ + verticalVector_ + jumpVector_;


        if (!firstMoveDone)
        {
            if (horizontalInput_ > 0 || verticalInput_ > 0 || Input.GetKeyDown(KeyCode.Space))
            {
                firstMoveDone = true;
                OnFirstMovement?.Invoke();
            }
        }

        return movementVector_;
    }


    #region Rotation
    private void Rotation()
    {
        if(playerCamera == null) { Debug.LogError("There's no camera attached to the player"); return; }

        x_MouseInput_ = Input.GetAxis("Mouse X");
        y_MouseInput_ = Input.GetAxis("Mouse Y");

        RotatePlayer(x_MouseInput_);
        RotateCamera(y_MouseInput_);
    }

    public void RotatePlayer(float _x)
    {
        if(isWallRunning) { return; }

        if (_x == 0) { return; }

        yLook_ = _x * rotateSensitivity * Time.deltaTime;

        rotationVector_ = transform.up * yLook_;

        transform.rotation *= Quaternion.Euler(rotationVector_);
    }

    public void RotateCamera(float _y)
    {

        xLook_ = _y * rotateSensitivity * Time.deltaTime;

        currentCameraRotation_X -= xLook_;
        currentCameraRotation_X = Mathf.Clamp(currentCameraRotation_X, minY_Rotation, maxY_Rotation);

        if (!isWallRunning)
        {
          //  playerCamera.transform.localEulerAngles = Vector3.Lerp(new Vector3(currentCameraRotation_X, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z),
         // new Vector3(currentCameraRotation_X, 0, 0f), wallRunCameraTurnSpeed * Time.deltaTime);

            playerCamera.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(currentCameraRotation_X, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z)),
         Quaternion.Euler(new Vector3(currentCameraRotation_X, 0, 0)),
          wallRunCameraTurnSpeed * Time.deltaTime);
        }
        else
        {
            playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotation_X, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z);
        }
    


       // if (_y == 0) { return; }

       

      
      

        // playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotation_X, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z);

        /*
        if (isWallRunning)
        {
            playerCamera.transform.localEulerAngles = Vector3.Slerp(new Vector3(playerCamera.transform.localEulerAngles.x, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z),
                new Vector3(currentCameraRotation_X, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z), wallRunCameraTurnSpeed * Time.deltaTime);
        }
        else
        {
            playerCamera.transform.localEulerAngles = Vector3.Slerp(new Vector3(playerCamera.transform.localEulerAngles.x, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z),
               new Vector3(currentCameraRotation_X, 0, 0), wallRunCameraTurnSpeed * Time.deltaTime);
        }
        */

        //playerCamera.transform.localEulerAngles = Vector3.Slerp(new Vector3(currentCameraRotation_X, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z),
        //   new Vector3(currentCameraRotation_X, 0f, 0f), wallRunCameraTurnSpeed * Time.deltaTime);

        // playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotation_X, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z);
    }
    #endregion


   
    
    private void CheckWallRunBreak()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EndWallRun();
        }
    }


    private bool IsMovementAllowed()
    {
        if (stopMovement == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }







    public void UpDraft(float _strenght, bool _releaseFromGround)
    {
        if(_releaseFromGround)
        {
            jumpLevel_ = jumpStrenght;
        }

        jumpLevel_ += _strenght;
        doubleJumpCounter = 1;
    }

    public void GiveDoubleJump()
    {
        if (!isDoubleJumpResetable) { return; }

        doubleJumpCounter ++;
        print(doubleJumpCounter);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //temp
        if (hit.transform.tag == "FallProtection")
        {
            transform.position = new Vector3(0, 1, 0);
        }

        if (!isWallRunUnlocked) { return; }

        if (hit.gameObject.layer == 8 /*Wall run layer*/ )
        {
            if(isWallRunning) { return; }

            if(isGrappling) { return; }

           WallRunObject _wro = hit.gameObject.GetComponent<WallRunObject>();

            StartWallRun(_wro,_wro.runDirection.forward);
        }
    }

   


    public void StartWallRun(WallRunObject _wro, Vector3 _runDir)
    {
        // check run direction

        if(Vector3.Dot(transform.forward,_runDir) < 0)
        {
            _runDir = -_runDir;
            wallRunCameraTiltMultiplier_ = 1;
        }
        else
        {
            wallRunCameraTiltMultiplier_ = -1;
        }

        wallRunDir = _runDir;

        jumpLevel_ = 0;

        isWallRunning = true;
        wallRunObject = _wro;

        if(isDoubleJumpResetable)
        {
            doubleJumpCounter = 1;
        }
    }

    private void WallRun()
    {
        cc.Move(wallRunDir * Time.deltaTime * wallRunSpeed);

       

        // camera movement, player rotation

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(wallRunDir), wallRunTurnSpeed * Time.deltaTime);

        if (wallRunCameraTiltMultiplier_ == 1)
        {
            wallRunAngle_ =  -wallRunCameraTiltAngle;
        }
        else
        {
            wallRunAngle_ = wallRunCameraTiltAngle;
        }



        playerCamera.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(currentCameraRotation_X, 0, playerCamera.transform.localEulerAngles.z)),
            Quaternion.Euler(new Vector3(currentCameraRotation_X, 0, wallRunAngle_)),
             wallRunCameraTurnSpeed * Time.deltaTime);

        //    playerCamera.transform.localEulerAngles = Vector3.Lerp(new Vector3(currentCameraRotation_X, 0, playerCamera.transform.localEulerAngles.z),
        //     new Vector3(currentCameraRotation_X, 0, wallRunAngle_), wallRunCameraTurnSpeed * Time.deltaTime);


        //  new Vector3(currentCameraRotation_X, playerCamera.transform.localEulerAngles.y, wallRunCameraTiltAngle);


        //Quaternion.Slerp(playerCamera.transform.rotation, Quaternion.Euler(0f, wallRunCameraTiltAngle, 0f), wallRunCameraTurnSpeed); 

        playerEffects.Audio_WallRunning();

        if ((wallRunObject.transform.position - transform.position).magnitude > wallRunObject.breakDistance)
        {
            EndWallRun();
        }
    }

    public void EndWallRun()
    {
        isWallRunning = false;
        wallRunObject = null;

        jumpLevel_ = jumpStrenght;

        playerEffects.Audio_StopRun();
        playerEffects.Audio_Jump();
    }


    public bool IsWallRunning()
    {
        return isWallRunning;
    }

    public Vector3 GetPlayerVelocity()
    {
        return cc.velocity;
    }

    public void StartedGrappling()
    {
        isGrappling = true;
        cc.enabled = false;
        
    }

    public void EndedGrappling()
    {
        jumpLevel_ = jumpStrenght;

        if (isDoubleJumpResetable)
        {
            doubleJumpCounter = 1;
        }

        isGrappling = false;
        cc.enabled = true;

        playerEffects.Audio_Jump();
    }


  
}
