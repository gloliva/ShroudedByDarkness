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

    #region stamina_variables
    [SerializeField]
    [Tooltip("Max stamina for player")]
    private float maxStamina;

    [SerializeField]
    [Tooltip("Stamina Recovery Rate")]
    private float staminaRecovery;

    [SerializeField]
    [Tooltip("Sprint Speed up")]
    private int speedUp;

    private float currStamina;

    private bool tiredOut;

    private StaminaController staminaBar;
    #endregion

    #region health_variables
    [SerializeField]
    [Tooltip("Max health for player")]
    private int maxHealth;

    private int currHealth;

    private bool isDead;

    [SerializeField]
    [Tooltip("The player health value which begins light flickering")]
    private int lowHealthFlickerThreshold;

    private HealthBarController healthBar;

    [SerializeField]
    [Tooltip("Player Dead sprite")]
    Sprite playerDead;
    #endregion

    #region light_variables
    private SpotlightController spotlight;
    private FlashlightController flashlight;
    private int currLightItem;
    private bool flashlightOn;
    private bool itemInUse;
    #endregion

    #region attack_variables
    private bool isAttacking;
    CapsuleCollider2D armCollider;
    #endregion

    #region inventory_variables
    private WeaponAndItemUI inventory;
    private Dictionary<int, int> itemList;
    private List<KeyController> keyList;
    private List<string> weaponList;
    private int currWeapon;
    private Weapon_Ranged rangedWeapon;
    private Weapon_Close meleeWeapon;
    #endregion

    #region animator_variables
    Animator anim;
    #endregion

    #region audio_variables
    private AudioManager audioManager;

    [SerializeField]
    [Tooltip("Names of audio samples for player getting hit")]
    private string[] hitClipNames;
    private int hitClipPos;
    #endregion

    #region gameplay_variables
    GameManager gm;

    [SerializeField]
    [Tooltip("Turn on for instructions screen")]
    private bool godMode;
    #endregion

    #region scripted_event_variables
    [SerializeField]
    private string endDoorKeyName;
    private int endDoorKeys;
    #endregion

    #region unity_functions
    // Start is called before the first frame update
    void Awake()
    {
        // Get components
        player = GetComponent<Rigidbody2D>();
        spotlight = GetComponentInChildren<SpotlightController>();
        flashlight = GetComponentInChildren<FlashlightController>();
        anim = GetComponent<Animator>();
        gm = FindObjectOfType<GameManager>();

        // Set movement variables
        angle = startAngle;

        //Set stamina variables
        currStamina = maxStamina;
        tiredOut = false;
        staminaBar = GameObject.FindGameObjectWithTag("StaminaBar").GetComponent<StaminaController>();

        // Set health variables
        currHealth = maxHealth;
        isDead = false;
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBarController>();

        // Set light variables
        currLightItem = 0;
        flashlightOn = false;
        itemInUse = false;

        // Set attack variables
        isAttacking = false;
        armCollider = GetComponents<CapsuleCollider2D>()[1];
        armCollider.enabled = false;

        // Set inventory variables
        inventory = GameObject.FindObjectOfType<WeaponAndItemUI>();
        inventory.SetWeaponImage(null, "", 0, 0);
        inventory.SetItemImage(null, "", 0, 0, 0);
        itemList = new Dictionary<int, int>();
        InitLightItems();
        keyList = new List<KeyController>();
        weaponList = new List<string>();
        weaponList.Add("No weapon");
        currWeapon = 0;
        ShowWeaponUI(weaponList[currWeapon], null);

        // Set animator variables
        anim.SetBool("Moving", false);
        anim.SetBool("hasMelee", false);
        anim.SetBool("hasGun", false);
        anim.SetBool("Attacking", false);

        // Set scripted event variables
        endDoorKeys = 0;
    }

    private void Start()
    {
        // Going to try calling this in Start instead of awake
        rangedWeapon = GetComponentInChildren<Weapon_Ranged>();
        rangedWeapon.gameObject.SetActive(false);
        meleeWeapon = GetComponentInChildren<Weapon_Close>();
        meleeWeapon.gameObject.SetActive(false);

        // Cheat codes / Testing
        if (godMode)
        {
            EnableGodMode();
        }

        // Set audio variables, needs to be in Start, not Awake
        audioManager = FindObjectOfType<AudioManager>();
        hitClipPos = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && currWeapon >= 0)
        {
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchWeapon();
        }
        SelectLighting();
        TestHealth(); // TODO remove once finished with health testing system
        GameplayUpdate();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            MovePlayer();
            RotatePlayer();
        }
    }
    #endregion

    #region attack_functions
    void Attack()
    {
        //go into attack animation
        string currWeaponName = weaponList[currWeapon];
        if (currWeaponName.Equals("Gun"))
        {
            if (rangedWeapon.getCurrentAmmo() > 0)
            {
                rangedWeapon.Attack();
                StartCoroutine(AttackRoutine(GetWeaponAttackTime(currWeaponName)));
                inventory.SetAmmoCount(currWeaponName, rangedWeapon.getCurrentAmmo());
            } else
            {
                audioManager.Play("PistolEmpty");
            }
        }
        else
        {
            StartCoroutine(AttackRoutine(GetWeaponAttackTime(currWeaponName)));
        }
    }

    private float GetWeaponAttackTime(string weaponName)
    {
        switch (weaponName)
        {
            case "Gun":
                return 0.05f;
            case "Pipe":
                return 0.05f;
            default:
                return 0f;
        }
    }

    IEnumerator AttackRoutine(float attackTime)
    {
        isAttacking = true;
        player.velocity = Vector2.zero;
        player.angularVelocity = 0f;
        anim.SetBool("Attacking", true);

        float currTime = 0f;

        while (currTime < attackTime)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        anim.SetBool("Attacking", false);
        isAttacking = false;
    }

    private void WeaponAnimator(string weaponName)
    {
        switch (weaponName) {
            case "Gun":
                anim.SetBool("hasMelee", false);
                anim.SetBool("hasGun", true);
                break;
            case "Pipe":
                anim.SetBool("hasMelee", true);
                anim.SetBool("hasGun", false);
                break;
            default:
                anim.SetBool("hasMelee", false);
                anim.SetBool("hasGun", false);
                break;
        }
    }
    #endregion

    #region movement_functions
    private void MovePlayer()
    {
        //WASD controls
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool isMoving = h != 0 || v != 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !tiredOut;

        if (isMoving)
        {
            anim.SetBool("Moving", true);
        } else
        {
            anim.SetBool("Moving", false);
        }

        if (isRunning)
        {
            player.velocity = new Vector3(h, v, 0).normalized * movespeed * speedUp;
            currStamina -= Time.deltaTime * 10;
            if (currStamina <= 0)
            {
                tiredOut = true;
                currStamina = 0;
            }
        }
        else
        {
            if (currStamina < maxStamina)
            {
                if (tiredOut)
                {
                    currStamina += staminaRecovery * Time.deltaTime / 2;
                }
                else
                {
                    currStamina += staminaRecovery * Time.deltaTime;
                }
                
                if (currStamina >= maxStamina)
                {
                    currStamina = maxStamina;
                    tiredOut = false;
                }
            }
            player.velocity = new Vector3(h, v, 0) * movespeed;
        }

        //PlayerMoveAudio(isMoving, isRunning);
        staminaBar.SetStaminaVal(currStamina);
    }

    private void RotatePlayer()
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

    public void freeze()
    {
        isDead = true;
        player.velocity = Vector2.zero;
        player.angularVelocity = 0f;
        anim.SetBool("Moving", false);
        anim.SetBool("Attacking", false);
        anim.SetBool("hasGun", false);
        anim.SetBool("hasMelee", false);
        anim.enabled = false;
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
            TakeDamage(1);
        } else if (Input.GetKeyDown(KeyCode.Equals))
        {
            Heal(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currHealth = Mathf.Max(currHealth - damage, 0);
        healthBar.DecreaseHealth(currHealth);
        if (currHealth <= 0)
        {
            Die();
            return;
        }

        // Play getting hit sample
        audioManager.Play(hitClipNames[hitClipPos]);
        hitClipPos = (hitClipPos + 1) % hitClipNames.Length;

        spotlight.HealthColorUpdate(GetHealthPercentage());
        if (currHealth <= lowHealthFlickerThreshold)
        {
            spotlight.SetLowHealthFlicker(true, GetLowHealthPercentage());
        }
    }

    private void Heal(int health)
    {
        currHealth = Mathf.Min(currHealth + health, maxHealth);
        healthBar.IncreaseHealth(currHealth);

        spotlight.HealthColorUpdate(GetHealthPercentage());
        if (currHealth > lowHealthFlickerThreshold)
        {
            spotlight.SetLowHealthFlicker(false, GetLowHealthPercentage());
        }
        else
        {
            spotlight.SetLowHealthFlicker(true, GetLowHealthPercentage());
        }
    }

    private void Die()
    {
        isDead = true;
        player.velocity = Vector2.zero;
        player.angularVelocity = 0f;
        anim.SetBool("Moving", false);
        anim.SetBool("Attacking", false);
        anim.SetBool("hasGun", false);
        anim.SetBool("hasMelee", false);
        anim.enabled = false;
        transform.localScale = new Vector3(5, 5, 1);
        player.GetComponent<SpriteRenderer>().sprite = playerDead;
        player.GetComponent<SpriteRenderer>().sortingLayerName = "DeadSprite";
        player.GetComponent<CapsuleCollider2D>().enabled = false;
        audioManager.Play("PlayerDeath");
        gm.SceneRestartOnDeath(2f);
    }

    IEnumerator DeathRoutine()
    {
        float currTime = 0f;
        while (currTime < 2f)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        gm.GoToMainMenu();
    }
    #endregion

    #region light_functions
    private void SelectLighting()
    {
        // Switch item type
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currLightItem = (currLightItem + 1) % LightMasterController.numLightItems;
            string lightName = LightMasterController.lanternNames[currLightItem];
            inventory.SetItemImage(inventory.GetItemSprite(currLightItem), lightName, itemList[currLightItem], 60, 60);
        }
        // Use light item
        else if (Input.GetKeyDown(KeyCode.F) && !itemInUse)
        {
            if (!spotlight.inTransition && HasItem(currLightItem))
            {
                UseLightItem(currLightItem);
                float range = LightMasterController.spotlightLanternValues[currLightItem, 0];
                float intensity = LightMasterController.spotlightLanternValues[currLightItem, 1];
                float angle = LightMasterController.spotlightLanternValues[currLightItem, 2];
                float z = LightMasterController.spotlightLanternValues[currLightItem, 3];
                float totalTime = LightMasterController.spotlightLanternValues[currLightItem, 4];
                spotlight.SetSpotight(range, intensity, angle, z, totalTime);

                if (currLightItem == (int) LightMasterController.LIGHTS.Flashlight)
                {
                    flashlight.TurnOnFlashlight();
                    flashlight.SetFlashlightCollider(true);
                }

                StartCoroutine(HandleLightDuration(currLightItem));
            }
        }
    }

    IEnumerator HandleLightDuration(int itemType)
    {
        itemInUse = true;
        float itemTime = 0f;
        float startTime = 0f;
        float waitTime = 2f;
        float lightDuration = LightMasterController.lightDurations[itemType];

        spotlight.SetCollider(itemType);
        while (itemTime < lightDuration)
        {
            itemTime += Time.deltaTime;
            yield return null;
        }

        // Reset spotlight collider and light
        if (itemType == (int) LightMasterController.LIGHTS.Flashlight)
        {
            flashlight.TurnOffFlashlight();
            flashlight.SetFlashlightCollider(false);
        }

        spotlight.ResetSpotight();

        while (startTime < waitTime)
        {
            startTime += Time.deltaTime;
            yield return null;
        }
        spotlight.SetCollider(-1);

        itemInUse = false;
    }
    #endregion

    #region inventory_functions
    public void AddLightToInventory(int lightType)
    {
        ++itemList[lightType];
        if (lightType == currLightItem)
        {
            inventory.SetCurrentItemCount(lightType, itemList[lightType]);
        }
    }

    private bool HasItem(int lightType)
    {
        Debug.Assert(lightType < LightMasterController.numLightItems);
        return itemList.ContainsKey(lightType) && itemList[lightType] > 0;
    }

    private void UseLightItem(int lightType)
    {
        Debug.Assert(HasItem(lightType));
        --itemList[lightType];
        inventory.SetCurrentItemCount(lightType, itemList[lightType]);
    }

    private void InitLightItems()
    {
        for (int lightType = 0; lightType < LightMasterController.numLightItems; ++lightType)
        {
            itemList.Add(lightType, 0);
        }

        inventory.SetItemImage(inventory.GetItemSprite(currLightItem), "Small", itemList[currLightItem], 60, 60);
    }

    private void AddWeapon(string weaponName, Sprite weaponSprite)
    {
        if (weaponList.Contains(weaponName))
        {
            if (weaponName == "Gun")
            {
                rangedWeapon.reload();
                if (weaponList[currWeapon] == "Gun")
                {
                    ShowWeaponUI(weaponName, weaponSprite);
                }
            }
            return;
        }

        weaponList.Add(weaponName);
        if (weaponList.Count == 2)
        {
            ShowWeaponUI(weaponName, weaponSprite);
            WeaponAnimator(weaponName);
            armCollider.enabled = true;
            SwitchWeapon();
        }
    }

    private void ShowWeaponUI(string weaponName, Sprite weaponSprite)
    {
        switch(weaponName)
        {
            case "Gun":
                string gunDisplayName = weaponName + " x" + rangedWeapon.getCurrentAmmo();
                Debug.Log(gunDisplayName);
                inventory.SetWeaponImage(weaponSprite, gunDisplayName, 188, 67);
                break;
            case "Pipe":
                inventory.SetWeaponImage(weaponSprite, weaponName, 170, 60);
                break;
            default:
                inventory.SetWeaponImage(null, weaponName, 0, 0);
                break;
        }
    }

    private void SwitchWeapon()
    {
        currWeapon = (currWeapon + 1) % weaponList.Count;
        string currWeaponName = weaponList[currWeapon];
        Sprite weaponSprite = inventory.GetWeaponSprite(currWeaponName);
        ShowWeaponUI(currWeaponName, weaponSprite);

        switch (currWeaponName)
        {
            case "Gun":
                rangedWeapon.gameObject.SetActive(true);
                meleeWeapon.gameObject.SetActive(false);
                armCollider.enabled = true;
                anim.SetBool("hasGun", true);
                anim.SetBool("hasMelee", false);
                break;
            case "Pipe":
                rangedWeapon.gameObject.SetActive(false);
                meleeWeapon.gameObject.SetActive(true);
                armCollider.enabled = false;
                anim.SetBool("hasGun", false);
                anim.SetBool("hasMelee", true);
                break;
            default:
                rangedWeapon.gameObject.SetActive(false);
                meleeWeapon.gameObject.SetActive(false);
                armCollider.enabled = false;
                anim.SetBool("hasGun", false);
                anim.SetBool("hasMelee", false);
                break;
        }
    }
    #endregion

    #region interact_functions
    private void Interact()
    {
        Vector2 currDirection = new Vector2(player.transform.up.x, player.transform.up.y);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(player.position, new Vector2(3f, 3f), 0f, Vector2.zero, 0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Door"))
            {
                hit.transform.GetComponent<DoorController>().OpenDoor(keyList);
            }
            else if (hit.transform.CompareTag("Key"))
            {
                KeyController key = hit.transform.gameObject.GetComponent<KeyController>();

                Debug.Log(key.name);
                if (key.getKeyName().Equals(endDoorKeyName))
                {
                    endDoorKeys++;
                }

                keyList.Add(key);
                hit.transform.GetComponent<KeyController>().PickUpKey();
                Destroy(hit.transform.gameObject);
            } 
            else if (hit.transform.CompareTag("Lantern"))
            {
                LightItemController lantern = hit.transform.gameObject.GetComponent<LightItemController>();
                int lightType = lantern.GetLightType();
                AddLightToInventory(lightType);
                lantern.PickUpLight();
                Destroy(hit.transform.gameObject);
            }
            else if (hit.transform.CompareTag("RangedWeaponPickup"))
            {
                Weapon_Ranged weaponItem = hit.transform.gameObject.GetComponent<Weapon_Ranged>();
                AddWeapon(weaponItem.GetWeaponName(), weaponItem.GetWeaponSprite());
                weaponItem.PickUpWeapon();
            }
            else if (hit.transform.CompareTag("MeleeWeaponPickup"))
            {
                Weapon_Close weaponItem = hit.transform.gameObject.GetComponent<Weapon_Close>();
                AddWeapon(weaponItem.GetWeaponName(), weaponItem.GetWeaponSprite());
                weaponItem.PickUpWeapon();
            }
            else if (hit.transform.CompareTag("Ammo"))
            {
                rangedWeapon.gameObject.GetComponent<Weapon_Ranged>().reload();
                if (weaponList[currWeapon].Equals("Gun")) {
                    inventory.SetAmmoCount(weaponList[currWeapon], rangedWeapon.getCurrentAmmo());
                }
                hit.transform.GetComponent<AmmoController>().PickUpAmmo();
            }
            else if (hit.transform.CompareTag("Bandages"))
            {
                int healamount = hit.transform.GetComponent<BandageController>().getHealAmount();
                hit.transform.GetComponent<BandageController>().PickUpBandages();
                Heal(healamount);
            }
        }
    }
    #endregion

    #region audio_functions
    private void PlayerMoveAudio(bool isMoving, bool isRunning) 
    {
        if (isMoving && isRunning && !audioManager.IsPlaying("PlayerRun"))
        {
            audioManager.Stop("PlayerWalk");
            audioManager.Play("PlayerRun");
        }
        else if (isMoving && !audioManager.IsPlaying("PlayerWalk"))
        {
            audioManager.Stop("PlayerRun");
            audioManager.Play("PlayerWalk");
        }
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

    private void EnableGodMode()
    {
        // Unlimited inventory
        for (int i = 0; i < itemList.Count; ++i)
        {
            itemList[i] = 999;
        }
        inventory.SetCurrentItemCount(0, itemList[0]);

        AddWeapon("Gun", inventory.GetWeaponSprite("Gun"));
        AddWeapon("Pipe", inventory.GetWeaponSprite("Pipe"));
        rangedWeapon.AddAmmo(994);
        SwitchWeapon();
        SwitchWeapon();
    }
    #endregion

    #region scripted_event_functions
    public int GetEndDoorKeysNum()
    {
        return endDoorKeys;
    }
    #endregion
}
