using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CaveInputTransportController : MonoBehaviour
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
    /// Action when the left joystick is moved.
    /// </summary>
    [Tooltip("Action when the left joystick is moved.")]
    [SerializeField] private InputActionProperty rightJoystick;

    [SerializeField] private Vector3 offsetCave;

    [SerializeField] private GrabObjectTransport rightGrab;
    [SerializeField] private GrabObjectTransport leftGrab;

    /// <summary>
    /// Method which initializes each action with the corresponding methods.
    /// </summary>
    private void Start()
    {
        leftJoystick.action.Enable();
        //rightJoystick.action.Enable();

        rightTriggerPressed.action.Enable();
        rightTriggerPressed.action.performed += OnRightTriggerPressed;

        rightTriggerReleased.action.Enable();
        rightTriggerReleased.action.performed += OnRightTriggerReleased;

        leftTriggerPressed.action.Enable();
        leftTriggerPressed.action.performed += OnLeftTriggerPressed;

        leftTriggerReleased.action.Enable();
        leftTriggerReleased.action.performed += OnLeftTriggerReleased;
    }

    private void Update()
    {
        Vector2 direction = leftJoystick.action.ReadValue<Vector2>();
        //Vector2 rotation = rightJoystick.action.ReadValue<Vector2>();
        CAVE.transform.position += Quaternion.Euler(0, headset.transform.eulerAngles.y + 180, 0) * new Vector3(direction.y, 0, -direction.x) * 0.5f * Time.deltaTime;
        //CAVE.transform.Rotate(0, rotation.x * 0.1f, 0);

    }
    
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
}
