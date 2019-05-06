using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractMessageController : MonoBehaviour
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
        messageText = GetComponentInChildren<Text>();
        messageBackground = GetComponentInChildren<Image>();

        messageText.gameObject.SetActive(false);
        messageBackground.gameObject.SetActive(false);
    }

    public void SetMessageActive(bool active)
    {
        messageText.gameObject.SetActive(active);
        messageBackground.gameObject.SetActive(active);

        messageShowing = active;
    }

    public void SetInteractMessage(string message)
    {
        messageText.text = message;
    }
}
