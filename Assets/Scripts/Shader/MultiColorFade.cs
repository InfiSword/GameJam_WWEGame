using UnityEngine;
using UnityEngine.UI;

public class MultiColorFadeUI : MonoBehaviour
{
    public enum ColorType
    {
        Red, SkyBlue, Green, Grey, Yellow
    }

    [Header("Settings")]
    public float fadeSpeed = 2.0f;
    public ColorType selectedColor = ColorType.Green;

    [Header("Color Definitions")]
    public Color redColor = Color.red;
    public Color skyBlueColor = new Color(0.53f, 0.81f, 0.92f);
    public Color greenColor = Color.green;
    public Color greyColor = Color.grey;
    public Color yellowColor = Color.yellow;

    private Image uiImage;
    private Material material;
    private float currentFade = 0f;
    private bool isFading = false;
    private bool isFadingOut = false;

    public void Init()
    {
        uiImage = GetComponent<Image>();

        material = new Material(uiImage.material);
        uiImage.material = material;

        material.SetFloat("_FadeAmount", 0);
    }

    void Update()
    {
        if (isFading)
        {
            currentFade += Time.deltaTime * fadeSpeed;
            material.SetFloat("_FadeAmount", Mathf.Clamp01(currentFade));

            if (currentFade >= 1.0f)
            {
                isFading = false;
            }
        }
        else if (isFadingOut)
        {
            currentFade -= Time.deltaTime * fadeSpeed;
            material.SetFloat("_FadeAmount", Mathf.Clamp01(currentFade));

            if (currentFade <= 0f)
            {
                isFadingOut = false;
            }
        }
    }

    public void StartFadeEffect(ColorType type)
    {
        Color targetColor = GetColorFromEnum(type);
        material.SetColor("_TargetColor", targetColor);

        currentFade = 0f;
        material.SetFloat("_FadeAmount", 0);
        isFading = true;
        isFadingOut = false;
    }

    public void StartFadeOutEffect()
    {
        isFading = false;
        isFadingOut = true;
    }

    public void ResetFade()
    {
        isFading = false;
        isFadingOut = false;
        currentFade = 0f;
        material.SetFloat("_FadeAmount", 0);
    }

    private Color GetColorFromEnum(ColorType type)
    {
        switch (type)
        {
            case ColorType.Red: return redColor;
            case ColorType.SkyBlue: return skyBlueColor;
            case ColorType.Green: return greenColor;
            case ColorType.Grey: return greyColor;
            case ColorType.Yellow: return yellowColor;
            default: return Color.white;
        }
    }

    public void TestClick()
    {
        StartFadeEffect(selectedColor);
    }
}