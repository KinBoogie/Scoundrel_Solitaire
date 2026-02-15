using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class CardUI : MonoBehaviour
{
    [Header("Visuals")]
    public Image artwork;                 // child Image that displays art
    public TextMeshProUGUI valueText;     // optional value label
    public Sprite backSprite;             // card back sprite (assign in prefab)
    public CanvasGroup canvasGroup;       // for fade-in

    [Header("Animation")]
    public float appearFadeDuration = 0.12f;
    public float flipDuration = 0.2f;

    // internal
    private Sprite faceSprite;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>Set card data (face image + a text label)</summary>
    public void SetCard(Sprite face, string value)
    {
        faceSprite = face;
        if (valueText != null) valueText.text = value;
        if (artwork != null) artwork.sprite = backSprite; // start showing back
    }

    /// <summary>Call to show the card with a short fade + flip to reveal face.</summary>
    public void ShowFaceAnimated()
    {
        StopAllCoroutines();
        StartCoroutine(AppearThenFlip());
    }

    IEnumerator AppearThenFlip()
    {
        // fade in
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            float t = 0f;
            while (t < appearFadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(t / appearFadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        // flip (scale X to 0, swap sprite, scale X back to 1)
        RectTransform rt = transform as RectTransform;
        Vector3 original = rt.localScale;
        float half = flipDuration * 0.5f;
        float timer = 0f;

        // scale X -> 0
        while (timer < half)
        {
            timer += Time.deltaTime;
            float s = Mathf.Lerp(1f, 0f, timer / half);
            rt.localScale = new Vector3(s, original.y, original.z);
            yield return null;
        }
        rt.localScale = new Vector3(0f, original.y, original.z);

        // swap sprite to face
        if (artwork != null && faceSprite != null)
            artwork.sprite = faceSprite;

        // scale X -> 1
        timer = 0f;
        while (timer < half)
        {
            timer += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1f, timer / half);
            rt.localScale = new Vector3(s, original.y, original.z);
            yield return null;
        }
        rt.localScale = original;
    }
}
