using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections.Generic;

public class CharacterSelector : MonoBehaviour
{
    public GameObject ServerSelectCanva;
    public GameObject CharacterCanva;

    public Material BlackHandMaterial;
    public Material WhiteHandMaterial;
    public Material LightBrownHandMaterial;

    public SkinnedMeshRenderer LeftHand;
    public SkinnedMeshRenderer RightHand;

    public ClientConnectionHandler ConnectionHandler;

    public Button BlackColorButton;
    public Button LightBrownColorButton;
    public Button WhiteColorButton;

    public Button MaleFaceButton;
    public Button FemaleFaceButton;

    public Button ShortHairButton;
    public Button LongHairButton;
    public Button NoHairButton;

    private ColorBlock greenColorBlock;
    private ColorBlock whiteColorBlock;

    private int color;
    private int face;
    private int hair;

    private void Start()
    {
        greenColorBlock = BlackColorButton.colors;
        greenColorBlock.normalColor = Color.green;
        greenColorBlock = BlackColorButton.colors;
        greenColorBlock.normalColor = Color.white;
        BlackColorChosed();
        MaleFaceChosed();
        ShortHairChosed();
    }

    public void BlackColorChosed()
    {
        color = 0;
        BlackColorButton.colors = greenColorBlock;
        LightBrownColorButton.colors = whiteColorBlock;
        WhiteColorButton.colors = whiteColorBlock;
    }

    public void LightBrownColorChosed()
    {
        color = 1;
        BlackColorButton.colors = whiteColorBlock;
        LightBrownColorButton.colors = greenColorBlock;
        WhiteColorButton.colors = whiteColorBlock;
    }

    public void WhiteColorChosed()
    {
        color = 2;
        BlackColorButton.colors = whiteColorBlock;
        LightBrownColorButton.colors = whiteColorBlock;
        WhiteColorButton.colors = greenColorBlock;
    }

    public void MaleFaceChosed()
    {
        face = 0;
        MaleFaceButton.colors = greenColorBlock;
        FemaleFaceButton.colors = whiteColorBlock;
    }

    public void FemaleFaceChosed()
    {
        face = 1;
        MaleFaceButton.colors = whiteColorBlock;
        FemaleFaceButton.colors = greenColorBlock;
    }

    public void ShortHairChosed()
    {
        hair = 0;
        ShortHairButton.colors = greenColorBlock;
        LongHairButton.colors = whiteColorBlock;
        NoHairButton.colors = whiteColorBlock;
    }

    public void LongHairColorChosed()
    {
        hair = 1;
        ShortHairButton.colors = whiteColorBlock;
        LongHairButton.colors = greenColorBlock;
        NoHairButton.colors = whiteColorBlock;
    }

    public void NoHairChosed()
    {
        hair = 2;
        ShortHairButton.colors = whiteColorBlock;
        LongHairButton.colors = whiteColorBlock;
        NoHairButton.colors = greenColorBlock;
    }

    public void OnSelectButtonClicked()
    {
        ConnectionHandler.SetClientPlayerPrefab(6 * color + 3 * face + hair);
        CharacterCanva.SetActive(false);
        ServerSelectCanva.SetActive(true);
        List<Material> mat = new List<Material>();
        if (color == 0)
        {
            mat.Add(BlackHandMaterial);
        }
        else if (color == 1)
        {
            mat.Add(LightBrownHandMaterial);
        }
        else
        {
            mat.Add(WhiteHandMaterial);
        }
        LeftHand.SetMaterials(mat);
        RightHand.SetMaterials(mat);
    }
}
