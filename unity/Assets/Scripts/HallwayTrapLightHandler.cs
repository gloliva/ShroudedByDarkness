using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayTrapLightHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Light[] lights = GetComponentsInChildren<Light>();
            StartCoroutine(LightFlicker(lights));
        }
    }

    IEnumerator LightFlicker(Light[] lights)
    {
        float waitTime = 1f;
        float dimTime = 0.5f;
        float currTime = 0f;

        float[] startIntensities = GetStartIntensities(lights);

        while (currTime < waitTime)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        currTime = 0f;
        while (currTime < dimTime)
        {
            DimLights(lights, startIntensities, currTime / waitTime);
            currTime += Time.deltaTime;
            yield return null;
        }

        SetActiveLights(lights, false);
    }

    private float[] GetStartIntensities(Light[] lights)
    {
        float[] startIntensities = new float[lights.Length];
        int i = 0;
        foreach (Light light in lights)
        {
            startIntensities[i] = light.intensity;
            ++i;
        }

        return startIntensities;
    }

    private void DimLights(Light[] lights, float[] startIntensities, float value)
    {
        for (int i = 0; i < lights.Length; ++i)
        {
            lights[i].intensity = Mathf.Lerp(startIntensities[i], 0, value);
        }
    }

    private void SetActiveLights(Light[] lights, bool active)
    {
        foreach (Light light in lights)
        {
            light.gameObject.SetActive(active);
        }
    }
}
