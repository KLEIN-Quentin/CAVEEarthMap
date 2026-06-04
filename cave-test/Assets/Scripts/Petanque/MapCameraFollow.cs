using UnityEngine;

// Used to keep the camera following the but
public class MapCameraFollow : MonoBehaviour
{
    public GameObject Camera;
    public GameObject Ball;

    void Update()
    {
        Camera.transform.eulerAngles = new Vector3(90, 0, 0);
        Camera.transform.position = new Vector3(Ball.transform.position.x, Ball.transform.position.y + 1, Ball.transform.position.z);
    }
}
