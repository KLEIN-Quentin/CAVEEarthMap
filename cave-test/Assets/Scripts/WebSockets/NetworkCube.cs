using UnityEngine;

public class NetworkCube : MonoBehaviour
{
    public int id;
    public WsClient_Transport client;

    private Transform transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        transform = GetComponent<Transform>();
        client.AddCube(id, transform);
        Debug.Log(id);
    }
}
