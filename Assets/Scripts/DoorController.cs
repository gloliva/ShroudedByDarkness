using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    #region door_variables
    [SerializeField]
    [Tooltip("Boolean value determining whether the door is locked")]
    private bool isLocked;

    [SerializeField]
    [Tooltip("Name of the key to unlock the door, if applicable")]
    private string keyName;
    #endregion

    #region message_variables
    private MessageController doorMessage;
    private ButtonInteractMessageController interactMessage;
    #endregion

    #region audio_variables
    private AudioManager audioManager;
    #endregion

    #region unity_functions
    private void Start()
    {
        doorMessage = GameObject.FindObjectOfType<MessageController>();
        interactMessage = GameObject.FindObjectOfType<ButtonInteractMessageController>();
        audioManager = GameObject.FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            interactMessage.SetInteractMessage("E to open door");
            interactMessage.SetMessageActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            interactMessage.SetMessageActive(false);
        }
    }
    #endregion

    public void OpenDoor(List<KeyController> keyList)
    {
        if (!isLocked)
        {
            audioManager.Play("DoorOpen");
            doorMessage.ShowOpenMessage();
            StartCoroutine(OpenDoorRoutine());
        }
        else if (hasKey(keyList))
        {
            audioManager.Play("DoorOpen");
            doorMessage.ShowUnlockedMessage(keyName);
            StartCoroutine(OpenDoorRoutine());
        }
        else
        {
            audioManager.Play("DoorLocked");
            doorMessage.ShowLockedMessage();
        }
    }

    public void OpenDoorOverride()
    {
        audioManager.Play("DoorOpen");
        StartCoroutine(OpenDoorRoutine());
    }

    IEnumerator OpenDoorRoutine()
    {
        float currTime = 0f;
        while (currTime < 0.5f)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }

    private bool hasKey(List<KeyController> keyList)
    {
        foreach (KeyController key in keyList)
        {
            if (key.getKeyName() == keyName)
            {
                return true;
            }
        }

        return false;
    }
}
