using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float gravity = -9.81f;
    [Space]
    [SerializeField] private float baseMovementSpeed = 20f;
    [SerializeField] private float jumpStrenght = 40f;
    [SerializeField] private float loseJumpLevelSpeed = 0.1f;
    [Space]
    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private float rotateSensitivity = 50f;
    [SerializeField] private int maxY_Rotation = 80;
    [SerializeField] private int minY_Rotation = -80;
    [Space(10)]
    [SerializeField] private bool isDoubleJumpUnlocked = false;


    private CharacterController cc = null;

    private int stopMovement = 0;


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

    void Start()
    {
        cc = GetComponent<CharacterController>();
        jumpLevel_ = gravity;

        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        if (IsMovementAllowed())
        {
            cc.Move(DetermineMovementVector());
            Rotation();

            if(cc.isGrounded)
            {
                jumpLevel_ = gravity;
                doubleJumpCounter = 1;
            }

            jumpLevel_ -= Time.deltaTime * loseJumpLevelSpeed;
            jumpLevel_ = Mathf.Clamp(jumpLevel_, gravity, Mathf.Infinity);
            print(jumpLevel_);
        }
    }

   
        

    private Vector3 DetermineMovementVector()
    {

        horizontalInput_ = Input.GetAxis("Horizontal");
        verticalInput_ = Input.GetAxis("Vertical");

        movementMultiplier_ = Time.deltaTime * baseMovementSpeed;

        horizontalVector_ = transform.right * (horizontalInput_ * movementMultiplier_);
        verticalVector_ = transform.forward * (verticalInput_ * movementMultiplier_);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cc.isGrounded)
            {

                jumpLevel_ = jumpStrenght;
            }
            else
            {
                if(isDoubleJumpUnlocked)
                {
                    if(doubleJumpCounter == 1)
                    {
                        jumpLevel_ = jumpStrenght;
                        doubleJumpCounter = 0;
                    }
                }
            }
        }

        jumpVector_ = transform.up * jumpLevel_ * Time.deltaTime;

        movementVector_ = horizontalVector_ + verticalVector_ + jumpVector_;

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
        if (_x == 0) { return; }

        yLook_ = _x * rotateSensitivity * Time.deltaTime;

        rotationVector_ = transform.up * yLook_;

        transform.rotation *= Quaternion.Euler(rotationVector_);
    }

    public void RotateCamera(float _y)
    {
        if (_y == 0) { return; }

        xLook_ = _y * rotateSensitivity * Time.deltaTime;

        currentCameraRotation_X -= xLook_;
        currentCameraRotation_X = Mathf.Clamp(currentCameraRotation_X, minY_Rotation, maxY_Rotation);
        playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotation_X, 0f, 0f);
    }
    #endregion


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
    }

    public void GiveDoubleJump()
    {
        doubleJumpCounter = 1;
    }
}
