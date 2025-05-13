using UnityEngine;
using UnityEngine.UI;

public class CookingProgressUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Link to the StoveCounter component on the pan.")]
    public StoveCounter stoveCounter;

    [Tooltip("Slider UI component for showing progress.")]
    public Slider progressSlider;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Expect a CanvasGroup on the same GameObject to show/hide the UI
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        // Subscribe to events
        stoveCounter.OnProgressChanged += HandleProgressChanged;
        stoveCounter.OnStateChanged += HandleStateChanged;

        // Initially hidden
        canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        // Cleanup event subscriptions
        if (stoveCounter != null)
        {
            stoveCounter.OnProgressChanged -= HandleProgressChanged;
            stoveCounter.OnStateChanged -= HandleStateChanged;
        }
    }

    private void HandleProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        // Update slider value (0–1)
        progressSlider.value = e.progressNormalized;
    }

    private void HandleStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        // Show UI when cooking or burned (still on stove)
        if (e.state == StoveCounter.State.Cooking || e.state == StoveCounter.State.Cooked)
        {
            canvasGroup.alpha = 1f;
        }
        else
        {
            // Hide when idle or fully burned
            canvasGroup.alpha = 0f;
        }
    }
}
