using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CaveInputController : MonoBehaviour
{
    /// <summary>
    /// Headset position.
    /// </summary>
    [Tooltip("Headset position.")]
    [SerializeField] private GameObject headset;
    /// <summary>
    /// Left controller position.
    /// </summary>
    [Tooltip("Left controller position.")]
    [SerializeField] private GameObject leftController;
    /// <summary>
    /// Right controller position.
    /// </summary>
    [Tooltip("Right controller position.")]
    [SerializeField] private GameObject rightController;
    /// <summary>
    /// CAVE position.
    /// </summary>
    [Tooltip("CAVE position.")]
    [SerializeField] private GameObject CAVE;


    /// <summary>
    /// Action when the right trigger is released.
    /// </summary>
    [Tooltip("Action when the right trigger is released.")]
    [SerializeField] private InputActionProperty rightTriggerReleased;
    /// <summary>
    /// Action when the right trigger is pressed.
    /// </summary>
    [Tooltip("Action when the right trigger is pressed.")]
    [SerializeField] private InputActionProperty rightTriggerPressed;

    /// <summary>
    /// Action when the left trigger is released.
    /// </summary>
    [Tooltip("Action when the left trigger is released.")]
    [SerializeField] private InputActionProperty leftTriggerReleased;
    /// <summary>
    /// Action when the left trigger is pressed.
    /// </summary>
    [Tooltip("Action when the left trigger is pressed.")]
    [SerializeField] private InputActionProperty leftTriggerPressed;

    /// <summary>
    /// Action when the left joystick is moved.
    /// </summary>
    [Tooltip("Action when the left joystick is moved.")]
    [SerializeField] private InputActionProperty leftJoystick;

    /// <summary>
    /// Action when the right joystick is moved.
    /// </summary>
    [Tooltip("Action when the right joystick is moved.")]
    [SerializeField] private InputActionProperty rightJoystick;

    [SerializeField] private Vector3 offsetCave;

    [SerializeField] private GrabObject rightGrab;
    [SerializeField] private GrabObject leftGrab;

    /// <summary>
    /// Boolean that defines whether the trigger is pressed so that the string is pulled in mid and low control.
    /// </summary>
    // private bool isJoystickTriggered = false;

    // Variable pour stocker la direction de mouvement
    // private Vector2 currentMoveDirection = Vector2.zero;

    /// <summary>
    /// Method which initializes each action with the corresponding methods.
    /// </summary>
    private void Start()
    {
        leftJoystick.action.Enable();
        //joystick.action.performed += OnMovePerformed;

        rightTriggerPressed.action.Enable();
        rightTriggerPressed.action.performed += OnRightTriggerPressed;

        rightTriggerReleased.action.Enable();
        rightTriggerReleased.action.performed += OnRightTriggerReleased;

        leftTriggerPressed.action.Enable();
        leftTriggerPressed.action.performed += OnLeftTriggerPressed;

        leftTriggerReleased.action.Enable();
        leftTriggerReleased.action.performed += OnLeftTriggerReleased;

        rightJoystick.action.Enable();
        rightJoystick.action.performed += OnRightJoystickPushed;
    }

    /// <summary>
    /// Method that once per frame pulls the string if the button is pressed or
    /// increasingly fills the validation button.
    /// </summary>
    private void Update()
    {
        Vector2 direction = leftJoystick.action.ReadValue<Vector2>();
        CAVE.transform.position += new Vector3(direction.x, 0, direction.y) * 0.5f * Time.deltaTime;
    }

    // private void OnMovePerformed(InputAction.CallbackContext context)
    // {
    //     RotateBowForShootDesktop(context.ReadValue<Vector2>());
    // }

    private void OnRightTriggerPressed(InputAction.CallbackContext context)
    {
        rightGrab.TryGrabObject();
    }
 
    private void OnRightTriggerReleased(InputAction.CallbackContext context)
    {
        rightGrab.ReleaseObject();
    }

    private void OnLeftTriggerPressed(InputAction.CallbackContext context)
    {
        leftGrab.TryGrabObject();
    }

    private void OnLeftTriggerReleased(InputAction.CallbackContext context)
    {
        leftGrab.ReleaseObject();
    }

    private void OnRightJoystickPushed(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
        leftGrab.RotateObject(context.ReadValue<Vector2>());
        rightGrab.RotateObject(context.ReadValue<Vector2>());
    }
}
