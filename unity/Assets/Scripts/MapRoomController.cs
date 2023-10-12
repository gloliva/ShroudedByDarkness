using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoomController : MonoBehaviour
{
    private bool roomDiscovered;
    private bool playerInRoom;

    private void Awake()
    {
        roomDiscovered = false;
        playerInRoom = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            roomDiscovered = playerInRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRoom = false;
        }
    }
}
