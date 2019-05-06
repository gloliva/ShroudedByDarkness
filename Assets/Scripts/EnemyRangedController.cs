using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedController : MonoBehaviour
{
    #region enemy_variables
    [SerializeField]
    [Tooltip("Enemy Type\n" +
        "Base: 0\n" +
        "Melee: 1\n" +
        "Ranged: 2\n" +
        "Hybrid: 3")]
    private int enemy_type;
    private Rigidbody2D enemy;
    private CircleCollider2D lineOfSight;
    #endregion

    #region movement_variables

    [HideInInspector]
    public Transform playerTransform;

    [HideInInspector]
    public bool trackPlayer;

    #endregion

    #region health_variables
    [SerializeField]
    [Tooltip("Enemy max health")]
    private int maxHealth;

    private int currHealth;
    #endregion

    #region attackVariables
    public Rigidbody2D projectile;

    [SerializeField]
    [Tooltip("speed of projectile")]
    private float projectileSpeed;

    [SerializeField]
    [Tooltip("Attack damage")]
    private int attackDamage;

    [SerializeField]
    [Tooltip("Time in seconds between attacks")]
    private float attackSpeed;

    private float attackTimer;

    private bool isAttacking;
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
        // Set component variables
        enemy = GetComponent<Rigidbody2D>();
        lineOfSight = GetComponentInChildren<CircleCollider2D>();

        // Set movement variables
        playerTransform = null;

        // Set health variables
        currHealth = maxHealth;

        // Set attack variables
        isAttacking = false;
        attackTimer = 0f;

        // Set room variables
        roomCurrentlyIn = startingRoom;
        isInvisible = false;

        // Assert attack variable constraints
        Debug.Assert(attackSpeed > 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer > 0 && !isInvisible)
        {
            attackTimer -= Time.deltaTime;
        }

        if (attackTimer <=0 && this.trackPlayer && !isInvisible)
        {
            Debug.Log("pew");
            Attack();
        }
    }

    private void FixedUpdate()
    {

        if (playerTransform != null)
        {
            Vector3 vectorToTarget = playerTransform.position - transform.position;
            // - 90 degrees to adjust for it starting forward at a 90 degree angle
            float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLight"))
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
        if (collision.gameObject.CompareTag("PlayerLight"))
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
        if (collision.gameObject.CompareTag("PlayerLight"))
        {
            isInvisible = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    #endregion

    #region health_functions
    public void TakeDamage(int damage)
    {
        // Start damage indicator
        StartCoroutine(TakeDamageIndicator());

        // Handle health and death
        currHealth -= damage;
        if (currHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator TakeDamageIndicator()
    {
        float currTime = 0f;
        float endTime = 0.1f;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = Color.red;

        while (currTime < endTime)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        sr.color = Color.white;
    }

    private void Heal(int health)
    {
        currHealth = Mathf.Min(currHealth + health, maxHealth);
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
    #endregion

    #region attack_functions
    private void Attack()
    {
        StartCoroutine(AttackRoutine());
        attackTimer = attackSpeed;
    }

    IEnumerator AttackRoutine()
    {
        // Pause movement and freeze enemy for duration of the attack
        isAttacking = true;
        enemy.velocity = Vector2.zero;

        // Pause briefly before calculating hitbox
        yield return new WaitForSeconds(attackSpeed);

        Fire();

        isAttacking = false;
    }

    private void Fire()
    {
        Rigidbody2D p = Instantiate(projectile, transform.position, Quaternion.identity);
        //projectile.transform.SetParent(transform);
        p.transform.localScale = new Vector3(8f, 8f, 1);
        float x = 0;
        float y = 1;
        float offset = Random.Range(-10, 10);
        float angle = (transform.rotation.eulerAngles.z + offset)* Mathf.PI / 180;
        
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);
        Vector3 direction = new Vector3(cos * x - sin * y, sin * x + cos * y, 0);
        p.velocity = direction * projectileSpeed;
    }
    #endregion
}
