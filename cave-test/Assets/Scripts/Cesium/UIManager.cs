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

    private const string zeroWidthSpace = "\u200b";

    public void ChangeWMSUrl()
    {
        string url = InputUrl.text;
        string layers = InputLayer.text;
        Debug.Log("Changed URL to " + "\n" + url);
        Debug.Log("Changed layers to " + "\n" + layers);
        if (url.EndsWith(zeroWidthSpace))
        {
            url = url.Remove(url.Length - 1);
            Debug.Log("Removed garbo char in URL");
        }
        if (layers.EndsWith(zeroWidthSpace))
        {
            layers = layers.Remove(layers.Length - 1);
            Debug.Log("Removed garbo char in layers");
        }
        Debug.Log("Final URL to " + "\n" +  url);
        Debug.Log("Final layers to " + "\n" + layers);
        WMS.baseUrl = url;
        WMS.layers = layers;
    }
}