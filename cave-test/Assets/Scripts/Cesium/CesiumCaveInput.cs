using CesiumForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CesiumCaveInput : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField]
    private GameObject CAVE;
    [SerializeField]
    private CesiumGeoreference CesiumGeoRef;
    [SerializeField]
    private GameObject[] Tiles;
    [SerializeField]
    private GameObject globeApproximator;
    [SerializeField]
    private GameObject globeApproximatorSphere;


    [Header("Settings")]
    [SerializeField]
    private bool allowViewToggle = false;



    [Header("Tracking")]
    [SerializeField]
    private Transform leftHand;

    [SerializeField]
    private GameObject mainCam;
    [SerializeField]
    private Transform initialCamPosition;

    [Header("Interpolation")]
    [SerializeField]
    private float startThreshold = 1000f;
    [SerializeField]
    private float endThreshold = 5000f;
    [SerializeField]
    private float interpolationSeconds = 3f;
    private bool freezeInputs = false;

    [Header("Speed calculations")]
    [SerializeField]
    private float downRaycastMaxHeight = 800f;
    [SerializeField]
    private List<float> heightThresholds;
    [SerializeField]
    private List<float> speedTable;

    [Header("Internals")]
    private Vector2 moveInputs = Vector2.zero;
    private Vector2 rotateInputs = Vector2.zero;
    private float rotateLeft = 0f;
    private float rotateRight = 0f;
    private Vector2 elevateInputs = Vector2.zero;
    private float zoomUp = 0f;
    private float zoomDown = 0f;

    private bool isViewParallel = false;

    private void Awake()
    {
        heightThresholds.Sort();
        //speedTable.Sort();
        Debug.Assert(heightThresholds.Count == speedTable.Count, "ASSERTION FAILED: heightThresholds and speedTable must have the same number of elements.");
    }

    private void LateUpdate()
    {
        if (!freezeInputs)
        {
            ApplyMove();
            ApplyRotate();
            ApplyZoom();
        }
        SaveCamera();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputs = context.ReadValue<Vector2>();
    }
    
    
    public void OnRotate(InputAction.CallbackContext context)
    {
        rotateInputs = context.ReadValue<Vector2>();
    }
    
    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        rotateLeft = context.ReadValue<float>();
    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {
        rotateRight = context.ReadValue<float>();
    }
    
    public void OnElevate(InputAction.CallbackContext context)
    {
        elevateInputs = context.ReadValue<Vector2>();
    }

    public void OnScaleUp(InputAction.CallbackContext context)
    {
        zoomUp = context.ReadValue<float>();
    }

    public void OnScaleDown(InputAction.CallbackContext context)
    {
        zoomDown = context.ReadValue<float>();
    }

    public void ToggleViewMode(InputAction.CallbackContext context)
    {
        if (!allowViewToggle)
        {
            Debug.Log("View toggling has been turned off.");
            return;
        }
        if (context.performed && !freezeInputs)
        {
            isViewParallel = !isViewParallel;
            freezeInputs = true;
            StartCoroutine(InterpolateRotationToSurface());
        }
    }

    private void ApplyMove()
    {
        //CAVE.transform.localPosition += new Vector3(moveInputs.x / 2, 0, moveInputs.y / 2);
        //moveInputs = Vector2.zero;
        
        Vector3 moveDirection = Vector3.zero;
        //Debug.Log("Movement vector magnitude: " + moveDirection.magnitude);
        moveDirection = transform.right * moveInputs.x + transform.up * moveInputs.y;
        float speed = RelativeSpeed();
        moveDirection *= speed;
        Vector3 finalMove = Vector3.ProjectOnPlane(moveDirection, (transform.position - globeApproximatorSphere.transform.position));
        foreach (GameObject tile in Tiles)
        {
            //tile.transform.RotateAround(globeApproximatorSphere.transform.position, Vector3.up, moveInputs.x);
            //tile.transform.RotateAround(globeApproximatorSphere.transform.position, Vector3.right, moveInputs.y);
        }
        CesiumGeoRef.longitude += moveInputs.x / 2;
        CesiumGeoRef.latitude += moveInputs.y / 2;
        if (CesiumGeoRef.longitude <= -180)
        {
            CesiumGeoRef.longitude = 180;
            return;
        }
        if (CesiumGeoRef.longitude >= 180)
        {
            CesiumGeoRef.longitude = -180;
            return;
        }
        //rb.AddForce(finalMove, ForceMode.VelocityChange);
    }

    private void ApplyRotate()
    {
        
    }
    
    private void ApplyZoom()
    {
        if (zoomUp > 0.001f) 
        {
            //CAVE.transform.localScale *= 1.1f;
            //CesiumGeoRef.height += HeightChangeSpeed();
            //anchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(anchor.longitudeLatitudeHeight.x, anchor.longitudeLatitudeHeight.y, anchor.longitudeLatitudeHeight.z + HeightChangeSpeed());
            Debug.Log("Zooming up");
            foreach (GameObject tile in Tiles)
            {
                Debug.Log("Old tile scale: " + tile.transform.localScale);
                tile.transform.localScale *= 1.1f;
                Debug.Log("New tile scale: " + tile.transform.localScale);
            }
            globeApproximator.transform.localScale *= 1.1f;
        }
        if (zoomDown > 0.001f)
        {
            //CAVE.transform.localScale /= 1.1f;
            //CesiumGeoRef.height -= HeightChangeSpeed();
            //anchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(anchor.longitudeLatitudeHeight.x, anchor.longitudeLatitudeHeight.y, anchor.longitudeLatitudeHeight.z - HeightChangeSpeed());
            Debug.Log("Zooming down");
            foreach (GameObject tile in Tiles)
            {
                Debug.Log("Old tile scale: " + tile.transform.localScale);
                tile.transform.localScale /= 1.1f;
                Debug.Log("New tile scale: " + tile.transform.localScale);
            }
            globeApproximator.transform.localScale /= 1.1f;
        }
        if (CesiumGeoRef.height > 10000000f)
        {
            //CesiumGeoRef.height = 10000000f;
            //anchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(anchor.longitudeLatitudeHeight.x, anchor.longitudeLatitudeHeight.y, 10000000f);

        }
    }

    private float RelativeSpeed()
    {
        //return 40 * (Mathf.Abs(transform.position.y) + (float)CesiumGeoRef.height);
        float height = (float)CesiumGeoRef.height;
        float speed = 1f;
        for (int i = 0; i < speedTable.Count; i++)
        {
            if (height < heightThresholds[0])
            {
                speed = speedTable[0];
                break;
            }
            if (height > heightThresholds[heightThresholds.Count - 1] || (i + 1) > speedTable.Count)
            {
                speed = speedTable[speedTable.Count - 1];
                break;
            }
            if (heightThresholds[i] < height && height < heightThresholds[i + 1])
            {
                speed = speedTable[i];
            }
        }
        //Debug.Log("Current speed: " + speed);
        return speed;
    }

    private IEnumerator InterpolateRotationToSurface()
    {
        float timeElapsed = 0f;
        if (isViewParallel)
        {
            Debug.Log("Supposed to interpolate from Perpendicular to Parallel view");
            while (timeElapsed < interpolationSeconds)
            {
                float t = timeElapsed / interpolationSeconds;
                Vector3 euler = transform.rotation.eulerAngles;
                float zTilt = Mathf.Lerp(90f, 0f, t);
                transform.rotation = Quaternion.Euler(0f, euler.y, zTilt);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            freezeInputs = false;
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

            
        }
        else
        {
            Debug.Log("Supposed to interpolate from Parallel to Perpendicular view");
            while (timeElapsed < interpolationSeconds)
            {
                float t = timeElapsed / interpolationSeconds;
                Vector3 euler = transform.rotation.eulerAngles;
                float zTilt = Mathf.Lerp(0f, 90f, t);
                transform.rotation = Quaternion.Euler(0f, euler.y, zTilt);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            freezeInputs = false;
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 90f);

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
 