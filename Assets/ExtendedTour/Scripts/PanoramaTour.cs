using System;
using UnityEngine;
using TMPro;

// Switches between 360 photo stops, the same way RoomSwitcher switches video rooms:
// each stop is a sphere plus its hotspots, and only one stop is active at a time.
public class PanoramaTour : MonoBehaviour
{
    [Serializable]
    public class TourStop
    {
        public string displayName = "Stop";
        [Tooltip("The stop's sphere and hotspots")]
        public GameObject content;
    }

    [SerializeField] private TourStop[] stops = new TourStop[0];
    [SerializeField] private int startStop;

    [Header("Optional")]
    [SerializeField] private TMP_Text locationLabel;
    [SerializeField] private AudioSource moveSound;

    private int current = -1;

    public int StopCount => stops.Length;

    private void Start()
    {
        ShowStop(Mathf.Clamp(startStop, 0, Mathf.Max(0, stops.Length - 1)));
    }

    public void GoTo(int index)
    {
        if (index == current || index < 0 || index >= stops.Length || SceneFader.IsBusy) return;

        if (moveSound != null) moveSound.Play();
        SceneFader.Instance.FadeOutIn(() => ShowStop(index));
    }

    public void Next() => GoTo(current + 1);

    public void Previous() => GoTo(current - 1);

    private void ShowStop(int index)
    {
        if (stops.Length == 0) return;

        current = index;

        for (int i = 0; i < stops.Length; i++)
        {
            if (stops[i].content != null) stops[i].content.SetActive(i == index);
        }

        if (locationLabel != null)
        {
            locationLabel.text = stops[index].displayName + "  <size=70%>(" + (index + 1) + "/" + stops.Length + ")</size>";
        }
    }
}
