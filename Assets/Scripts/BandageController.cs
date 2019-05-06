using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandageController : MonoBehaviour
{
    public int healAmount;

    private ButtonInteractMessageController interactMessage;
    private MessageController bandageMessage;
    private AudioManager audioManager;

    #region room_variables
    [SerializeField]
    [Tooltip("The room the object starts in")]
    private int startingRoom;

    private int roomCurrentlyIn;

    private bool isInvisible;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        bandageMessage = GameObject.FindObjectOfType<MessageController>();
        interactMessage = GameObject.FindObjectOfType<ButtonInteractMessageController>();
        audioManager = GameObject.FindObjectOfType<AudioManager>();

        // Set room variables
        roomCurrentlyIn = startingRoom;
        isInvisible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void PickUpBandages()
    {
        audioManager.Play("PickupBandage");
        bandageMessage.ShowItemPickUpMessage("Bandages");
        Destroy(this.gameObject);
    }

    public int getHealAmount()
    {
        return healAmount;
    }
}
