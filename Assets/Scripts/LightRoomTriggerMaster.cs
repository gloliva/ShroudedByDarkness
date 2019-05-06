using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRoomTriggerMaster : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The room the player is in")]
    private int roomPlayerIn;

    // Start is called before the first frame update
    void Start()
    {
        roomPlayerIn = 1;
    }

    public int GetPlayerRoom()
    {
        Debug.Log("Get Player room: " + roomPlayerIn);
        return roomPlayerIn;
    }

    public void SetPlayerRoom(int room)
    {
        roomPlayerIn = room;
    }
}
