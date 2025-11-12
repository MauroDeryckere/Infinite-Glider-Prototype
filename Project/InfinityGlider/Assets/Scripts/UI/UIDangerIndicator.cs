using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIDangerIndicator : MonoBehaviour
{
    [SerializeField] private Image dangerOverlay;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float holdDuration = 0.2f;

    private Coroutine flashRoutine;

    public void Flash(float intensity)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine(intensity));
    }

    private IEnumerator FlashRoutine(float intensity)
    {
        Color color = dangerOverlay.color;
        float targetAlpha = Mathf.Lerp(0.2f, 0.7f, intensity);

        // fade in
        while (color.a < targetAlpha)
        {
            color.a = Mathf.MoveTowards(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
            dangerOverlay.color = color;
            yield return null;
        }

        // hold a bit at full intensity
        yield return new WaitForSeconds(holdDuration);

        // fade out
        while (color.a > 0f)
        {
            color.a = Mathf.MoveTowards(color.a, 0f, Time.deltaTime * fadeSpeed);
            dangerOverlay.color = color;
            yield return null;
        }

        flashRoutine = null;
    }
}
