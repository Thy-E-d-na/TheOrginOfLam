using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController charCtrl;
    private float gravity = -9.81f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotateForce;
    private InputAction moveAction;
    [SerializeField] private InputActionReference jumpAction;
    Vector3 velocity;

    private void Start()
    {
        charCtrl = GetComponent<CharacterController>();
        moveAction = InputSystem.actions["Move"];
    }
    private void Update()
    {
        Move();
    }

    void Move()
    {
        velocity.y += gravity * Time.deltaTime;
        var input = moveAction.ReadValue<Vector2>();

        var dir = (transform.forward * input.y);
        var rot = (transform.right * input.x).normalized;
        if (charCtrl.isGrounded && jumpAction.action.WasPressedThisFrame())
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        if (rot != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rot), rotateForce * Time.deltaTime);
        }
        charCtrl.Move((dir * speed + velocity) * Time.deltaTime);
    }
}
