using CesiumForUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CesiumCaveInput : MonoBehaviour
{
    [SerializeField]
    private GameObject CAVE;
    [SerializeField]
    private CesiumGeoreference CesiumGeoRef;

    private Vector2 moveInputs = Vector2.zero;

    //private Vector2 rotateInputs = Vector2.zero;

    private float rotateLeft = 0f;

    private float rotateRight = 0f;

    private Vector2 elevateInputs = Vector2.zero;

    private float scaleUp = 0f;
    private float scaleDown = 0f;

    private Rigidbody rb;

    [SerializeField]
    private Transform leftHand;

    [SerializeField]
    private GameObject mainCam;
    [SerializeField]
    private Transform initialCamPosition;

    [SerializeField]
    private CesiumFlyToController flyToController;
    [SerializeField]
    private TMP_InputField longitudeInput;
    [SerializeField]
    private TMP_InputField latitudeInput;

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
        //ApplyZoom();
        //ApplyElevate();
        ApplyElevate();
        ApplyScale();
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
    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {
        rotateRight = context.ReadValue<float>();
    }
    /*
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
    */
    public void OnElevate(InputAction.CallbackContext context)
    {
        elevateInputs = context.ReadValue<Vector2>();
    }

    public void OnScaleUp(InputAction.CallbackContext context)
    {
        scaleUp = context.ReadValue<float>();
    }

    public void OnScaleDown(InputAction.CallbackContext context)
    {
        scaleDown = context.ReadValue<float>();
    }

    private void ApplyMove()
    {
        //CAVE.transform.localPosition += new Vector3(moveInputs.x / 2, 0, moveInputs.y / 2);
        //moveInputs = Vector2.zero;
        Vector3 moveDirection = leftHand.right * moveInputs.x + leftHand.forward * moveInputs.y;
        moveDirection *= RelativeSpeed();
        rb.AddForce(moveDirection, ForceMode.Acceleration);
    }

    public void FlyTo()
    {
        if (string.IsNullOrEmpty(longitudeInput.text))
        {
            Debug.LogError("Empty longitude field!");
            return;
        }
        if (string.IsNullOrEmpty(latitudeInput.text))
        {
            Debug.LogError("Empty latitude field!");
            return;
        }
        bool longitudeRelative = false;
        bool addToLongitude = true;
        bool latitudeRelative = false;
        bool addToLatitude = true;
        if (longitudeInput.text[0] == '+' || longitudeInput.text[0] == '-')
        {
            longitudeRelative = true;
            addToLongitude = longitudeInput.text[0] == '+' ? true: false;
            longitudeInput.text.Remove(0, 1);
        }
        if (latitudeInput.text[0] == '+' || latitudeInput.text[0] == '-')
        {
            latitudeRelative = true;
            addToLatitude = latitudeInput.text[0] == '+' ? true : false;
            latitudeInput.text.Remove(0, 1);

        }
        double destLongitude = 0f;
        bool success = double.TryParse(longitudeInput.text, out destLongitude);
        if (!success)
        {
            Debug.LogError("Invalid longitude field: must only contain digits and a dot (.)");
            return;
        }
        Debug.Log("Got destLongitude = " + destLongitude);
        double destLatitude = 0f;
        success = double.TryParse(latitudeInput.text, out destLatitude);
        if (!success)
        {
            Debug.LogError("Invalid latitude field: must only contain digits and a dot (.)");
            return;
        }
        if (destLongitude < -180 || destLongitude > 180)
        {
            Debug.LogError("Invalid longitude field: value must be in range [-180; 180]");
            return;
        }
        if (destLatitude < -90 || destLatitude > 90)
        {
            Debug.LogError("Invalid latitude field: value must be in range [-90; 90]");
        }
        Debug.Log("Got destLatitude = " + destLatitude);
        if (longitudeRelative)
        {
            destLongitude = addToLongitude ? CesiumGeoRef.longitude + destLongitude : CesiumGeoRef.longitude - destLongitude;
            Debug.Log("destLongitude was relative, it is now " + destLongitude);
        }
        if (latitudeRelative)
        {
            destLatitude = addToLatitude ? CesiumGeoRef.latitude + destLatitude : CesiumGeoRef.latitude - destLatitude;
            Debug.Log("destLatitude was relative, it is now" + destLatitude);
        }
        Unity.Mathematics.double3 destination = new Unity.Mathematics.double3(destLongitude, destLatitude, CesiumGeoRef.height);
        flyToController.FlyToLocationLongitudeLatitudeHeight(destination, transform.rotation.z, transform.rotation.y, true);
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
    /*
    private void ApplyZoom()
    {
        if (zoomInputs.y >= 0.5f)
        {
            CAVE.transform.localScale *= 1 + zoomInputs.y;
        }        
    }

    private void ApplyElevate()
    {
        Vector3 elevation = new Vector3(0, goUp - goDown, 0);
        float multiplier = ElevationSpeed();
        elevation *= multiplier;
        rb.AddForce(elevation, ForceMode.VelocityChange);
    }
    */
    private void ApplyElevate()
    {
        Vector3 elevation = new Vector3(0, elevateInputs.y, 0);
        float multiplier = ElevationSpeed();
        elevation *= multiplier;
        rb.AddForce(elevation, ForceMode.VelocityChange);
    }

    private void ApplyScale()
    {
        if (scaleUp > 0.001f) 
        {
            //CAVE.transform.localScale *= 1.1f;
            CesiumGeoRef.height += 10f;
        }
        if (scaleDown > 0.001f)
        {
            //CAVE.transform.localScale /= 1.1f;
            CesiumGeoRef.height -= 10f;
        }
    }

    private float ElevationSpeed()
    {
        return 4 * Mathf.Abs(transform.position.y);
    }

    private float RelativeSpeed()
    {
        return 40 * (Mathf.Abs(transform.position.y) + (float)CesiumGeoRef.height);
    }

    /// Pour une quelconque raison, la caméra attachée au CAVE tombe d'elle même
    /// si le CAVE a un Rigidbody (peu importe si le Rigidbody est kinématique ou ignore la gravité).
    /// Cette méthode s'assure que la caméra reste à sa place.
    private void SaveCamera()
    {
        mainCam.transform.localPosition = initialCamPosition.position;
    }

}
