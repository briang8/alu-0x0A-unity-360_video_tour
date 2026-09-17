using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// A hotspot that moves the Campus Tour to another stop.
[RequireComponent(typeof(Button))]
public class PanoramaHotspot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PanoramaTour tour;
    [SerializeField] private int targetStop;

    [Header("Feedback")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float scaleSpeed = 10f;
    [SerializeField] private float idlePulse = 0.04f;

    private Vector3 baseScale;
    private bool hovered;

    private void Awake()
    {
        baseScale = transform.localScale;
        GetComponent<Button>().onClick.AddListener(() => tour.GoTo(targetStop));
    }

    private void OnDisable()
    {
        hovered = false;
        transform.localScale = baseScale;
    }

    private void Update()
    {
        // gentle pulse so hotspots are easy to spot, bigger when pointed at
        float pulse = 1f + Mathf.Sin(Time.time * 3f) * idlePulse;
        Vector3 target = hovered ? baseScale * hoverScale : baseScale * pulse;
        transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData) => hovered = true;

    public void OnPointerExit(PointerEventData eventData) => hovered = false;
}
