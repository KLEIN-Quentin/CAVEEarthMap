using UnityEngine;

public class ObjectTeleportation : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = new Vector3(-2f, 1f, -5.5f);
    }
}
