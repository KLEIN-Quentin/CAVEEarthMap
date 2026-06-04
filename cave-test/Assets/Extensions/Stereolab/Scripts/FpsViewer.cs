using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsViewer : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textMeshProGUI;
    // Start is called before the first frame update

    private void Update()
    {
        int fps = (int)(1f / Time.unscaledDeltaTime);
        textMeshProGUI.text = "FPS: " + fps;
    }
}
