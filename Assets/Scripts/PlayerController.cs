using System;
using UnityEngine;
using UnityEngine.InputSystem;


// TODO: Once proper character model is in place, adjust the capsule collider to match

public class PlayerController : MonoBehaviour
{
    // Input
    PlayerInputActions actions;
    [SerializeField] InputAction move;

    // Movement
    private CharacterController control;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 20f; 
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float gravity = 2.2f;

    // Movement is relative to camera so we need it
    [SerializeField] private Camera cam;

    // Temporarily removed jump function since it isn't necessary to fix atm (i just like being able to jump)
    private void PlayerJump(InputAction.CallbackContext context)
    {
        //if (IsGrounded())
        //{
        //    moveDir += Vector3.up * jumpForce;
        //}
    }

    //// Shoots a raycast from above player to the ground to check if player is touching the ground
    //private bool IsGrounded()
    //{
    //    Ray r = new Ray(this.transform.position + Vector3.up * .025f, Vector3.down);
    //    if (Physics.Raycast(r, out RaycastHit hit, 1.05f)) // Max distance should be a little bigger than ray origin offset
    //    {
    //        // TODO: Add more logic so ensure player can't jump off of anything they want, only the ground
    //        return true;
    //    }
    //    return false;

    //}

    private void Awake()
    {
        //body = this.GetComponent<Rigidbody>();
        control = this.GetComponent<CharacterController>();
        actions = new PlayerInputActions();
    }

    // OnEnable & OnDisable subscribe/unsubscribe from actions
    private void OnEnable()
    {
        actions.Player.Jump.started += PlayerJump;
        move = actions.Player.Move;
        actions.Player.Enable();
    }

    private void OnDisable()
    {
        actions.Player.Jump.started -= PlayerJump;
        actions.Player.Disable();
    }

    private void FixedUpdate()
    {
        // Move
        //forceDir += move.ReadValue<Vector2>().x * GetCameraRight(cam) * moveForce;
        //forceDir += move.ReadValue<Vector2>().y * GetCameraForward(cam) * moveForce;

        //body.AddForce(forceDir, ForceMode.Impulse);
        //forceDir = Vector3.zero;
        Vector3 moveDir = Vector3.zero;
        moveDir += move.ReadValue<Vector2>().x * GetCameraRight(cam);
        moveDir += move.ReadValue<Vector2>().y * GetCameraForward(cam);
        Vector3 grav = Vector3.down * gravity;
        control.Move((moveDir + grav) * Time.deltaTime * moveSpeed);


        LookAt();

        // Makes the jump feel worse
        //if (body.linearVelocity.y < 0f)
        //{
        //    body.linearVelocity += Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
        //}

        //Vector3 horizontalVel = body.linearVelocity;
        //horizontalVel.y = 0;
        //if (horizontalVel.sqrMagnitude > maxSpeed * maxSpeed)
        //{
        //    body.linearVelocity = horizontalVel.normalized * maxSpeed + Vector3.up * body.linearVelocity.y;
        //}
    }

    // Player looks in direction of movement
    private void LookAt()
    {
        Vector3 dir = control.velocity;
        dir.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && dir.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }


        Debug.DrawLine(this.transform.position, this.transform.forward);
    }

    // Camera directions normalized to the horizontal plane
    private Vector3 GetCameraForward(Camera cam)
    {
        Vector3 forward = cam.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera cam)
    {
        Vector3 right = cam.transform.right;
        right.y = 0;
        return right.normalized;
    }
}
