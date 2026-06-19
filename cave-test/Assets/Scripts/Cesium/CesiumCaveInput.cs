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
    //[SerializeField]
    //private CesiumGlobeAnchor anchor;
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
    private Rigidbody rb;
    private Vector3 startPos;
    private float orbitRadius = 0f;

    private bool isViewParallel = false;

    private void Awake()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        Debug.Assert(rb != null, "ASSERTION FAILED: A Rigidbody must be attached to this GameObject.");
        //rb.maxLinearVelocity = 20f;
        rb.maxAngularVelocity = 2f;
        heightThresholds.Sort();
        speedTable.Sort();
        Debug.Assert(heightThresholds.Count == speedTable.Count, "ASSERTION FAILED: heightThresholds and speedTable must have the same number of elements.");
        //anchor = GetComponent<CesiumGlobeAnchor>();
        //Debug.Assert(anchor != null, "ASSERTION FAILED: A Cesium Globe Anchor must be attached to this GameObject.");
        orbitRadius = Vector3.Distance(transform.position, globeApproximatorSphere.transform.position);
    }

    private void LateUpdate()
    {
        if (!freezeInputs)
        {
            ApplyMove();
            ApplyRotate();
            ApplyZoom();
        }
        //ApplyZoom();
        //ApplyElevate();
        //InterpolateRotationToSurface();
        InterpolateTileSize();
        SaveCamera();
        KeepFacingTowardsSurface();
        KeepInOrbit();
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
        moveInputs = new Vector2(0.2f, 0f);
        //Debug.Log("Movement vector magnitude: " + moveDirection.magnitude);
        moveDirection = transform.right * moveInputs.x + transform.up * moveInputs.y;
        float speed = RelativeSpeed();
        moveDirection *= speed;
        Vector3 finalMove = Vector3.ProjectOnPlane(moveDirection, (transform.position - globeApproximatorSphere.transform.position));
        foreach (GameObject tile in Tiles)
        {
            tile.transform.RotateAround(globeApproximatorSphere.transform.position, Vector3.up, moveInputs.x);
            tile.transform.RotateAround(globeApproximatorSphere.transform.position, Vector3.right, moveInputs.y);
        }
        //rb.AddForce(finalMove, ForceMode.VelocityChange);
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
                rb.AddTorque(Vector3.up * rotateRight, ForceMode.VelocityChange);
            }
        }
        else 
        {
            if (rotateRight <= 0.001f)
            {
                rb.AddTorque(Vector3.up * -rotateLeft, ForceMode.VelocityChange);
            }
            else
            {
                return;
            }    
        }
        
        //Vector3 torque = new Vector3(rotateInputs.y, rotateInputs.x, 0f);
        //Vector3 torque = transform.forward * rotateInputs.y + Vector3.up * rotateInputs.x;
        //torque *= 5f;
        //rb.AddTorque(torque, ForceMode.VelocityChange);
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
    
    private void ApplyElevate()
    {
        Vector3 elevation = new Vector3(0, elevateInputs.y, 0);
        float multiplier = ElevationSpeed();
        elevation *= multiplier;
        rb.AddForce(elevation, ForceMode.VelocityChange);
    }
    */
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

    private float ElevationSpeed()
    {
        return RelativeSpeed();
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

    private float HeightChangeSpeed()
    {
        float result = RelativeSpeed() / 10f;
        return result < 100f ? 100f : result;
    }

    private void InterpolateTileSize()
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = new Vector3(0.1f, 0.1f, 0.1f);
        if (CesiumGeoRef.height > startThreshold && CesiumGeoRef.height < endThreshold)
        {
            float t = Mathf.InverseLerp(startThreshold, endThreshold, (float)CesiumGeoRef.height);
            Vector3 newScale = Vector3.Lerp(startScale, endScale, t);
            foreach (GameObject tile in Tiles)
            {
                tile.transform.localScale = newScale;
            }
        }
    }
    /*
    private void InterpolateRotationToSurface()
    {
        Quaternion parallel = Quaternion.Euler(new Vector3(0, 0, 0));
        Quaternion perpendicular = Quaternion.Euler(new Vector3(0, 0, 90));
        if (CesiumGeoRef.height > startThreshold && CesiumGeoRef.height < endThreshold)
        {
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
    */
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

    private void KeepFacingTowardsSurface()
    {
        //Quaternion rotate = Quaternion.LookRotation(globeApproximatorSphere.transform.position - transform.position, transform.up);
        //rb.MoveRotation(rotate);
    }

    private void KeepInOrbit()
    {

    }
}
 