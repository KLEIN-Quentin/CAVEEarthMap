using UnityEngine;

public class ShadowFollow : MonoBehaviour
{
    public Transform HeadTransform;
    public Transform ShadowTransform;

    // Update is called once per frame
    void Update()
    {
        ShadowTransform.position = new Vector3(HeadTransform.position.x, ShadowTransform.position.y, HeadTransform.position.z);
    }
}
