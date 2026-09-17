using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Persistent fade that survives scene loads.
// Every scene change and every in-scene location change goes through here so the user never sees a hard cut.
public class SceneFader : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.6f;
    [SerializeField] private Color fadeColor = Color.black;

    private static SceneFader instance;
    private static bool creatingAtRuntime;
    private bool playIntroFade = true;

    private Canvas canvas;
    private CanvasGroup group;

    public static bool IsBusy { get; private set; }

    public static SceneFader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SceneFader>();
                if (instance == null)
                {
                    creatingAtRuntime = true;
                    instance = new GameObject("SceneFader").AddComponent<SceneFader>();
                    creatingAtRuntime = false;
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildOverlay();

        // placed in a scene: start black and fade in once the camera exists
        // created on demand by a button: stay clear so the button's own fade runs
        playIntroFade = !creatingAtRuntime;
        group.alpha = playIntroFade ? 1f : 0f;
        group.blocksRaycasts = false;
    }

    private void Start()
    {
        if (instance != this || !playIntroFade) return;
        StartCoroutine(FadeInAfterLoad());
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            IsBusy = false;
        }
    }

    // Called by menu and navigation buttons
    public static void LoadScene(string sceneName)
    {
        if (IsBusy) return;

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning("SceneFader: scene '" + sceneName + "' is not in Build Settings.");
            return;
        }

        Instance.StartCoroutine(Instance.LoadRoutine(sceneName));
    }

    // Fade to black, run an action (e.g. swap the 360 photo), fade back in
    public void FadeOutIn(Action atBlack)
    {
        if (IsBusy) return;
        StartCoroutine(FadeOutInRoutine(atBlack));
    }

    private IEnumerator FadeOutInRoutine(Action atBlack)
    {
        IsBusy = true;
        AttachToCamera();
        yield return Fade(0f, 1f);
        atBlack?.Invoke();
        yield return null; // give the new content one frame to settle
        yield return Fade(1f, 0f);
        IsBusy = false;
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        IsBusy = true;
        AttachToCamera();
        yield return Fade(group.alpha, 1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        yield return FadeInAfterLoad();
    }

    private IEnumerator FadeInAfterLoad()
    {
        IsBusy = true;
        group.alpha = 1f;

        // wait a couple of frames so the XR camera and head pose are ready
        yield return null;
        yield return null;
        AttachToCamera();
        yield return new WaitForSeconds(0.15f);

        yield return Fade(1f, 0f);
        IsBusy = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        group.alpha = from;
        group.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration));
            yield return null;
        }

        group.alpha = to;
        group.blocksRaycasts = to > 0.01f;
    }

    // Screen Space Overlay is not drawn in the headset, so the fade sits just in front of the XR camera
    private void AttachToCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = cam.nearClipPlane + 0.02f;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        canvas.sortingOrder = short.MaxValue;
    }

    private void BuildOverlay()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;
        group = gameObject.AddComponent<CanvasGroup>();
        group.interactable = false;

        GameObject imageGo = new GameObject("Fade", typeof(RectTransform), typeof(Image));
        imageGo.transform.SetParent(transform, false);
        RectTransform rt = imageGo.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(-50f, -50f);
        rt.offsetMax = new Vector2(50f, 50f);

        Image image = imageGo.GetComponent<Image>();
        image.color = fadeColor;
        image.raycastTarget = false;
    }
}
