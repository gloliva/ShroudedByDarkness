using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Ranged : MonoBehaviour
{
    #region component_variables
    public Rigidbody2D bullet;
    private MessageController weaponMessage;
    private ButtonInteractMessageController interactMessage;
    private AudioManager audioManager;
    #endregion

    #region weapon_variables
    [SerializeField]
    [Tooltip("Name of this weapon")]
    private string weaponName;

    [SerializeField]
    [Tooltip("If this weapon is a pickup item, i.e. not attached to the player")]
    private bool pickupWeapon;

    public float speed;
    public float rotation;
    public int reloadAmount;
    public int startAmmo;
    private int currentAmmo;
    #endregion

    #region room_variables
    [SerializeField]
    [Tooltip("The room the object starts in")]
    private int startingRoom;

    private int roomCurrentlyIn;

    private bool isInvisible;
    #endregion

    #region unity_functions
    // Start is called before the first frame update
    void Awake()
    {
        weaponMessage = GameObject.FindObjectOfType<MessageController>();
        interactMessage = GameObject.FindObjectOfType<ButtonInteractMessageController>();
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        currentAmmo = startAmmo;

        // Set room variables
        roomCurrentlyIn = startingRoom;
        isInvisible = false;
    }

    private void Start()
    {
        
    }

    public void Attack()
    {
        if (!pickupWeapon && currentAmmo > 0)
        {
            audioManager.Play("PistolShot");
            Shoot();
            currentAmmo--;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            interactMessage.SetInteractMessage("E to pick up");
            interactMessage.SetMessageActive(true);
        }

        if (collision.gameObject.CompareTag("PlayerLight") && pickupWeapon)
        {
            if (FindObjectOfType<LightRoomTriggerMaster>().GetPlayerRoom() != roomCurrentlyIn)
            {
                isInvisible = true;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLight") && pickupWeapon)
        {
            if (FindObjectOfType<LightRoomTriggerMaster>().GetPlayerRoom() == roomCurrentlyIn)
            {
                isInvisible = false;
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            interactMessage.SetMessageActive(false);
        }

        if (collision.gameObject.CompareTag("PlayerLight") && pickupWeapon)
        {
            isInvisible = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    #endregion

    #region attack_functions
    void Shoot()
    {
        Transform parent = GetComponentInParent<Transform>();
        Rigidbody2D projectile = Instantiate(bullet, parent.position, Quaternion.identity);
        //projectile.transform.SetParent(transform);
        projectile.transform.localScale = new Vector3(3f, 3f, 1);
        float x = 0;
        float y = 1;
        float angle = parent.rotation.eulerAngles.z * Mathf.PI / 180;
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);
        Vector3 direction = new Vector3(cos * x - sin * y, sin * x + cos * y, 0);
        projectile.velocity = direction * speed;
        //projectile.velocity = transform.forward * speed;
    }
    #endregion

    #region inventory_functions
    public int getCurrentAmmo()
    {
        return currentAmmo;
    }

    public string GetWeaponName()
    {
        return weaponName;
    }

    public Sprite GetWeaponSprite()
    {
        return gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    public void PickUpWeapon()
    {
        weaponMessage.ShowItemPickUpMessage(weaponName);
        audioManager.Play("PistolReload");
        Destroy(this.gameObject);
    }

    // Add fixed amount of ammo
    public void reload()
    {
        audioManager.Play("PistolReload");
        currentAmmo += reloadAmount;
    }

    // Add numBullets amount of ammo
    public void AddAmmo(int numBullets)
    {
        currentAmmo += numBullets;
    }
    #endregion
}
