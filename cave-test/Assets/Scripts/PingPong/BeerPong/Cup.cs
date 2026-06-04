using UnityEngine;

public class Cup : MonoBehaviour
{
    private static int val = 0;
    private int id;

    private WsClient_Petanque client;

    private Transform transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        id = val;
        val++;

        transform = GetComponent<Transform>();

        client = FindFirstObjectByType<WsClient_Petanque>();
    }

    private void OnTriggerEnter(Collider other) {
        // if (other.tag == "Ball" && !client.isTheBallGrabbed())
        // {
        //     client.RemoveCup(id);
        //     this.gameObject.SetActive(false);
        // }
    }
}
