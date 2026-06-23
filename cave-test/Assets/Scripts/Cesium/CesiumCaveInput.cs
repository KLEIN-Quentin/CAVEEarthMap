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

    [Header("Tracking")]
    [SerializeField]
    private Transform leftHand;
    [SerializeField]
    private GameObject mainCam;
    [SerializeField]
    private Transform initialCamPosition;

    [Header("Zoom Controls")]
    [SerializeField]
    private float minInternalScale = -25f;
    [SerializeField]
    private float maxInternalScale = 160f;


    [Header("Internals")]
    private Vector2 moveInputs = Vector2.zero;
    private float zoomUp = 0f;
    private float zoomDown = 0f;
    private float currentScale = 1f;
    private float posLongitudeVelocity = 0f;
    private float posLatitudeVelocity = 0f;
    private float negLongitudeVelocity = 0f;
    private float negLatitudeVelocity = 0f;
    private float acceleration = 0.05f;
    private float deceleration = 0.05f;
    private Rigidbody rb;


    private void Awake()
    {

    }

    private void LateUpdate()
    {
        ApplyMove();
        ApplyZoom();
        ApplyVelocities();
        DecayVelocities();
        SaveCamera();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputs = context.ReadValue<Vector2>();
    }

    public void OnScaleUp(InputAction.CallbackContext context)
    {
        zoomUp = context.ReadValue<float>();
    }

    public void OnScaleDown(InputAction.CallbackContext context)
    {
        zoomDown = context.ReadValue<float>();
    }

    private void ApplyMove()
    {
        float ratio = RelativeSpeed();
        float maxLongSpeed = moveInputs.x / ratio;
        float maxLatSpeed = moveInputs.y / ratio;

        if (maxLongSpeed < 0f)
        {
            negLongitudeVelocity = maxLongSpeed;
        }
        if (maxLongSpeed > 0f)
        {
            posLongitudeVelocity = maxLongSpeed;
        }
        
        if (maxLatSpeed < 0f)
        {
            negLatitudeVelocity = maxLatSpeed;
        }
        if (maxLatSpeed > 0f)
        {
            posLatitudeVelocity = maxLatSpeed;
        }
    }
    
    private void ApplyZoom()
    {
        float zoomSpeed = ZoomSpeed();
        if (zoomUp > 0.001f) 
        {
            if (currentScale >= maxInternalScale)
            {
                return;
            }
            foreach (GameObject tile in Tiles)
            {
                tile.transform.localScale *= zoomSpeed;
            }
            globeApproximator.transform.localScale *= zoomSpeed;
            currentScale += 1f;
        }
        if (zoomDown > 0.001f)
        {
            if (currentScale <= minInternalScale)
            {
                return;
            }
            foreach (GameObject tile in Tiles)
            {
                tile.transform.localScale /= zoomSpeed;
            }
            globeApproximator.transform.localScale /= zoomSpeed;
            currentScale -= 1f;
        }
    }

    private void ApplyVelocities()
    {
        CesiumGeoRef.longitude += posLongitudeVelocity + negLongitudeVelocity;
        CesiumGeoRef.latitude += posLatitudeVelocity + negLatitudeVelocity;
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
    }

    private void DecayVelocities()
    {
        if (posLongitudeVelocity > 0)
        {
            posLongitudeVelocity -= deceleration;
            if (Mathf.Abs(posLongitudeVelocity) < deceleration)
            {
                posLongitudeVelocity = 0;
            }
        }
        if (negLongitudeVelocity < 0)
        {
            negLongitudeVelocity += deceleration;
            if (Mathf.Abs(negLongitudeVelocity) < deceleration)
            {
                negLongitudeVelocity = 0;
            }
        }
        if (posLatitudeVelocity > 0)
        {
            posLatitudeVelocity -= deceleration;
            if (Mathf.Abs(posLatitudeVelocity) < deceleration)
            {
                posLatitudeVelocity = 0;
            }
        }
        if (negLatitudeVelocity < 0)
        {
            negLatitudeVelocity += deceleration;
            if (Mathf.Abs(negLatitudeVelocity) < deceleration)
            {
                negLatitudeVelocity = 0;
            }
        }
    }

    private float RelativeSpeed()
    {
        if (currentScale <= 1f)
        {
            return 2f;
        }
        float result = Mathf.Pow(currentScale, 2f) / 2f;
        return Mathf.Lerp(2f, 10000f, Mathf.InverseLerp(2f, 10000f, result));
    }

    private float ZoomSpeed()
    {
        if (currentScale <= 20f)
        {
            return 1.1f;
        }
        else if (currentScale <= 60f)
        {
            return 1.05f;
        }
        else
        {
            return 1.01f;
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
 