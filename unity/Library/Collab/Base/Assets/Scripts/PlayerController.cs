using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region movement_variables
    private Rigidbody2D player;

    [SerializeField]
    [Tooltip("Determines the player's velocity")]
    private float movespeed;

    [SerializeField]
    [Tooltip("The player's starting angle, where 0 degrees is facing right")]
    private float startAngle;

    private float angle;
    #endregion

    #region health_variables
    [SerializeField]
    [Tooltip("Max health for player")]
    private int maxHealth;

    private int currHealth;

    [SerializeField]
    [Tooltip("The player health value which begins light flickering")]
    private int lowHealthFlickerThreshold;
    #endregion

    #region light_variables
    private SpotlightController spotlight;
    private FlashlightController flashlight;
    private bool flashlightOn;
    private bool itemInUse;
    private int[] numLightsAvailable;
    #endregion

    #region inventory_variables
    [SerializeField]
    [Tooltip("The itemcount gameObject to update the UI")]
    private ItemCount ic;
    #endregion

    #region gameplay_variables
    GameManager gm;
    #endregion

    #region unity_functions
    // Start is called before the first frame update
    void Awake()
    {
        // Get components
        player = GetComponent<Rigidbody2D>();
        spotlight = GetComponentInChildren<SpotlightController>();
        flashlight = GetComponentInChildren<FlashlightController>();
        gm = FindObjectOfType<GameManager>();

        // Set movement variables
        angle = startAngle;

        // Set health variables
        currHealth = maxHealth;

        // Set light variables
        numLightsAvailable = new int[LightMasterController.numLightItems];
        flashlightOn = false;
        itemInUse = false;
    }


    // Update is called once per frame
    void Update()
    {
        Move();
        SelectLighting();
        TestHealth(); // TODO remove once finished with health testing system
        GameplayUpdate();
    }
    #endregion

    #region movement_functions
    private void Move()
    {
        //mouse controls
        Vector3 pos = Input.mousePosition;
        if (checkMouse(pos))
        {
            pos.x -= Screen.width / 2;
            pos.y -= Screen.height / 2;

            angle = Mathf.Atan2(pos.y, pos.x);

            //Used if the start angle of the player isn't zero
            angle += startAngle * Mathf.PI / 180;

            //Sets rotation
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y,
                angle * 180 / Mathf.PI);
        }

        //WASD controls
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        player.transform.position += new Vector3(h, v, 0) * movespeed * Time.deltaTime;
    }

    //checks if mouse is in screen
    bool checkMouse(Vector3 pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x > Screen.width || pos.y > Screen.height)
        {
            return false;
        }
        return true;
    }
    #endregion

    #region health_functions
    private float GetHealthPercentage()
    {
        return (float) currHealth / maxHealth;
    }

    private float GetLowHealthPercentage()
    {
        return (float) currHealth / lowHealthFlickerThreshold;
    }

    // This is just to test health/lighting system before combat and health systems are in place
    private void TestHealth()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            currHealth = Mathf.Max(currHealth - 5, 0);
            spotlight.HealthColorUpdate(GetHealthPercentage());

            if (currHealth <= lowHealthFlickerThreshold)
            {
                spotlight.SetLowHealthFlicker(true, GetLowHealthPercentage());
            }
        } else if (Input.GetKeyDown(KeyCode.Equals))
        {
            currHealth = Mathf.Min(currHealth + 5, maxHealth);
            spotlight.HealthColorUpdate(GetHealthPercentage());

            if (currHealth > lowHealthFlickerThreshold)
            {
                spotlight.SetLowHealthFlicker(false, GetLowHealthPercentage());
            } else
            {
                spotlight.SetLowHealthFlicker(true, GetLowHealthPercentage());
            }
        }
    }
    #endregion

    #region light_functions
    private void SelectLighting()
    {
        // Use only one item at a time
        if (itemInUse)
        {
            return;
        }

        // Check which light source to use
        if (Input.GetKeyDown(KeyCode.Alpha1) && !flashlightOn)
        {
            if (!spotlight.inTransition && numLightsAvailable[(int) LightMasterController.LIGHTS.SmallSpotlight] > 0)
            {
                UseLightItem((int) LightMasterController.LIGHTS.SmallSpotlight);
                spotlight.SetSpotight(12, 2, 65, -4, 1);
                StartCoroutine(HandleLightDuration((int) LightMasterController.LIGHTS.SmallSpotlight));
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !flashlightOn && numLightsAvailable[(int)LightMasterController.LIGHTS.MedSpotlight] > 0)
        {
            if (!spotlight.inTransition)
            {
                UseLightItem((int) LightMasterController.LIGHTS.MedSpotlight);
                spotlight.SetSpotight(14, 5, 100, -5, 1);
                StartCoroutine(HandleLightDuration((int) LightMasterController.LIGHTS.MedSpotlight));
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && !flashlightOn && numLightsAvailable[(int)LightMasterController.LIGHTS.LargeSpotlight] > 0)
        {
            if (!spotlight.inTransition)
            {
                UseLightItem((int) LightMasterController.LIGHTS.LargeSpotlight);
                spotlight.SetSpotight(22, 5, 100, -10, 1);
                StartCoroutine(HandleLightDuration((int) LightMasterController.LIGHTS.LargeSpotlight));
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            // Flashlight is off
            if (!flashlightOn)
            {
                // Minimize spotlight
                spotlight.SetSpotight(12, 2, 65, -4, 1);

                // Turn on flashlight
                flashlight.TurnOnFlashlight();
                flashlightOn = !flashlightOn;
            }
            // Flashlight is on
            else
            {
                // Reset to default spotlight
                spotlight.ResetSpotight();

                // Turn off flashlight
                flashlight.TurnOffFlashlight();
                flashlightOn = !flashlightOn;
            }
        }
    }

    IEnumerator HandleLightDuration(int itemType)
    {
        itemInUse = true;
        float itemTime = 0f;
        float lightDuration = LightMasterController.lightDurations[itemType];
        while (itemTime < lightDuration)
        {
            itemTime += Time.deltaTime;
            yield return null;
        }

        spotlight.ResetSpotight();
        itemInUse = false;
    }

    #endregion

    #region inventory_functions
    public void AddLightToInventory(int lightType)
    {
        Debug.Assert(lightType < numLightsAvailable.Length);
        numLightsAvailable[lightType]++;
        ic.addItem(lightType);
    }

    private bool HasItem(int lightType)
    {
        Debug.Assert(lightType < numLightsAvailable.Length);
        return numLightsAvailable[lightType] > 0;
    }

    private void UseLightItem(int lightType)
    {
        Debug.Assert(lightType < numLightsAvailable.Length);
        Debug.Assert(numLightsAvailable[lightType] > 0);
        numLightsAvailable[lightType]--;
        ic.removeItem(lightType);
    }
    #endregion

    #region gameplay_functions
    private void GameplayUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gm.GoToMainMenu();
        }
    }
    #endregion
}
