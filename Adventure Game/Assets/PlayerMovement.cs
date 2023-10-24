using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;

    private int isWalkingHash;
    private int isRunningHash;

    private PlayerInput input;

    private Vector2 currentMovement;
    private bool movementPressed;
    private bool runPressed;

    private void Awake()
    {
        input = new PlayerInput();

        input.PlayerControls.Movement.performed += ctx =>
        {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
        };
        input.PlayerControls.Run.performed += ctx => runPressed = ctx.ReadValueAsButton();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleRotation()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = new Vector3(currentMovement.x, 0, currentMovement.y);
        Vector3 positionToLookAt = currentPosition + newPosition;
        transform.LookAt(positionToLookAt);
    }

    private void HandleMovement()
    {
        bool isRunning = anim.GetBool(isRunningHash);
        bool isWalking = anim.GetBool(isWalkingHash);

        if (movementPressed && !isWalking)
        {
            anim.SetBool(isWalkingHash, true);
        }

        if (!movementPressed && isWalking)
        {
            anim.SetBool(isWalkingHash, false);
        }

        if ((movementPressed && runPressed) && !isRunning)
        {
            anim.SetBool(isRunningHash, true);
        }

        if ((!movementPressed || !runPressed) && isRunning)
        {
            anim.SetBool(isRunningHash, false);
        }
    }

    private void OnEnable()
    {
        input.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        input.PlayerControls.Disable();
    }
}