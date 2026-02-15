using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardDisplay : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Image cardFrontImage;
    [SerializeField] private Image cardBackImage;

    [Header("Flip animation")]
    [SerializeField] private float flipDuration = 0.2f;

    public CardData CardData { get; private set; }

    public void Setup(CardData data)
    {
        CardData = data;

        if (cardFrontImage != null)
            cardFrontImage.sprite = data.frontSprite;

        if (cardBackImage != null)
            cardBackImage.sprite = data.backSprite;
    }

    public void ShowFrontImmediate()
    {
        if (cardFrontImage != null) cardFrontImage.gameObject.SetActive(true);
        if (cardBackImage != null) cardBackImage.gameObject.SetActive(false);

        transform.localScale = Vector3.one;
    }

    public void ShowBackImmediate()
    {
        if (cardFrontImage != null) cardFrontImage.gameObject.SetActive(false);
        if (cardBackImage != null) cardBackImage.gameObject.SetActive(true);

        transform.localScale = Vector3.one;
    }

    public void FlipToFront()
    {
        StopAllCoroutines();
        StartCoroutine(FlipRoutine(showFront: true));
    }

    public void FlipToBack()
    {
        StopAllCoroutines();
        StartCoroutine(FlipRoutine(showFront: false));
    }

    private IEnumerator FlipRoutine(bool showFront)
    {
        float half = flipDuration * 0.5f;
        float t = 0f;

        // Scale X from 1 → 0
        while (t < half)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, 0f, t / half);
            var local = transform.localScale;
            local.x = s;
            transform.localScale = local;
            yield return null;
        }

        // Switch visible side at "thin" moment
        if (showFront)
        {
            if (cardFrontImage != null) cardFrontImage.gameObject.SetActive(true);
            if (cardBackImage != null) cardBackImage.gameObject.SetActive(false);
        }
        else
        {
            if (cardFrontImage != null) cardFrontImage.gameObject.SetActive(false);
            if (cardBackImage != null) cardBackImage.gameObject.SetActive(true);
        }

        t = 0f;

        // Scale X from 0 → 1
        while (t < half)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1f, t / half);
            var local = transform.localScale;
            local.x = s;
            transform.localScale = local;
            yield return null;
        }
    }
}
