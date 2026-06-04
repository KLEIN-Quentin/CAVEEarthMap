using UnityEngine;

public class HoverManager : MonoBehaviour
{
    public GameObject Hover;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Hover.SetActive(false);
    }

    public void SetHover(bool state)
    {
        Hover.SetActive(state);
    }
}
