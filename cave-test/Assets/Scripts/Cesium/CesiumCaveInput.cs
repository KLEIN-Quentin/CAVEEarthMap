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

    [Header("Internals")]
    private Vector2 moveInputs = Vector2.zero;
    private float zoomUp = 0f;
    private float zoomDown = 0f;
    private float currentScale = 1f;


    private void Awake()
    {

    }

    private void LateUpdate()
    {
        ApplyMove();
        ApplyZoom();
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
        CesiumGeoRef.longitude += moveInputs.x / ratio;
        CesiumGeoRef.latitude += moveInputs.y / ratio;
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
    
    private void ApplyZoom()
    {
        if (zoomUp > 0.001f) 
        {
            foreach (GameObject tile in Tiles)
            {
                tile.transform.localScale *= 1.1f;
            }
            globeApproximator.transform.localScale *= 1.1f;
            currentScale += 1f;
        }
        if (zoomDown > 0.001f)
        {
            foreach (GameObject tile in Tiles)
            {
                tile.transform.localScale /= 1.1f;
            }
            globeApproximator.transform.localScale /= 1.1f;
            currentScale -= 1f;
        }
    }

    private float RelativeSpeed()
    {
        if (currentScale <= 1f)
        {
            return 1f;
        }
        float result = Mathf.Pow(currentScale, 2f) / 2f;
        return Mathf.Lerp(1f, 10000f, Mathf.InverseLerp(1f, 10000f, result));
    }

    /// Pour une quelconque raison, la caméra attachée au CAVE tombe d'elle même
    /// si le CAVE a un Rigidbody (peu importe si le Rigidbody est kinématique ou ignore la gravité).
    /// Cette méthode s'assure que la caméra reste à sa place.
    private void SaveCamera()
    {
        mainCam.transform.localPosition = initialCamPosition.position;
    }
}
 