using UnityEngine;

// Manages the sounds of the ball
public class BallFallSound : MonoBehaviour
{
    public AudioSource FallAudio;
    public AudioSource CollisionAudio;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Floor"))
        {
            Debug.Log("Floor");
            FallAudio.Play();
        }
        else if (other.CompareTag("GoldBall") || other.CompareTag("SilverBall"))
        {
            if (GetInstanceID() < other.GetInstanceID())
            {
                Debug.Log("Ball");
                CollisionAudio.Play();
            }
        }
    }
}
