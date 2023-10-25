using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Input Fields
    private PlayerInput playerInput;
    private InputAction move;

    // Movement
    private Rigidbody rb;

    public float movementForce = 1f;
    public float jumpForce = 5f;
    public float maxSpeed = 5f;
    public float rotationSpeed = 5f;  // Rotation speed

    private Vector3 forceDirection = Vector3.zero;

    // Camera
    [SerializeField]
    private Camera playerCamera;
    private Animator anim;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerInput = new PlayerInput();
        anim = this.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerInput.Player.Jump.started += DoJump;
        playerInput.Player.Attack.started += DoAttack;
        move = playerInput.Player.Move;
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Jump.started -= DoJump;
        playerInput.Player.Attack.started -= DoAttack;
        playerInput.Player.Disable();
    }

    private void FixedUpdate()
    {
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * getCameraForward(playerCamera) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f)
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        LookAt();
    }

    private void LookAt()
    {
        Vector2 movementInput = move.ReadValue<Vector2>();

        // Only update the rotation if there is significant input
        if (movementInput.sqrMagnitude > 0.1f)
        {
            Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);
            Vector3 cameraForward = getCameraForward(playerCamera);
            Vector3 cameraRight = GetCameraRight(playerCamera);
            Vector3 desiredDirection = cameraForward * moveDirection.z + cameraRight * moveDirection.x;
            if (desiredDirection.sqrMagnitude > 0.1f)
            {
                desiredDirection = Vector3.Normalize(desiredDirection);  // Normalize the direction vector
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }
        }
        else
        {
            // If there's no input, stop rotating
            rb.angularVelocity = Vector3.zero;
        }
    }


    private Vector3 getCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            forceDirection += Vector3.up * jumpForce;
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
            return true;
        else
            return false;
    }

    private void DoAttack(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Attack");
    }
}
