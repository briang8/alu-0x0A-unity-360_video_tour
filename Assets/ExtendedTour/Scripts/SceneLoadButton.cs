using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Put on any UI Button to open another scene with a fade.
[RequireComponent(typeof(Button))]
public class SceneLoadButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string sceneName;

    [Header("Hover feedback")]
    [SerializeField] private float hoverScale = 1.06f;
    [SerializeField] private float scaleSpeed = 10f;
    [SerializeField] private AudioSource clickSound;

    private Vector3 baseScale;
    private Vector3 targetScale;

    public string SceneName
    {
        get => sceneName;
        set => sceneName = value;
    }

    private void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnDisable()
    {
        transform.localScale = baseScale;
        targetScale = baseScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData) => targetScale = baseScale * hoverScale;

    public void OnPointerExit(PointerEventData eventData) => targetScale = baseScale;

    private void OnClicked()
    {
        if (clickSound != null) clickSound.Play();
        SceneFader.LoadScene(sceneName);
    }
}
