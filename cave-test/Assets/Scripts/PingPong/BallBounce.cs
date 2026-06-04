using UnityEngine;

public class BallBounce : MonoBehaviour
{
    private AudioSource audio;

    private WsClient_Petanque client;

    private void Awake() 
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start() 
    {
        client = FindFirstObjectByType<WsClient_Petanque>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        // if (!client.isTheBallGrabbed())
        // {
        //     audio.Play();
        // }
    }
}
