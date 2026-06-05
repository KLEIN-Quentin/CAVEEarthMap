using UnityEngine;
using UnityEngine.InputSystem;

public class CesiumCaveInput : MonoBehaviour
{
    [SerializeField]
    private GameObject CAVE;

    private Vector2 moveInputs = Vector2.zero;

    private Vector2 rotateInputs = Vector2.zero;

    private void Update()
    {
        ApplyMove();
        ApplyRotate();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputs = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        rotateInputs = context.ReadValue<Vector2>();
    }

    private void ApplyMove()
    {
        CAVE.transform.localPosition += new Vector3(moveInputs.x, 0, moveInputs.y);
        moveInputs = Vector2.zero;
    }

    private void ApplyRotate()
    {
        CAVE.transform.Rotate(new Vector3(rotateInputs.y, rotateInputs.x, 0));
        rotateInputs = Vector2.zero;
        CAVE.transform.Rotate(Vector3.zero);
    }

}
