using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public GameObject Player;

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            Vector3 newPos = new Vector3(Player.transform.position.x, other.transform.position.y, Player.transform.position.z);
            Player.transform.position = newPos;
        }
    }
}
 