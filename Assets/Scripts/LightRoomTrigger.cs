using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRoomTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Room number")]
    private int roomNumber;

    private LightRoomTriggerMaster masterController;
    private bool playerInRoom;

    // Start is called before the first frame update
    void Start()
    {
        masterController = GetComponentInParent<LightRoomTriggerMaster>();
        playerInRoom = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            masterController.SetPlayerRoom(roomNumber);
            playerInRoom = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().SetCurrentRoom(roomNumber);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            masterController.SetPlayerRoom(roomNumber);
            playerInRoom = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().SetCurrentRoom(roomNumber);
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
