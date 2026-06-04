using UnityEngine;

public class HandFollow : MonoBehaviour
{
    public Transform handTarget;
    public Transform vrhand;

    void Update()
    {
        handTarget.position = vrhand.position;
        handTarget.rotation = vrhand.rotation;
    }
}
