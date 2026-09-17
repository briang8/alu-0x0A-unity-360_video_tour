using UnityEngine;

// Places a UI panel in front of the user once, when the scene starts, then leaves it alone.
// Nothing follows the head while you look around, so the panel never drifts.
public class HeadAnchoredPanel : MonoBehaviour
{
    [SerializeField] private float distance = 2f;
    [SerializeField] private float heightOffset = -0.05f;
    [SerializeField] private float tilt;
    [Tooltip("Seconds to wait so the headset has a valid head pose before placing")]
    [SerializeField] private float placeDelay = 0.4f;

    private Transform head;
    private float timer;
    private bool placed;

    private void OnEnable()
    {
        placed = false;
        timer = 0f;
    }

    private void LateUpdate()
    {
        if (placed) return;

        if (head == null)
        {
            if (Camera.main == null) return;
            head = Camera.main.transform;
        }

        timer += Time.deltaTime;
        if (timer < placeDelay) return;

        Place();
        placed = true;
    }

    // Call this from a button if you ever want the panel brought back in front of you
    public void Recenter()
    {
        if (head == null && Camera.main != null) head = Camera.main.transform;
        if (head != null) Place();
    }

    private void Place()
    {
        Vector3 forward = head.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;
        forward.Normalize();

        transform.position = head.position + forward * distance + Vector3.up * heightOffset;
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(tilt, 0f, 0f);
    }
}
