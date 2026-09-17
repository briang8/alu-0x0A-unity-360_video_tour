using UnityEngine;

// Rotates a UI panel toward the user's level heading without changing its Unity-authored position.
public class HeadFollowPanel : MonoBehaviour
{
    [SerializeField] private float tiltTowardsUser = 20f;
    [SerializeField] private bool followContinuously = true;
    [SerializeField] private float reorientAngle = 45f;
    [SerializeField] private float followSpeed = 2.5f;

    private Transform head;
    private Vector3 direction = Vector3.forward;
    private bool moving;
    private float startTimer = 0.3f;
    private bool placed;

    private void LateUpdate()
    {
        if (head == null)
        {
            if (Camera.main == null) return;
            head = Camera.main.transform;
        }

        // wait briefly so the headset pose is valid before the first placement
        if (!placed)
        {
            startTimer -= Time.deltaTime;
            if (startTimer > 0f) return;
            direction = FlatForward();
            Place(1f);
            placed = true;
            return;
        }

        if (!followContinuously) return;

        Vector3 look = FlatForward();
        if (Vector3.Angle(look, direction) > reorientAngle) moving = true;

        if (moving)
        {
            direction = Vector3.Slerp(direction, look, Time.deltaTime * followSpeed).normalized;
            if (Vector3.Angle(look, direction) < 2f) moving = false;
        }

        Place(Time.deltaTime * followSpeed * 2f);
    }

    public void Recenter()
    {
        if (head == null) return;
        direction = FlatForward();
        Place(1f);
    }

    private Vector3 FlatForward()
    {
        Vector3 f = head.forward;
        f.y = 0f;
        return f.sqrMagnitude < 0.001f ? direction : f.normalized;
    }

    private void Place(float t)
    {
        Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(tiltTowardsUser, 0f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Mathf.Clamp01(t));
    }
}
