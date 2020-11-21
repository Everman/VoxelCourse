using UnityEngine;
using System.Collections;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class UnityMovementExample : MonoBehaviour
{
    CharacterController characterController;

    [SerializeField] private float normalSpeed = 6.0f;
    [SerializeField] private float sprintSpeed = 12.0f;
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private float gravity = 20.0f;

    float speed;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetButton("Sprint")) {
            speed = sprintSpeed;
        } else {
            speed = normalSpeed;
        }

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate

            // Get Input
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // We do this on the transform so that it uses the direction the player is looking
            moveDirection = transform.right * x + transform.forward * z;
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime); 
    }
}