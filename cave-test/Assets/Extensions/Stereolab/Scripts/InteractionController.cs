using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private InputActionProperty joystick;
    [SerializeField] private float speed;
    [SerializeField] private CharacterController characterController;

    private Vector2 _inputs;

    // Start is called before the first frame update
    void Start()
    {
        joystick.action.Enable();
        joystick.action.performed += PlayerMovement;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void PlayerMovement(InputAction.CallbackContext context)
    {
        _inputs = context.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(_inputs.x, 0, _inputs.y);
        Vector3 velocity = direction * speed;
        characterController.Move(transform.TransformDirection(velocity * Time.deltaTime));
    }
}
