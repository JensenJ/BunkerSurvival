using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//The player controller is a unit controlled by a player

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(PlayerFlashLight))]
[RequireComponent(typeof(PlayerAttributes))]
public class PlayerController : NetworkBehaviour
{
    public bool isCursorEnabled = false;
    public bool canMove = true;
    [SerializeField] [Range(0, 10)] public float speed = 5.0f;
    [SerializeField] [Range(0, 15)] public float sprintSpeed = 7.0f;
    [SerializeField] [Range(0, 5)] public float sensitivity = 3.0f;
    [SerializeField] [Range(0, 20)] public float jumpForce = 2.0f;

    NetworkUtils netUtils = null;
    GameObject gameManager = null;
    PlayerMotor motor = null;
    PlayerAttributes attributes = null;

    // Start is called before the first frame update
    void Start()
    {
        //Start settings
        motor = GetComponent<PlayerMotor>();
        attributes = GetComponent<PlayerAttributes>();
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        netUtils = gameManager.GetComponent<NetworkUtils>();
        DisableCursor();
    }

    // Update is called once per frame
    void Update()
    {
        //Check that this is owned by player
        if(hasAuthority == false)
        {
            return;
        }

        //Toggle cursor activation
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (isCursorEnabled)
            {
                DisableCursor();
                canMove = true;
            }
            else
            {
                EnableCursor();
                canMove = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
            if(host != null)
            {
                host.CmdToggleFlashLight();
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            attributes.DamageHealth(10.0f);
        }

        //Movement
        Move();
        Rotate();
        Jump();
    }

    //Function to enable cursor, used in menus
    void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        isCursorEnabled = true;
    }

    //Function to disable cursor, used in menus
    void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isCursorEnabled = false;
    }

    //Movement
    void Move()
    {
        Vector3 velocity = Vector3.zero;
        //Checks whether player can move
        if (canMove)
        {
            float xMove = Input.GetAxisRaw("Horizontal");
            float zMove = Input.GetAxisRaw("Vertical");

            Vector3 moveHorizontal = transform.right * xMove;
            Vector3 moveVertical = transform.forward * zMove;

            velocity = (moveHorizontal + moveVertical).normalized * speed;
        }

        motor.Move(velocity);
    }

    //Rotation
    void Rotate()
    {
        Vector3 rotation = Vector3.zero;
        float CameraRotationX = 0.0f;
        //Checks whether player can move
        if (canMove)
        {
            float yRot = Input.GetAxisRaw("Mouse X");
            float xRot = Input.GetAxisRaw("Mouse Y");

            rotation = new Vector3(0.0f, yRot, 0.0f) * sensitivity;
            CameraRotationX = xRot * sensitivity;
        }
        motor.Rotate(rotation);
        motor.RotateCamera(CameraRotationX);
    }

    void Jump()
    {
        if (canMove)
        {
            if (Input.GetButtonDown("Jump") && motor.IsGrounded())
            {
                motor.Jump(jumpForce);
            }
        }
    }
}
