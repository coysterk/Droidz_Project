using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    public float jumpHeight = 1.5f;
    public float gravityValue = -9.81f;

    public CharacterController controller;

    private float verticalVelocity;

    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {
        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < -2f)
            verticalVelocity = -2f;

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        float rotateAmount = input.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotateAmount, 0);

        Vector3 forwardMove = transform.forward * input.y * moveSpeed;

        if (grounded && jumpAction.action.WasPressedThisFrame())
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityValue);

        verticalVelocity += gravityValue * Time.deltaTime;

        Vector3 motion = forwardMove + Vector3.up * verticalVelocity;
        controller.Move(motion * Time.deltaTime);
    }
}