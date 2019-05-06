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

    private bool isSearching;

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
    #endregion

    #region health_variables
    [SerializeField]
    [Tooltip("Enemy max health")]
    private float maxHealth;
    private float currHealth;
    #endregion

    #region attack_variables
    #endregion

    #region unity_functions
    // Start is called before the first frame update
    void Start()
    {
        // Set component variables
        enemy = GetComponent<Rigidbody2D>();
        lineOfSight = GetComponentInChildren<CircleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !trackPlayer && isMoving)
        {
            Vector2 direction = collision.transform.position - transform.position;
            Debug.Log(direction);
            enemy.velocity = direction.normalized * moveSpeed;
        }
    }
    #endregion

    #region movement_functions
    // Move the enemy 
    private void Move()
    {
        // Enemy has seen the player
        if (trackPlayer)
        {
            Chase();
        }
        else if (!trackPlayer && !isSearching && playerTransform != null)
        {
            isSearching = true;
            enemy.velocity = enemy.velocity.normalized * moveSpeed;
            StartCoroutine(Search());
        }
    }

    /* Enemies will chase the player when the player's light 
     * source enters the enemy's line of sight */
    private void Chase()
    {
        // Calculate enemy movement vecotr: Player Position - Enemy Position = Direction of player relative to enemy
        Vector2 direction = playerTransform.position - transform.position;
        enemy.velocity = direction.normalized * chasePlayerMoveSpeed;
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
                RandomDirection();
            }
            currTime += Time.deltaTime;
            yield return null;
        }

        if (!trackPlayer) {
            isMoving = false;
            playerTransform = null;
            enemy.velocity = Vector2.zero;
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


    private void RandomDirection()
    {
        enemy.velocity = new Vector2(Random.value, Random.value) * moveSpeed;
    }
    #endregion

    #region health_functions
    public void TakeDamage(float damage)
    {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            Die();
        }
    }

    private void Heal(float health)
    {
        currHealth = Mathf.Min(currHealth + health, maxHealth);
    }

    private void Die()
    {

    }
    #endregion
}
