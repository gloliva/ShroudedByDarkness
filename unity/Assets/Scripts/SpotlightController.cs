using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightController : MonoBehaviour
{
    #region component_variables
    private Light lt;
    private Transform playerTransform;
    private PlayerController player;
    CircleCollider2D lightCollider;
    #endregion

    #region light_constants
    [SerializeField]
    [Tooltip("Default value for range")]
    private float defaultRange;

    [SerializeField]
    [Tooltip("Default value for intensity")]
    private float defaultIntensity;

    [SerializeField]
    [Tooltip("Default value for spot angle")]
    private float defaultAngle;

    [SerializeField]
    [Tooltip("Default value for z position")]
    private float defaultZ;

    [SerializeField]
    [Tooltip("Default value for total time it takes to adjust light")]
    private float defaultTotalTime;
    #endregion

    #region light_variables
    [HideInInspector]
    public bool inTransition; // the spotlight is in a transition state
    private bool lowHealthFlickerOn;
    private float lowHealthFlickerTotalTime;
    #endregion

    #region unity_functions
    // Start is called before the first frame update
    void Start()
    {
        // Set component variables
        lt = GetComponent<Light>();
        playerTransform = GetComponentInParent<Transform>();
        player = GetComponentInParent<PlayerController>();
        lightCollider = GetComponent<CircleCollider2D>();

        // Set default light properties
        lt.range = defaultRange;
        lt.intensity = defaultIntensity;
        lt.spotAngle = defaultAngle;
        transform.localPosition = new Vector3(0, 0, defaultZ);

        // Set light variables
        inTransition = false;
        lowHealthFlickerOn = false;
        lowHealthFlickerTotalTime = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region light_functions
    public void ResetSpotight()
    {
        StartCoroutine(UpdateSpotlight(defaultRange, defaultIntensity, defaultAngle, defaultZ, defaultTotalTime));
    }

    public void SetSpotight(float range, float intensity, float angle, float z, float totalTime)
    {
        StartCoroutine(UpdateSpotlight(range, intensity, angle, z, totalTime));
    }

    IEnumerator UpdateSpotlight(float endRange, float endIntensity, float endAngle, float endZ, float totalTime)
    {
        inTransition = true;
        float elapsedTime = 0f;
        
        // Get starting values
        float startRange = lt.range;
        float startIntensity = lt.intensity;
        float startAngle = lt.spotAngle;
        Vector3 startPosition = transform.localPosition;
        
        // Set ending position 
        Vector3 endPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, endZ);

        while (elapsedTime / totalTime < 1)
        {
            lt.range = Mathf.Lerp(startRange, endRange, elapsedTime / totalTime);
            if (!lowHealthFlickerOn)
            {
                lt.intensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / totalTime);
            }
            lt.spotAngle = Mathf.Lerp(startAngle, endAngle, elapsedTime / totalTime);

            transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / totalTime);

            // Increase time forward
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!lowHealthFlickerOn)
        {
            lt.intensity = endIntensity;
        }
        inTransition = false;
    }

    public void SetCollider(int itemType)
    {
        switch(itemType)
        {
            case 0:
                lightCollider.radius = 0.35f;
                break;
            case 1:
                lightCollider.radius = 1.0f;
                break;
            case 2:
                lightCollider.radius = 1.35f;
                break;
            case 3:
                lightCollider.radius = 0.2f;
                break;
            default:
                lightCollider.radius = 0.6f;
                break;
        }
    }
    #endregion

    #region health_functions
    #region health_update_light_functions
    public void HealthColorUpdate(float healthPercentage)
    {
        Debug.Log(healthPercentage);
        Color newHealthColor = Color.Lerp(Color.white, Color.red, 1 - healthPercentage);
        StartCoroutine(HealthColorTransition(newHealthColor, 0.2f));
    }

    public void SetLowHealthFlicker(bool turnOn, float lowHealthPercentage)
    {
        // Set the flicker rate based on currHealth / lowHealthThreshold
        // (0 < currHealth / lowHealthThreshold <= 1)
        if (lowHealthPercentage != 0)
        {
            SetLowHealthFlickerTotalTime(lowHealthPercentage);
        }

        // Turn on light flicker if low health and not already flickering
        if (!lowHealthFlickerOn && turnOn)
        {
            lowHealthFlickerOn = turnOn;
            StartCoroutine(LowHealthFlickerTransition(2f));
        } else if (lowHealthFlickerOn && !turnOn)
        {
            // Turn health flicker off
            // (turnOn == false)
            lowHealthFlickerOn = turnOn;
        }
    }

    // Set the flickering rate
    private void SetLowHealthFlickerTotalTime(float time)
    {
        lowHealthFlickerTotalTime = time;
    }
    #endregion

    #region health_coroutines
    IEnumerator HealthColorTransition(Color endColor, float totalTime)
    {
        float currTime = 0f;
        Color startColor = lt.color;
        while (currTime / totalTime < 1)
        {
            lt.color = Color.Lerp(startColor, endColor, currTime / totalTime);
            currTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator LowHealthFlickerTransition(float endIntensity)
    {
        float absoluteStartIntensity = lt.intensity;
        float startIntensity = absoluteStartIntensity;
        float currTime = 0f;
        float totalTime = lowHealthFlickerTotalTime;

        // While low on health
        while (lowHealthFlickerOn)
        {
            // Oscillate light on and off
            while (currTime / totalTime < 1)
            {
                lt.intensity = Mathf.Lerp(startIntensity, endIntensity, currTime / totalTime);
                currTime += Time.deltaTime;
                yield return null;
            }

            currTime = 0f;
            totalTime = lowHealthFlickerTotalTime;
            float tempIntensity = endIntensity;
            endIntensity = startIntensity;
            startIntensity = tempIntensity;
        }

        float resetTotalTime = 0.6f;
        startIntensity = lt.intensity;
        currTime = 0f;
        while (currTime / resetTotalTime < 1)
        {
            lt.intensity = Mathf.Lerp(startIntensity, absoluteStartIntensity, currTime / resetTotalTime);
            currTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator LowHealthFlickerReset(float endIntensity, float totalTime)
    {
        // Reset back to original intensity value
        float startIntensity = lt.intensity;
        float currTime = 0f;
        while (currTime / totalTime < 1)
        {
            lt.intensity = Mathf.Lerp(startIntensity, startIntensity, currTime / totalTime);
            currTime += Time.deltaTime;
            yield return null;
        }
    }
    #endregion
    #endregion
}
