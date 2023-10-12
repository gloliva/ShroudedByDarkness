using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightItemController : MonoBehaviour
{
    #region light_variables
    [SerializeField]
    [Tooltip("This item's light type")]
    private int lightType;

    private MessageController itemMessage;
    private ButtonInteractMessageController interactMessage;
    private AudioManager audioManager;
    #endregion

    #region room_variables
    [SerializeField]
    [Tooltip("The room the object starts in")]
    private int startingRoom;

    private int roomCurrentlyIn;

    private bool isInvisible;
    #endregion

    #region unity_functions
    private void Start()
    {
        itemMessage = GameObject.FindObjectOfType<MessageController>();
        interactMessage = GameObject.FindObjectOfType<ButtonInteractMessageController>();
        audioManager = GameObject.FindObjectOfType<AudioManager>();

        // Set room variables
        roomCurrentlyIn = startingRoom;
        isInvisible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
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
        if (collision.gameObject.CompareTag("Player"))
        {
            interactMessage.SetMessageActive(false);
        }

        if (collision.gameObject.CompareTag("PlayerLight"))
        {
            isInvisible = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void PickUpLight()
    {
        string lightName;
        switch (lightType)
        {
            case 0:
                lightName = "Small lantern";
                break;
            case 1:
                lightName = "Medium lantern";
                break;
            case 2:
                lightName = "Large lantern";
                break;
            case 3:
                lightName = "Flashlight";
                break;
            default:
                lightName = "lantern";
                break;
        }

        audioManager.Play("PickupLight");
        itemMessage.ShowItemPickUpMessage(lightName);
    }
    #endregion

    #region accessors
    public int GetLightType()
    {
        return lightType;
    }
    #endregion
}
