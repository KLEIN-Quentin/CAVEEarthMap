using UnityEngine;

public class GrabObjectTransport : MonoBehaviour
{
    [SerializeField] private Transform handPosition;

    [SerializeField] private GrabObjectTransport otherHand;

    private bool canHold = false;

    private GameObject holdedObject = null;

    private NetworkGrabbable_Transport networkGrabbable;

    private Quaternion currentRotation = Quaternion.identity;

    private Rigidbody rb;

    private WsClient_Transport client;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        client = FindFirstObjectByType<WsClient_Transport>();
    }

    // Update is called once per frame
    void Update()
    {
        if (holdedObject != null)
        {
            holdedObject.transform.position = handPosition.position;
            holdedObject.transform.rotation = handPosition.rotation * currentRotation; 
        }
    }

    public void TryGrabObject()
    {
        canHold = true;
    }

    public void ReleaseObject()
    {
        if (holdedObject != null)
        {
            client.Released(networkGrabbable.id);
        }
        holdedObject = null;
        networkGrabbable = null;
        canHold = false;
    }

    public void OnTriggerStay(Collider other)
    {
        if (canHold && other.CompareTag("Rope"))
        {
            other.GetComponent<HoverManager>().SetHover(false);
            networkGrabbable = other.GetComponent<NetworkGrabbable_Transport>();
            if (other.gameObject == otherHand.holdedObject)
            {
                otherHand.ReleaseObject();
            }
            canHold = false;
            holdedObject = other.gameObject;
            currentRotation = Quaternion.identity;
            client.Grabbed(networkGrabbable.id);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!canHold && other.CompareTag("Rope") && holdedObject == null)
        {
            other.GetComponent<HoverManager>().SetHover(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!canHold && other.CompareTag("Rope") && holdedObject == null)
        {
            other.GetComponent<HoverManager>().SetHover(false);
        }
    }
}
