using System.Collections;
using UnityEngine;
using UnityEngine.UI;


// this script keeps track of which room sphere is active and fades between them when you switch.
public class RoomSwitcher : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private GameObject[] rooms;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isSwitching = false;

    private void Start()
    {
    
        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i].SetActive(i == 0);
        }
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

        // fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // swap which room is active while the screen is black
        foreach (GameObject room in rooms)
        {
            room.SetActive(room == target);
        }

        // fade back in
        yield return StartCoroutine(Fade(1f, 0f));

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