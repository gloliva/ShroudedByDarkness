using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Close : MonoBehaviour
{
    public float xOffset;
    public float yOffset;
    public float animationTime;
    public float stall;
    public float startSwingAngle;
    public float endSwingAngle;
    public int damage;

    private bool attackAnimation;
    private double startTime;
    private Vector3 startAngle;
    private Vector3 endAngle;

    [SerializeField]
    private float attackDelay;

    #region inventory_variables
    [SerializeField]
    [Tooltip("Name of this weapon")]
    private string weaponName;

    [SerializeField]
    [Tooltip("If this weapon is a pickup item, i.e. not attached to the player")]
    private bool pickupWeapon;
    #endregion

    #region component_variables
    private MessageController weaponMessage;
    private ButtonInteractMessageController interactMessage;
    private AudioManager audioManager;
    #endregion

    #region room_variables
    [SerializeField]
    [Tooltip("The room the object starts in")]
    private int startingRoom;

    private int roomCurrentlyIn;

    private bool isInvisible;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        // Set components
        weaponMessage = GameObject.FindObjectOfType<MessageController>();
        interactMessage = GameObject.FindObjectOfType<ButtonInteractMessageController>();
        audioManager = GameObject.FindObjectOfType<AudioManager>();

        // Set weapon variables
        attackAnimation = false;
        if (!pickupWeapon)
        {
            transform.localPosition = new Vector3(xOffset, yOffset, 0);
            startAngle = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, startSwingAngle);
            endAngle = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, endSwingAngle);
        }

        // Set room variables
        roomCurrentlyIn = startingRoom;
        isInvisible = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !attackAnimation && !pickupWeapon)
        {
            audioManager.Play("PipeSwing");
            StartCoroutine(AttackRoutine());
        }
        else if (!pickupWeapon)
        {
            transform.localPosition = new Vector3(xOffset, yOffset, 0);
        }
    }

    IEnumerator AttackRoutine()
    {
        attackAnimation = true;
        yield return new WaitForSeconds(attackDelay);

        float currTime = 0f;
        while (currTime < animationTime)
        {
            transform.localEulerAngles = Vector3.Lerp(startAngle, endAngle, currTime / animationTime);
            float angle = (transform.localEulerAngles.z + 90) * Mathf.PI / 180;
            transform.localPosition = new Vector3(xOffset + Mathf.Cos(angle) * yOffset, yOffset * Mathf.Sin(angle), 0);
            currTime += Time.deltaTime;

            yield return null;
        }

        attackAnimation = false;
        transform.localPosition = new Vector3(xOffset, yOffset, 0);
        transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackAnimation)
        {
            if (collision.gameObject.transform.tag == "Enemy")
            {
                collision.gameObject.transform.GetComponent<EnemyController>().TakeDamage(damage);
            }
            else if (collision.gameObject.transform.tag == "RangedEnemy")
            {
                collision.gameObject.transform.GetComponent<EnemyRangedController>().TakeDamage(damage);
            }
            else if (collision.gameObject.transform.tag == "FinalEnemy")
            {
                collision.gameObject.transform.GetComponent<EnemyController>().TakeDamage(damage);
            }
        }

        if (collision.transform.CompareTag("Player") && pickupWeapon)
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
        if (collision.transform.CompareTag("Player") && pickupWeapon)
        {
            interactMessage.SetMessageActive(false);
        }

        if (collision.gameObject.CompareTag("PlayerLight") && pickupWeapon)
        {
            isInvisible = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    void Swing()
    {
        float state = (float)(Time.time - startTime) / animationTime;
        transform.localEulerAngles = Vector3.Lerp(startAngle, endAngle, state * Mathf.Sqrt(state));
        float angle = (transform.localEulerAngles.z + 90) * Mathf.PI / 180;
        transform.localPosition = new Vector3(xOffset + Mathf.Cos(angle) * yOffset, yOffset * Mathf.Sin(angle), 0);
    }

    #region inventory_functions
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
        audioManager.Play("PickupPipe");
        weaponMessage.ShowItemPickUpMessage(weaponName);
        Destroy(this.gameObject);
    }
    #endregion
}
