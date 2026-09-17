using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Safety net: makes sure controller rays can press every world space UI in the scene.
[DefaultExecutionOrder(-100)]
public class XRUISetup : MonoBehaviour
{
    private void Awake()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
        }

        if (eventSystem.GetComponent<XRUIInputModule>() == null)
        {
            foreach (BaseInputModule module in eventSystem.GetComponents<BaseInputModule>())
            {
                module.enabled = false;
                Destroy(module);
            }
            eventSystem.gameObject.AddComponent<XRUIInputModule>();
        }

        foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (canvas.renderMode != RenderMode.WorldSpace) continue;
            if (canvas.GetComponent<TrackedDeviceGraphicRaycaster>() != null) continue;

            GraphicRaycaster old = canvas.GetComponent<GraphicRaycaster>();
            if (old != null) old.enabled = false;
            canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
        }
    }
}
