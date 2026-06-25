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
    private GameObject leftLaserPointer;
    [SerializeField]
    private GameObject rightLaserPointer;

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


    [Header("Curves")]
    [SerializeField]
    private AnimationCurve moveSpeedCurve;
    [SerializeField]
    private AnimationCurve zoomSpeedCurve;
    [SerializeField]
    private AnimationCurve accelerationCurve;
    [SerializeField]
    private AnimationCurve decelerationCurve;

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


    private void Awake()
    {

    }

    private void LateUpdate()
    {
        ApplyMove();
        ApplyZoom();
        Accelerate();
        ApplyVelocities();
        Debug.Log("Current moveInputs: " + moveInputs);
        Debug.Log("Current target velocities: +long=" + targetPosLongVel
                    + " -long=" + targetNegLongVel + " +lat=" + targetPosLatVel + " -lat=" + targetNegLatVel);
        Debug.Log("Current velocities: +long=" + posLongVel + " -long=" + negLongVel + " +lat=" + posLatVel + " -lat=" + negLatVel);
        Decelerate();
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

    public void LeftLaserPoint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            leftLaserPointer.SetActive(true);
        }
        if (context.canceled)
        {
            leftLaserPointer.SetActive(false);
        }
    }

    public void RightLaserPoint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rightLaserPointer.SetActive(true);
        }
        if (context.canceled)
        {
            rightLaserPointer.SetActive(false);
        }
    }

    private void ApplyMove()
    {
        if (moveInputs == Vector2.zero)
        {
            Debug.Log("moveInputs are 0, not applying move");
            return;
        }
        float speed = GetMoveSpeed();
        Debug.Log("Current speed: " + speed);
        float maxLongSpeed = moveInputs.x * speed;
        float maxLatSpeed = moveInputs.y * speed;

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
        float zoomSpeed = GetZoomSpeed();
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
            currentScale -= 1f;
        }
    }

    private void Accelerate()
    {
        float accel = GetAcceleration();
        posLongVel += accel;
        posLatVel += accel;
        negLongVel -= accel;
        negLatVel -= accel;
        
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

    private void Decelerate()
    {
        float decel = GetDeceleration();
        if (targetPosLongVel > 0)
        {
            targetPosLongVel -= decel;
            if (Mathf.Abs(targetPosLongVel) < decel)
            {
                targetPosLongVel = 0;
            }
        }
        if (targetNegLongVel < 0)
        {
            targetNegLongVel += decel;
            if (Mathf.Abs(targetNegLongVel) < decel)
            {
                targetNegLongVel = 0;
            }
        }
        if (targetPosLatVel > 0)
        {
            targetPosLatVel -= decel;
            if (Mathf.Abs(targetPosLatVel) < decel)
            {
                targetPosLatVel = 0;
            }
        }
        if (targetNegLatVel < 0)
        {
            targetNegLatVel += decel;
            if (Mathf.Abs(targetNegLatVel) < decel)
            {
                targetNegLatVel = 0;
            }
        }
    }

    private float GetMoveSpeed()
    {
        /*
        if (currentScale <= 1f)
        {
            return 2f;
        }
        float result = Mathf.Pow(currentScale, 2f) / 2f;
        return Mathf.Lerp(2f, 10000f, Mathf.InverseLerp(2f, 10000f, result));
        */
        return moveSpeedCurve.Evaluate(currentScale);
    }

    private float GetZoomSpeed()
    {
        /*
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
        */
        return zoomSpeedCurve.Evaluate(currentScale);
    }

    private float GetAcceleration()
    {
        return accelerationCurve.Evaluate(currentScale);
    }

    private float GetDeceleration()
    {
        return decelerationCurve.Evaluate(currentScale);
    }

    /// Pour une quelconque raison, la caméra attachée au CAVE tombe d'elle même
    /// si le CAVE a un Rigidbody (peu importe si le Rigidbody est kinématique ou ignore la gravité).
    /// Cette méthode s'assure que la caméra reste à sa place.
    private void SaveCamera()
    {
        mainCam.transform.localPosition = initialCamPosition.position;
    }
}
 