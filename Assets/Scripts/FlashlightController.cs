using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    #region component_variables
    private Light lt;
    private Transform playerTransform;
    private PolygonCollider2D flashlightCollider;
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
    [Tooltip("Default value for y position")]
    private float defaultY;

    [SerializeField]
    [Tooltip("Default value for z position")]
    private float defaultZ;

    [SerializeField]
    [Tooltip("Default value for total time it takes to adjust light")]
    private float defaultTotalTime;
    #endregion

    #region light_variables
    [HideInInspector]
    public bool inTransition; // the flashlight is in a transition state
    #endregion

    #region unity_functions
    // Start is called before the first frame update
    void Start()
    {
        // Set component variables
        lt = GetComponent<Light>();
        playerTransform = GetComponentInParent<Transform>();
        flashlightCollider = GetComponentInChildren<PolygonCollider2D>();
        flashlightCollider.enabled = false;

        // Set default light properties
        lt.range = defaultRange;
        lt.intensity = 0;
        lt.spotAngle = defaultAngle;
        transform.localPosition = new Vector3(0, defaultY, defaultZ);

        // Set light variables
        inTransition = false;
    }
    #endregion

    #region light_functions
    public void TurnOnFlashlight()
    {
        StartCoroutine(UpdateFlashlight(defaultRange, defaultIntensity, defaultAngle, defaultY, defaultZ, defaultTotalTime));
    }

    public void TurnOffFlashlight()
    {
        StartCoroutine(UpdateFlashlight(defaultRange, 0, defaultAngle, defaultY, defaultZ, defaultTotalTime));
    }

    IEnumerator UpdateFlashlight(float endRange, float endIntensity, float endAngle, float endY, float endZ, float totalTime)
    {
        inTransition = true;
        float elapsedTime = 0f;

        // Get starting values
        float startRange = lt.range;
        float startIntensity = lt.intensity;
        float startAngle = lt.spotAngle;
        Vector3 startPosition = transform.localPosition;

        // Set ending position 
        Vector3 endPosition = new Vector3(transform.localPosition.x, endY, endZ);

        while (elapsedTime / totalTime < 1)
        {
            lt.range = Mathf.Lerp(startRange, endRange, elapsedTime / totalTime);
            lt.intensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / totalTime);
            lt.spotAngle = Mathf.Lerp(startAngle, endAngle, elapsedTime / totalTime);

            transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / totalTime);

            // Increase time forward
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        lt.intensity = endIntensity;
        inTransition = false;
    }

    public void SetFlashlightCollider(bool on)
    {
        flashlightCollider.enabled = on;
    }
    #endregion
}
