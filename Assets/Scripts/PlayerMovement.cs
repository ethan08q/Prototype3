using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float airSpeed;
    public float wallrunSpeed;
    public float slideSpeed;


    [Header("Ground Check")]
    public float playerHeight;
    public float groundDrag;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public float airTime;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public Vector3 turn;



    public enum MovementState
    {
        walking,
        sprinting,
        sliding,
        wallrunning,
        air
    }
    public bool wallrunning;

    public List<Vector3> walls;


    private void Start()
    {
        readyToJump = true;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        airTime = 0f;
    }


    private void FixedUpdate()
    {

        if (state != MovementState.air) { airTime = 0f; }
        switch (state)
        {
            case MovementState.walking:
                MovePlayer(walkSpeed);
                break;
            case MovementState.air:
                MovePlayer(airSpeed);
                airTime += Time.fixedDeltaTime;
                rb.AddForce(Vector3.down * airTime * 15f); //add gravity
                break;
            case MovementState.sprinting:
                MovePlayer(sprintSpeed);
                break;
            case MovementState.wallrunning:
                state = MovementState.wallrunning;
                MovePlayer(wallrunSpeed);
                break;
            case MovementState.sliding:
                MovePlayer(slideSpeed);
                break;
        }

       // walls.Clear();
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        Vector3 dir = MyInput();
        StateHandler();

        if (state != MovementState.air)
        {
            airTime = 0f;
        }
        else { airTime += Time.deltaTime; }
    }

    private Vector3 MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        return new Vector3(horizontalInput, 0f, verticalInput);
    }



    private void StateHandler()
    {
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
        }

        else if (grounded)
        {
            state = MovementState.walking;
        }

        else
        {
            state = MovementState.air;
        }
    }
    private void MovePlayer(float speed)
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
       // foreach (Vector3 v in walls) { moveDirection += v; }

        if (grounded)
        {
            rb.linearDamping = groundDrag;
            rb.AddForce(moveDirection * speed * 10f, ForceMode.Force);

        }
        else if (!grounded)
        {
            rb.linearDamping = 0f;
            rb.AddForce(moveDirection.normalized * speed * 10f * airMultiplier, ForceMode.Force);
        }
        SpeedControl(speed);
    }

    private void SpeedControl(float speed)
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

   // void OnCollisionStay(Collision collision)
 //   {
  //      Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.magenta);
  //
   //     ContactPoint[] _contacts = new ContactPoint[collision.contactCount];
    //    for (int i = 0; i < _contacts.Length; i++)
    //    {
    //        _contacts[i] = collision.GetContact(i);
     //   }

     //   Vector3 normal = Vector3.zero;
     //   foreach (ContactPoint p in _contacts) { normal += p.normal; }
     //   normal /= _contacts.Length;
     //   if (Vector3.Dot(normal, Vector3.up) < .95) { walls.Add(normal); }

   // }
}



