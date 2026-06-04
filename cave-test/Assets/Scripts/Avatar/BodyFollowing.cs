using UnityEngine;

public class BodyFollowing : MonoBehaviour
{
    public Transform BodyTransform;
    public Transform HeadTransform;
    public Transform CameraTransform;

    private float gap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gap = HeadTransform.position.y - BodyTransform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        HeadTransform.rotation = CameraTransform.rotation;
        HeadTransform.position = new Vector3(CameraTransform.position.x, CameraTransform.position.y, CameraTransform.position.z);
        HeadTransform.localPosition = new Vector3(HeadTransform.localPosition.x, HeadTransform.localPosition.y, HeadTransform.localPosition.z - 0.075f);
        BodyTransform.position = new Vector3(HeadTransform.position.x, HeadTransform.position.y - gap + 0.005f, HeadTransform.position.z);
        BodyTransform.eulerAngles = new Vector3(BodyTransform.eulerAngles.x, CameraTransform.eulerAngles.y + 90, BodyTransform.eulerAngles.z);
    }
}
