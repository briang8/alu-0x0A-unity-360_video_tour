using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class HotspotButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string targetRoomName;
    [SerializeField] private RoomSwitcher roomSwitcher;

    [Header("Feedback")]
    [SerializeField] private float hoverScale = 1.15f;
    [SerializeField] private float scaleSpeed = 8f;

    private Vector3 baseScale;
    private Vector3 targetScale;

    private void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnHotspotClicked);
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = baseScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = baseScale;
    }

    private void OnHotspotClicked()
    {
        roomSwitcher.SwitchToRoom(targetRoomName);
    }
}