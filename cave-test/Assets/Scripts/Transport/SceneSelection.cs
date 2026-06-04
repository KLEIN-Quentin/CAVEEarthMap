using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelection : MonoBehaviour
{

    public void LoadTwoRopesScene()
    {
        SceneManager.LoadScene("TransportTwoRopes_0");
    }

    public void LoadFourRopesScene()
    {
        SceneManager.LoadScene("Transport_0");
    }
}
