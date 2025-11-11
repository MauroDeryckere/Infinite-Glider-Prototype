using UnityEngine;
using TMPro;
using System.Collections;

public class UIControlHint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private float displayDuration = 3f;

    private void Start()
    {
        if (hintText == null)
        {
            Debug.LogError("ControlHint: No TextMeshProUGUI assigned.");
            return;
        }

        hintText.gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        Debug.LogError("ControlHint: No TextMeshProUGUI assigned.");
        yield return new WaitForSecondsRealtime(displayDuration);

        hintText.text = string.Empty;
    }
}