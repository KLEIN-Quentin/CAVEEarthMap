using CesiumForUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class CesiumCaveInput : MonoBehaviour
{
    [SerializeField]
    private GameObject CAVE;
    [SerializeField]
    private CesiumGeoreference CesiumGeoRef;
    [SerializeField]
    private GameObject[] Tiles;

    private Vector2 moveInputs = Vector2.zero;

    //private Vector2 rotateInputs = Vector2.zero;

    private float rotateLeft = 0f;

    private float rotateRight = 0f;

    private Vector2 elevateInputs = Vector2.zero;

    private float heightUp = 0f;
    private float heightDown = 0f;

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
        //ApplyZoom();
        //ApplyElevate();
        ApplyElevate();
        ApplyHeight();
        InterpolateRotationToSurface();
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
        heightUp = context.ReadValue<float>();
    }

    public void OnScaleDown(InputAction.CallbackContext context)
    {
        heightDown = context.ReadValue<float>();
    }

    private void ApplyMove()
    {
        //CAVE.transform.localPosition += new Vector3(moveInputs.x / 2, 0, moveInputs.y / 2);
        //moveInputs = Vector2.zero;
        Vector3 moveDirection = leftHand.right * moveInputs.x + leftHand.forward * moveInputs.y;
        moveDirection *= RelativeSpeed();
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

    private void ApplyHeight()
    {
        CesiumGeoRef.height += HeightChangeSpeed();
        if (heightUp > 0.001f) 
        {
            //CAVE.transform.localScale *= 1.1f;
            CesiumGeoRef.height += HeightChangeSpeed();
            if (CesiumGeoRef.height > 5000f)
            {
                foreach (GameObject tile in Tiles)
                {
                    tile.transform.localScale *= HeightChangeSpeed();
                }
            }
        }
        if (heightDown > 0.001f)
        {
            //CAVE.transform.localScale /= 1.1f;
            CesiumGeoRef.height -= HeightChangeSpeed();
            if (CesiumGeoRef.height > 5000f)
            {
                foreach (GameObject tile in Tiles)
                {
                    tile.transform.localScale /= HeightChangeSpeed();
                }
            }
        }
        if (CesiumGeoRef.height > 500000f)
        {
            CesiumGeoRef.height = 500000f;
            foreach (GameObject tile in Tiles)
            {
                tile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
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

    private float HeightChangeSpeed()
    {
        float result = Mathf.Exp((float)CesiumGeoRef.height / 1000f);
        if (result > 5000f)
        {
            return 5000f;
        }
        return result;
    }

    private void InterpolateRotationToSurface()
    {
        Quaternion parallel = Quaternion.Euler(new Vector3(0, 0, 0));
        Quaternion perpendicular = Quaternion.Euler(new Vector3(0, 0, 90));
        float startThreshold = 1000f;
        float endThreshold = 5000f;
        if (CesiumGeoRef.height > startThreshold && CesiumGeoRef.height < endThreshold)
        {
            Debug.Log("Interpolating...");
            float t = Mathf.InverseLerp(startThreshold, endThreshold, (float)CesiumGeoRef.height);
            Vector3 euler = transform.rotation.eulerAngles;
            float zTilt = Mathf.Lerp(0f, 90f, t);
            transform.rotation = Quaternion.Euler(0f, euler.y, zTilt);
            //Quaternion rotation = Quaternion.Lerp(parallel, perpendicular, t);
            //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, rotation.z, transform.rotation.w);
            //transform.rotation = transform.rotation * rotation;
            //transform.Rotate(rotation.eulerAngles);
        }
    }

    /// Pour une quelconque raison, la caméra attachée au CAVE tombe d'elle même
    /// si le CAVE a un Rigidbody (peu importe si le Rigidbody est kinématique ou ignore la gravité).
    /// Cette méthode s'assure que la caméra reste à sa place.
    private void SaveCamera()
    {
        mainCam.transform.localPosition = initialCamPosition.position;
    }

}
