using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;


// this script keeps track of which room sphere is active and fades between them when you switch.
public class RoomSwitcher : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private GameObject[] rooms;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isSwitching = false;

    private void Awake()
    {
        UseXRInputModule();
    }

    // XR controller rays can only press UI through the XR UI Input Module.
    // The EventSystem had the Input System UI Input Module, which ignores the controllers completely.
    private void UseXRInputModule()
    {
        EventSystem eventSystem = EventSystem.current != null ? EventSystem.current : FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
        }

        if (eventSystem.GetComponent<XRUIInputModule>() != null) return;

        foreach (BaseInputModule module in eventSystem.GetComponents<BaseInputModule>())
        {
            module.enabled = false;
            Destroy(module);
        }

        eventSystem.gameObject.AddComponent<XRUIInputModule>();
    }

    private void Start()
    {
        MakeRoomCanvasesClickable();
        MakeFadeVisibleInHeadset();

        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i].SetActive(i == 0);
        }
    }


    // XR controller rays only hit canvases that have a TrackedDeviceGraphicRaycaster,
    // a plain GraphicRaycaster only works with the mouse
    private void MakeRoomCanvasesClickable()
    {
        foreach (GameObject room in rooms)
        {
            foreach (Canvas canvas in room.GetComponentsInChildren<Canvas>(true))
            {
                if (canvas.renderMode != RenderMode.WorldSpace) continue;

                if (canvas.GetComponent<TrackedDeviceGraphicRaycaster>() == null)
                {
                    GraphicRaycaster old = canvas.GetComponent<GraphicRaycaster>();
                    if (old != null) old.enabled = false;
                    canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
                }
            }
        }
    }

    // Screen Space Overlay canvases are not drawn inside the headset, so render the fade in front of the XR camera instead
    private void MakeFadeVisibleInHeadset()
    {
        if (fadeCanvasGroup == null || Camera.main == null) return;

        Canvas fadeCanvas = fadeCanvasGroup.GetComponentInParent<Canvas>();
        if (fadeCanvas == null || fadeCanvas.renderMode != RenderMode.ScreenSpaceOverlay) return;

        fadeCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        fadeCanvas.worldCamera = Camera.main;
        fadeCanvas.planeDistance = Camera.main.nearClipPlane + 0.05f;
        fadeCanvas.sortingOrder = 100;
    }

    public void SwitchToRoom(string roomName)
    {
        if (isSwitching) return; // ignore clicks while a fade is already happening

        GameObject target = FindRoomByName(roomName);
        if (target == null)
        {
            Debug.LogWarning("RoomSwitcher: no room found named " + roomName);
            return;
        }

        StartCoroutine(FadeAndSwitch(target));
    }

    private GameObject FindRoomByName(string roomName)
    {
        foreach (GameObject room in rooms)
        {
            if (room.name == roomName)
            {
                return room;
            }
        }
        return null;
    }

    private IEnumerator FadeAndSwitch(GameObject target)
    {
        isSwitching = true;
        fadeCanvasGroup.blocksRaycasts = true; // stop double clicks during the fade

        // fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // swap which room is active while the screen is black
        foreach (GameObject room in rooms)
        {
            room.SetActive(room == target);
        }

        // fade back in
        yield return StartCoroutine(Fade(1f, 0f));

        fadeCanvasGroup.blocksRaycasts = false; // let clicks through again
        isSwitching = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        fadeCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
    }
}