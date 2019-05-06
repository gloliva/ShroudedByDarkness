using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Name of this key")]
    private string keyName;

    private MessageController keyMessage;
    private ButtonInteractMessageController interactMessage;

    #region room_variables
    [SerializeField]
    [Tooltip("The room the object starts in")]
    private int startingRoom;

    private int roomCurrentlyIn;

    private bool isInvisible;
    #endregion

    private void Start()
    {
        keyMessage = GameObject.FindObjectOfType<MessageController>();
        interactMessage = GameObject.FindObjectOfType<ButtonInteractMessageController>();

        // Set room variables
        roomCurrentlyIn = startingRoom;
        isInvisible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            interactMessage.SetInteractMessage("E to pick up");
            interactMessage.SetMessageActive(true);
        }

        if (collision.gameObject.CompareTag("PlayerLight"))
        {
            if (FindObjectOfType<LightRoomTriggerMaster>().GetPlayerRoom() != roomCurrentlyIn)
            {
                isInvisible = true;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLight"))
        {
            if (FindObjectOfType<LightRoomTriggerMaster>().GetPlayerRoom() == roomCurrentlyIn)
            {
                isInvisible = false;
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            interactMessage.SetMessageActive(false);
        }

        if (collision.gameObject.CompareTag("PlayerLight"))
        {
            isInvisible = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public string getKeyName()
    {
        return keyName;
    }

    public void PickUpKey()
    {
        keyMessage.ShowItemPickUpMessage(keyName);
    }
}
