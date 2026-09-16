using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HotspotButton : MonoBehaviour
{
    [SerializeField] private string targetRoomName;
    [SerializeField] private RoomSwitcher roomSwitcher;

    private void Start()
    {
    
        GetComponent<Button>().onClick.AddListener(OnHotspotClicked);
    }

    private void OnHotspotClicked()
    {
        roomSwitcher.SwitchToRoom(targetRoomName);
    }
}