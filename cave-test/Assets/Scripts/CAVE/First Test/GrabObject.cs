using UnityEngine;

public class GrabObject : MonoBehaviour
{
    [SerializeField] private Transform handPosition;

    [SerializeField] private GrabObject otherHand;

    private bool canHold = false;

    private GameObject holdedObject = null;

    private Quaternion currentRotation = Quaternion.identity;

    private bool isScaling = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (holdedObject != null)
        {
            if (!isScaling)
            {
                holdedObject.transform.position = handPosition.position;
                holdedObject.transform.rotation = handPosition.rotation * currentRotation; 
            }
            else
            {
                Vector3 direction = holdedObject.transform.position - handPosition.position;
                float distance = direction.magnitude;
                
                Vector3 newScale = new Vector3(distance, distance, distance);
                holdedObject.transform.localScale = newScale;
            }
        }
    }

    public void TryGrabObject()
    {
        canHold = true;
    }

    public void ReleaseObject()
    {
        holdedObject = null;
        canHold = false;
        isScaling = false;
    }

    public void OnTriggerStay(Collider other)
    {
        if (canHold && other.CompareTag("Grabbable"))
        {
            canHold = false;
            holdedObject = other.gameObject;
            currentRotation = Quaternion.identity;
            if (otherHand.holdedObject == holdedObject)
            {
                isScaling = true;
            }
        }
    }

    public void RotateObject(Vector2 rotation)
    {
        if (holdedObject != null)
        {
            Quaternion rotationX = Quaternion.AngleAxis(rotation.x * 3f, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(rotation.y * 3f, Vector3.up);

            currentRotation = rotationY * rotationX * currentRotation; 
        }
    }
}
