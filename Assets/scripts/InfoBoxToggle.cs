using System.Collections;
using UnityEngine;

/// <summary>
/// Toggles a UI info panel's visibility and interactivity via its CanvasGroup,
/// fading it in/out over <see cref="fadeDuration"/> seconds. Intended to be
/// wired directly to a Button's OnClick event.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class InfoBoxToggle : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.25f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;
    private bool isOpen;

    private void Awake()
    {
        // cache the CanvasGroup and ensure the panel starts hidden and non-interactive
        canvasGroup = GetComponent<CanvasGroup>();
        SetVisible(false, immediate: true);
    }

    /// <summary>
    /// Opens the panel if it is currently closed, or closes it if it is open.
    /// Safe to call repeatedly, e.g. from a single Button's OnClick event.
    /// </summary>
    public void Toggle()
    {
        SetVisible(!isOpen);
    }

    // single entry point for both the instant (Awake) and animated (Toggle) cases
    private void SetVisible(bool visible, bool immediate = false)
    {
        isOpen = visible;

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        if (immediate || !gameObject.activeInHierarchy)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
            return;
        }

        fadeRoutine = StartCoroutine(Fade(visible));
    }

    // animates alpha toward the target state and updates interactivity once it's reached
    private IEnumerator Fade(bool visible)
    {
        float from = canvasGroup.alpha;
        float to = visible ? 1f : 0f;
        float elapsed = 0f;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = visible;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.blocksRaycasts = visible;
        fadeRoutine = null;
    }
}
