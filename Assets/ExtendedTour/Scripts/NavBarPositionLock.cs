using UnityEngine;

[DefaultExecutionOrder(1000)]
public class NavBarPositionLock : MonoBehaviour
{
    private Vector3 authoredPosition;
    private Quaternion authoredRotation;

    private void Awake()
    {
        authoredPosition = transform.position;
        authoredRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.SetPositionAndRotation(authoredPosition, authoredRotation);
    }
}