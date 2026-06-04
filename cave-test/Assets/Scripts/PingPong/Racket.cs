using UnityEngine;

public class Racket : MonoBehaviour
{
    public float strengthMultiplier = 0.002f;
    private Vector3 lastPosition;
    private Vector3 velocity;

    void FixedUpdate()
    {
        // Calculer la vitesse du mouvement de la raquette
        velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();

            Vector3 hitDirection = velocity.normalized; 
            float hitStrength = Mathf.Clamp(velocity.magnitude * strengthMultiplier, 0, 0.04f);

            ballRb.AddForce(hitDirection * hitStrength, ForceMode.Impulse);
        }
    }
}

// using UnityEngine;

// public class Racket : MonoBehaviour
// {
//     public float strength = 0.001f;
    
//     void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("Ball"))
//         {
//             Debug.Log("Ball touched");
//             Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
//             Vector3 hitDirection = transform.forward + Vector3.up; // Direction du coup
//             ballRb.AddForce(hitDirection * strength, ForceMode.Impulse);
//         }
//     }
// }
