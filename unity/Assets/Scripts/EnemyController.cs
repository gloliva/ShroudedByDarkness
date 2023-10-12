using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
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
    public bool isMoving;

    [HideInInspector]
    public bool trackPlayer;
    
    // Search for the player after losing line of sight
    private bool isSearching;

    // Chases the player regardless of line of sight, used in scripted events
    [HideInInspector]
    public bool overrideTrackPlayer;

    // If the enemy is in contact with the player
    private bool touchingPlayer;

    [HideInInspector]
    public Transform playerTransform;

    [SerializeField]
    [Tooltip("Move speed for walking/searching")]
    private float moveSpeed;

    [SerializeField]
    [Tooltip("Move speed when chasing the player")]
    private float chasePlayerMoveSpeed;

    [SerializeField]
    [Tooltip("Time in seconds for how long the enemy will search for the player")]
    private float searchTotalTime;

    [SerializeField]
    [Tooltip("Time in seconds for how long an enemy will pause after getting damaged")]
    private float hitPauseTotalTime;

    private float hitPauseCurrTime;
    #endregion

    #region health_variables
    [SerializeField]
    [Tooltip("Enemy max health")]
    private int maxHealth;

    private int currHealth;

    private bool isDead;

    // If a melee enemy is hit, it will pause briefly before chasing player again
    private bool isPaused;

    [SerializeField]
    [Tooltip("Percentage that an enemy will be stunned when taking damage\n" +
        "Must be between 0 and 1, inclusive.")]
    private float stunPercentage;

    [SerializeField]
    [Tooltip("Dead enemy sprite")]
    private Sprite deadEnemy; 
    #endregion

    #region attack_variables
    [SerializeField]
    [Tooltip("Attack damage")]
    private int attackDamage;

    [SerializeField]
    [Tooltip("Time in seconds between attacks")]
    private float attackSpeed;

    private float attackTimer;

    [SerializeField]
    [Tooltip("The x and y of the enemy hitbox")]
    private Vector2 attackHitbox;

    [SerializeField]
    [Tooltip("Time in seconds to calculate the hitbox during an attack")]
    private float hitboxTiming;

    private bool isAttacking;
    #endregion

    #region audio_variables
    private AudioManager audioManager;
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
        hitPauseCurrTime = 0f;
        overrideTrackPlayer = false;
        touchingPlayer = false;

        // Set health variables
        currHealth = maxHealth;
        isPaused = false;
        isDead = false;

        // Set attack variables
        isAttacking = false;
        attackTimer = 0f;

        // Set room variables
        roomCurrentlyIn = startingRoom;
        isInvisible = false;

        // Assert health variable constraints
        Debug.Assert(stunPercentage >= 0f && stunPercentage <= 1f);

        // Assert attack variable constraints
        Debug.Assert(attackSpeed > 0);

    }

    private void Start()
    {
        // Set audio variables, needs to be in Start, not Awake
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer > 0 && !isDead && !isInvisible)
        {
            attackTimer -= Time.deltaTime;
        }

        if (!isDead && !isInvisible)
        {
            EnemyMoveAudio();
        }
    }

    private void FixedUpdate()
    {
        if (isMoving && !isAttacking && !isDead && !isInvisible)
        {
            Move();
        }

        if (playerTransform != null && !isDead && !isInvisible)
        {
            Vector3 vectorToTarget = playerTransform.position - transform.position;
            // - 90 degrees to adjust for it starting forward at a 90 degree angle
            float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5);
        }

        if (isDead)
        {
            enemy.velocity = Vector2.zero;
            enemy.angularVelocity = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !trackPlayer && isMoving && !isDead)
        {
            Vector2 direction = collision.transform.position - transform.position;
            enemy.velocity = direction.normalized * moveSpeed;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")
            && enemy_type == (int) EnemyMasterController.ENEMY_TYPES.Melee
            && !isDead)
        {
            isMoving = false;
            if (attackTimer <= 0)
            {
                Attack();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")
            && enemy_type == (int)EnemyMasterController.ENEMY_TYPES.Melee
            && !isDead)
        {
            isMoving = true;
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
            } else
            {
                isInvisible = true;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
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

    #region movement_functions
    public void AlwaysChasePlayer()
    {
        this.playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        this.overrideTrackPlayer = true;
        this.isMoving = true;
    }

    // Move the enemy 
    private void Move()
    {
        // Enemy is scripted to follow player
        if (overrideTrackPlayer && playerTransform != null)
        {
            Chase();
        }
        // Enemy has seen the player
        else if (trackPlayer)
        {
            Chase();
        }
        else if (!trackPlayer && !isSearching && playerTransform != null)
        {
            isSearching = true;
            enemy.velocity = enemy.velocity.normalized * moveSpeed;
            StartCoroutine(Search());
        } else if (!isSearching)
        {
            enemy.velocity = Vector2.zero;
            return;
        }
    }

    /* Enemies will chase the player when the player's light 
     * source enters the enemy's line of sight */
    private void Chase()
    {
        FollowPlayer(chasePlayerMoveSpeed);
    }

    /* When the player has attracted an enemy but left
     * their line of sight */
    private IEnumerator Search()
    {
        Debug.Assert(searchTotalTime != 0);
        float currTime = 0f;
        List<float> timeThresholds = new List<float>();
        FillList(timeThresholds);
        float changeDirThresh = timeThresholds[0];
        timeThresholds.RemoveAt(0);


        while (currTime / searchTotalTime < 1 && !trackPlayer)
        {
            if (currTime > changeDirThresh)
            {
                changeDirThresh = timeThresholds[0];
                timeThresholds.RemoveAt(0);
                FollowPlayer(moveSpeed);
            }
            currTime += Time.deltaTime;
            yield return null;
        }

        if (!trackPlayer) {
            isMoving = false;
            playerTransform = null;
            enemy.velocity = Vector2.zero;
            enemy.rotation = 0;
        }

        isSearching = false;
        yield return null;
    }

    private void FillList(List<float> lst)
    {
        float multiple = searchTotalTime / 4;
        for (int i = 1; i < 5; ++i)
        {
            lst.Add(multiple * i);
        }
    }


    private void FollowPlayer(float moveSpeed)
    {
        // Enemy has not been stunned
        if (!isPaused)
        {
            // Calculate enemy movement vecotr: Player Position - Enemy Position = Direction of player relative to enemy
            Vector2 direction = playerTransform.position - transform.position;
            enemy.velocity = direction.normalized * moveSpeed;
        }
        // Enemy has been stunned and is not moving
        else
        {
            if (hitPauseCurrTime >= hitPauseTotalTime)
            {
                isPaused = false;
                hitPauseCurrTime = 0f;
            } else
            {
                hitPauseCurrTime += Time.deltaTime;
            }
        }
    }
    #endregion

    #region health_functions
    // Used to set enemy health in events
    public void SetMaxHealth(int health)
    {
        maxHealth = currHealth = health;
    }

    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            // Start damage indicator
            StartCoroutine(TakeDamageIndicator());

            // Handle health and death
            currHealth -= damage;
            if (currHealth <= 0)
            {
                Die();
                return;
            }

            // Chance to pause melee enemy briefly when taking damage
            if (!isPaused && Random.value < stunPercentage && enemy_type == (int)EnemyMasterController.ENEMY_TYPES.Melee)
            {
                isPaused = true;
                enemy.velocity = Vector2.zero;
            }
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
        isDead = true;
        enemy.velocity = Vector2.zero;
        enemy.angularVelocity = 0f;
        gameObject.GetComponent<Animator>().enabled = false;
        enemy.GetComponent<CapsuleCollider2D>().enabled = false;
        audioManager.Stop("EnemyMove");
        audioManager.Play("EnemyDeath");
        gameObject.GetComponent<SpriteRenderer>().sprite = deadEnemy;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    public bool EnemyIsDead()
    {
        return isDead;
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
        yield return new WaitForSeconds(hitboxTiming);

        // Create hitbox
        audioManager.Play("EnemyAttack");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(enemy.position + new Vector2(enemy.transform.forward.x, enemy.transform.forward.y), attackHitbox, 0f, Vector2.zero, 0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerController>().TakeDamage(attackDamage);
            }
        }

        isAttacking = false;
    }
    #endregion

    #region audio_functions
    private void EnemyMoveAudio()
    {
        if (playerTransform != null)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - playerTransform.position.x, 2) 
                                        + Mathf.Pow(transform.position.y - playerTransform.position.y, 2));
            distance = Mathf.Min(distance, 20);
            float volume = 1 - (distance / 20);
            if (!audioManager.IsPlaying("EnemyMove"))
            {
                audioManager.Play("EnemyMove", volume);
            }
            else
            {
                audioManager.SetVolume("EnemyMove", volume);
            }
        }
    }
    #endregion

    #region room_functions
    public void SetCurrentRoom(int room)
    {
        roomCurrentlyIn = room;
    }
    #endregion
}
