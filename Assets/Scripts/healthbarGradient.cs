using UnityEngine;
using UnityEngine.UI;

public class healthBarGradient : MonoBehaviour
{
    public Image healthBarImage;

    void Start()
    {
        int width = 256;
        int height = 1;

        Texture2D gradientTex = new Texture2D(width, height);
        gradientTex.wrapMode = TextureWrapMode.Clamp;

        Color color1 = Color.red;
        Color color2 = new Color(1f, 0.5f, 0f);
        Color color3 = Color.yellow;
        Color color4 = Color.green;

        for (int x = 0; x < width; x++)
        {
            float t = x / (float)(width - 1);
            Color col;

            if (t < 0.33f)
            {
                col = Color.Lerp(color1, color2, t / 0.33f);
            }
            else if (t < 0.66f)
            {
                col = Color.Lerp(color2, color3, (t - 0.33f) / 0.33f);
            }
            else
            {
                col = Color.Lerp(color3, color4, (t - 0.66f) / 0.34f);
            }

            gradientTex.SetPixel(x, 0, col);
        }

        gradientTex.Apply();

        Sprite gradientSprite = Sprite.Create(gradientTex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        healthBarImage.sprite = gradientSprite;
        healthBarImage.type = Image.Type.Filled;
        healthBarImage.fillMethod = Image.FillMethod.Horizontal;
        healthBarImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthBarImage.fillAmount = 1f;
        healthBarImage.color = Color.white;
    }
}
