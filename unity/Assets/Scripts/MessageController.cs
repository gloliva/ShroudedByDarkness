using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Time in seconds a message appears")]
    private float appearTime;

    private bool messageShowing;

    private Text messageText;
    private Image messageBackground;

    // Start is called before the first frame update
    void Start()
    {
        messageShowing = false;

        messageText = GetComponentInChildren<Text>();
        messageBackground = GetComponentInChildren<Image>();

        messageText.gameObject.SetActive(false);
        messageBackground.gameObject.SetActive(false);
    }

    public void ShowLockedMessage()
    {
        if (!messageShowing)
        {
            messageText.text = "The door is locked.";
            StartCoroutine(ShowMessage());
        }

    }

    public void ShowUnlockedMessage(string keyName)
    {
        if (!messageShowing)
        {
            messageText.text = "You unlock the door using: " + keyName + ".";
            StartCoroutine(ShowMessage());
        }
    }

    public void ShowOpenMessage()
    {
        if (!messageShowing)
        {
            messageText.text = "You open the door.";
            StartCoroutine(ShowMessage());
        }
    }

    public void ShowItemPickUpMessage(string itemName)
    {
        if (!messageShowing)
        {
            messageText.text = "You picked up: " + itemName;
            StartCoroutine(ShowMessage());
        }
    }

    IEnumerator ShowMessage()
    {
        messageShowing = true;

        messageText.gameObject.SetActive(true);
        messageBackground.gameObject.SetActive(true);

        float currTime = 0f;
        while (currTime <= appearTime)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        messageText.gameObject.SetActive(false);
        messageBackground.gameObject.SetActive(false);

        messageShowing = false;
        yield return null;
    }
}
