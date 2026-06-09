using UnityEngine;
using UnityEngine.InputSystem;

public class CesiumCaveInput : MonoBehaviour
{
    [SerializeField]
    private GameObject CAVE;

    private Vector2 moveInputs = Vector2.zero;

    //private Vector2 rotateInputs = Vector2.zero;

    private float rotateLeft = 0f;

    private float rotateRight = 0f;

    private Vector2 zoomInputs = Vector2.one;

    private float goUp = 0f;
    private float goDown = 0f;

    private Rigidbody rb;

    [SerializeField]
    private Transform leftHand;

    [SerializeField]
    private GameObject mainCam;
    [SerializeField]
    private Transform initialCamPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = 20f;
        rb.maxAngularVelocity = 2f;
    }

    private void FixedUpdate()
    {
        ApplyMove();
        ApplyRotate();
        ApplyZoom();
        ApplyElevate();
        SaveCamera();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputs = context.ReadValue<Vector2>();
    }
    
    /*
    public void OnRotate(InputAction.CallbackContext context)
    {
        rotateInputs = context.ReadValue<Vector2>();
    }
    */
    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        rotateLeft = context.ReadValue<float>();
        Debug.Log("ZL pressed! rotateLeft is " + rotateLeft);
    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {
        rotateRight = context.ReadValue<float>();
        Debug.Log("ZR pressed! rotateRight is " + rotateRight);
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        zoomInputs = context.ReadValue<Vector2>();
        if (zoomInputs == Vector2.zero)
        {
            zoomInputs = Vector2.one;
        }
    }

    public void OnGoUp(InputAction.CallbackContext context)
    {
        goUp = context.ReadValue<float>();
        Debug.Log("Left/Up pressed!");
    }

    public void OnGoDown(InputAction.CallbackContext context)
    {
        goDown = context.ReadValue<float>();
        Debug.Log("Left/Down pressed!");
    }

    private void ApplyMove()
    {
        //CAVE.transform.localPosition += new Vector3(moveInputs.x / 2, 0, moveInputs.y / 2);
        //moveInputs = Vector2.zero;
        Vector3 moveDirection = leftHand.right * moveInputs.x + leftHand.forward * moveInputs.y;
        moveDirection *= 40f;
        rb.AddForce(moveDirection, ForceMode.Acceleration);
    }

    private void ApplyRotate()
    {
        //CAVE.transform.Rotate(new Vector3(rotateInputs.y, rotateInputs.x, 0));
        //rotateInputs = Vector2.zero;
        //CAVE.transform.Rotate(Vector3.zero);
        if (rotateLeft <= 0.001f) 
        {
            if (rotateRight <= 0.001f)
            {
                return;
            }
            else
            {
                rb.AddTorque(Vector3.up * rotateRight * 10f, ForceMode.VelocityChange);
            }
        }
        else 
        {
            if (rotateRight <= 0.001f)
            {
                rb.AddTorque(Vector3.up * -rotateLeft * 10f, ForceMode.VelocityChange);
            }
            else
            {
                return;
            }    
        }
    }

    private void ApplyZoom()
    {
        CAVE.transform.localScale *= zoomInputs.y;
        zoomInputs = Vector2.one;
    }

    private void ApplyElevate()
    {
        Vector3 elevation = new Vector3(0, goUp - goDown, 0);
        elevation *= 40f;
        rb.AddForce(elevation, ForceMode.Acceleration);
    }

    /// Pour une quelconque raison, la caméra attachée au CAVE tombe d'elle même
    /// si le CAVE a un Rigidbody (peu importe si le Rigidbody est kinématique ou ignore la gravité).
    /// Cette méthode s'assure que la caméra reste à sa place.
    private void SaveCamera()
    {
        mainCam.transform.localPosition = initialCamPosition.position;
    }

}
