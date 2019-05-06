using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneLightController : MonoBehaviour
{
    [SerializeField]
    private float maxIntensity;

    [SerializeField]
    private float totalTime;

    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        light.intensity = 0f;
        StartCoroutine(TurnOnLight());
    }

    IEnumerator TurnOnLight()
    {
        float currTime = 0f;
        while (currTime / totalTime <= 1)
        {
            light.intensity = Mathf.Lerp(0, maxIntensity, currTime / totalTime);
            currTime += Time.deltaTime;
            yield return null;
        }

        light.intensity = maxIntensity;
    }
}
