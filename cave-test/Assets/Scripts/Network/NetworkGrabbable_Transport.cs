using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NetworkGrabbable_Transport : MonoBehaviour
{
    public int id;

    public WsClient_Transport client;

    public XRGrabInteractable grabInteractable;

    public Transform sphereTransform;
    public Transform minisphereTransform1;
    public Transform minisphereTransform2;
    public Transform minisphereTransform3;
    public Transform minisphereTransform4;
    public Transform minisphereTransform5;
    public Transform minisphereTransform6;
    public Transform minisphereTransform7;

    private void Awake()
    {
        // Écouter les événements de saisie et de relâchement
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        client.AddRope(id, sphereTransform, minisphereTransform1, minisphereTransform2, minisphereTransform3, minisphereTransform4, minisphereTransform5, minisphereTransform6, minisphereTransform7, grabInteractable);
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