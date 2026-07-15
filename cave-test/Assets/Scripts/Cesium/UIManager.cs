using CesiumForUnity;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text InputUrl;

    [SerializeField]
    private TMP_Text InputLayer;

    [SerializeField]
    private CesiumWebMapServiceRasterOverlay WMS;

    public void ChangeWMSUrl()
    {
        WMS.baseUrl = InputUrl.text;
        WMS.layers = InputLayer.text;
    }
}