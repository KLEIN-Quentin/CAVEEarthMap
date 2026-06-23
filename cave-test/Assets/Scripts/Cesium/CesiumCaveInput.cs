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
    private float targetPosLongVel = 0f;
    private float targetPosLatVel = 0f;
    private float targetNegLongVel = 0f;
    private float targetNegLatVel = 0f;
    private float posLongVel = 0f;
    private float posLatVel = 0f;
    private float negLongVel = 0f;
    private float negLatVel = 0f;
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
        Accelerate();
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
            targetNegLongVel = maxLongSpeed;
        }
        if (maxLongSpeed > 0f)
        {
            targetPosLongVel = maxLongSpeed;
        }
        
        if (maxLatSpeed < 0f)
        {
            targetNegLatVel = maxLatSpeed;
        }
        if (maxLatSpeed > 0f)
        {
            targetPosLatVel = maxLatSpeed;
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

    private void Accelerate()
    {
        posLongVel += acceleration;
        posLatVel += acceleration;
        negLongVel -= acceleration;
        negLatVel -= acceleration;
        
        if (posLongVel >= targetPosLongVel)
        {
            posLongVel = targetPosLongVel;
        }
        if (posLatVel >= targetPosLatVel)
        {
            posLatVel = targetPosLatVel;
        }
        if (negLongVel <= targetNegLongVel)
        {
            negLongVel = targetNegLongVel;
        }
        if (negLatVel <= targetNegLatVel)
        {
            negLatVel = targetNegLatVel;
        }
    }

    private void ApplyVelocities()
    {
        CesiumGeoRef.longitude += posLongVel + negLongVel;
        CesiumGeoRef.latitude += posLatVel + negLatVel;
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
        if (targetPosLongVel > 0)
        {
            targetPosLongVel -= deceleration;
            if (Mathf.Abs(targetPosLongVel) < deceleration)
            {
                targetPosLongVel = 0;
            }
        }
        if (targetNegLongVel < 0)
        {
            targetNegLongVel += deceleration;
            if (Mathf.Abs(targetNegLongVel) < deceleration)
            {
                targetNegLongVel = 0;
            }
        }
        if (targetPosLatVel > 0)
        {
            targetPosLatVel -= deceleration;
            if (Mathf.Abs(targetPosLatVel) < deceleration)
            {
                targetPosLatVel = 0;
            }
        }
        if (targetNegLatVel < 0)
        {
            targetNegLatVel += deceleration;
            if (Mathf.Abs(targetNegLatVel) < deceleration)
            {
                targetNegLatVel = 0;
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
 