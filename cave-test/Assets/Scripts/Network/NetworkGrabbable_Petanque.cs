using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Manages the object that can be grabbed on the network
public class NetworkGrabbable_Petanque : MonoBehaviour
{
    public int id;

    public WsClient_Petanque client;

    private XRGrabInteractable grabInteractable;

    private Transform transform;

    private void Awake()
    {
        transform = GetComponent<Transform>();

        grabInteractable = GetComponent<XRGrabInteractable>();

        // Add listener events when someone interacts with the object
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        // Sends the object to the network
        client.AddBall(id, transform, grabInteractable);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        client.Grabbed(id);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        client.Released(id);
    }
}