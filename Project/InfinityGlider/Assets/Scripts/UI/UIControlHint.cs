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
        StartCoroutine(ShowHintsSequence());
    }

    private IEnumerator ShowHintsSequence()
    {
        hintText.text = "Use A and D to move left and right";
        yield return new WaitForSecondsRealtime(displayDuration);

        hintText.text = "Hold Space to stop gliding";
        yield return new WaitForSecondsRealtime(displayDuration);

        hintText.text = string.Empty;
    }
}