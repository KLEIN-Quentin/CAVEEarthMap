// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;

// public class PingPongBallController : MonoBehaviour
// {
//     public float airResistance = 0.05f;
//     private Rigidbody rb;
//     private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
//     private Vector3 lastPosition;
//     private Vector3 velocity;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//         grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

//         if (grabInteractable != null)
//         {
//             grabInteractable.selectExited.AddListener(OnRelease);
//         }
//     }

//     void FixedUpdate()
//     {
//         ApplyAirResistance();

//         if (grabInteractable != null && grabInteractable.isSelected)
//         {
//             velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
//             lastPosition = transform.position;
//         }
//     }

//     void ApplyAirResistance()
//     {
//         rb.AddForce(-rb.linearVelocity * airResistance, ForceMode.Acceleration);
//     }

//     void OnRelease(SelectExitEventArgs args)
//     {
//         rb.isKinematic = false; // S’assurer que la physique est active
//         rb.linearVelocity = velocity * 2f + Vector3.up * 0.7f; // Boost plus fort
//     }
// }

using UnityEngine;

public class PingPongBallController : MonoBehaviour
{
    public float airResistance = 0.05f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ApplyAirResistance();
    }

    void ApplyAirResistance()
    {
        rb.AddForce(-rb.linearVelocity * airResistance, ForceMode.Acceleration);
    }
}