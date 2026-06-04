using UnityEngine;

public class AvatarTeleport : MonoBehaviour
{
    public Transform AvatarPosition;

    public void Teleport()
    {
        AvatarPosition.position = new Vector3(1,0,-5);
    }
}
