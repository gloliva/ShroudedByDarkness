using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroTextDisplay : MonoBehaviour
{
    [SerializeField]
    private string[] textToDisplay;

    [SerializeField]
    private float displayTime;

    [SerializeField]
    private float fadeTime;

    [SerializeField]
    private Text displayText;

    private bool startGame;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        startGame = false;
        gm = FindObjectOfType<GameManager>();
        StartCoroutine(DisplayText());
        StartCoroutine(StartFirstScene());
    }

    IEnumerator DisplayText()
    {
        float currTime = 0f;
        while (currTime < displayTime)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        foreach (string text in textToDisplay)
        {
            displayText.text = text;

            currTime = 0f;
            while (currTime / fadeTime <= 1)
            {
                displayText.color = new Color(displayText.color.r, displayText.color.g, displayText.color.b, Mathf.Lerp(0, 1, currTime / fadeTime));
                currTime += Time.deltaTime;
                yield return null;
            }

            currTime = 0f;
            while (currTime < displayTime)
            {
                currTime += Time.deltaTime;
                yield return null;
            }

            currTime = 0f;
            while (currTime / fadeTime <= 1)
            {
                displayText.color = new Color(displayText.color.r, displayText.color.g, displayText.color.b, Mathf.Lerp(1, 0, currTime / fadeTime));
                currTime += Time.deltaTime;
                yield return null;
            }

            currTime = 0f;
            while (currTime < 1f)
            {
                currTime += Time.deltaTime;
                yield return null;
            }
        }
        startGame = true;
    }

    IEnumerator StartFirstScene()
    {
        while (!startGame)
        {
            yield return null;
        }
        gm.StartLevel1();
    }
}
