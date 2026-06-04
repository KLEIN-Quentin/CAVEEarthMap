using UnityEngine;

// For grabbing objects in the CAVE
public class GrabObjectPetanque : MonoBehaviour
{
    [SerializeField] private Transform handPosition;

    [SerializeField] private GrabObjectPetanque otherHand;

    private bool canHold = false;

    private GameObject holdedObject = null;

    private NetworkGrabbable_Petanque networkGrabbable;

    private Quaternion currentRotation = Quaternion.identity;

    private Rigidbody rb;

    private WsClient_Petanque client;

    void Start()
    {
        client = FindFirstObjectByType<WsClient_Petanque>();
    }

    void Update()
    {
        // The holded object follows the transform of the hand
        if (holdedObject != null)
        {
            holdedObject.transform.position = handPosition.position;
            holdedObject.transform.rotation = handPosition.rotation * currentRotation; 
        }
    }

    // The hand can now grab an object, called when the player is pressing the trigger
    public void TryGrabObject()
    {
        canHold = true;
    }

    // The holded object is released
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

    // If the player is trying to grab an object and the triggered object is a ball, it is attached to the hand
    public void OnTriggerStay(Collider other)
    {
        if (canHold && (other.CompareTag("GoldBall") || other.CompareTag("SilverBall") || other.CompareTag("But")))
        {
            Debug.Log("test");
            other.GetComponent<HoverManager>().SetHover(false);
            networkGrabbable = other.GetComponent<NetworkGrabbable_Petanque>();
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

    // Displays the hover
    public void OnTriggerEnter(Collider other)
    {
        if (!canHold && (other.CompareTag("GoldBall") || other.CompareTag("SilverBall") || other.CompareTag("But")) && holdedObject == null)
        {
            other.GetComponent<HoverManager>().SetHover(true);
        }
    }

    // Hides the hover
    public void OnTriggerExit(Collider other)
    {
        if (!canHold && (other.CompareTag("GoldBall") || other.CompareTag("SilverBall") || other.CompareTag("But")) && holdedObject == null)
        {
            other.GetComponent<HoverManager>().SetHover(false);
        }
    }
}
