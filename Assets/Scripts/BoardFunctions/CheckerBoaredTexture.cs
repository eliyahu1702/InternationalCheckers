using UnityEngine;

public class CheckerBoaredTexture : MonoBehaviour
{

    public Texture2D mainTexture;

    public int mainTexWidth;
    public int mainTexHeight;
    // Start is called before the first frame update
    void Start()
    {
        SetTextureSize();
        CreatePattern();

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetTextureSize()
    {
        this.mainTexHeight = 10;
        this.mainTexWidth = 10;
        mainTexture = new Texture2D(mainTexWidth, mainTexHeight);
    }
    void CreatePattern()
    {
        for (int i = 0; i < mainTexWidth; i++)
        {
            for (int j = 0; j < mainTexHeight; j++)
            {
                if ((i + j) % 2 == 0)
                    mainTexture.SetPixel(i, j, new Color(139, 0, 0));
                else
                    mainTexture.SetPixel(i, j, Color.white);
            }

        }
        mainTexture.Apply();
        GetComponent<Renderer>().material.mainTexture = mainTexture;
        mainTexture.wrapMode = TextureWrapMode.Clamp;
        mainTexture.filterMode = FilterMode.Point;


    }
}
