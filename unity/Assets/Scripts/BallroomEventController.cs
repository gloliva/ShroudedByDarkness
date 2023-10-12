using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallroomEventController : MonoBehaviour
{

    #region inspector_fields
    [SerializeField]
    [Tooltip("Enemy to spawn")]
    private GameObject enemy;

    [SerializeField]
    [Tooltip("Points in global space where enemies spawn from")]
    private Vector3[] spawnPositions;

    [SerializeField]
    [Tooltip("Number of enemies to spawn")]
    private int numEnemies;

    [SerializeField]
    [Tooltip("Number of seconds before event")]
    private float eventStartDelay;

    [SerializeField]
    [Tooltip("Time between enemy spawns")]
    private float enemySpawnRate;
    #endregion

    #region private_fields
    private bool eventStart;
    private DoorController[] doors;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        eventStart = false;
        doors = GetComponentsInChildren<DoorController>(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !eventStart)
        {
            StartCoroutine(StartEvent());
            StartCoroutine(EnemySpawner());
        }
    }

    #region event_functions
    IEnumerator StartEvent()
    {
        // Lock doors
        Debug.Log(doors.Length);
        foreach (DoorController door in doors)
        {
            door.gameObject.SetActive(true);
        }

        float currTime = 0f;
        while (currTime < eventStartDelay)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        eventStart = true;
    }

    IEnumerator EnemySpawner()
    {
        yield return new WaitUntil(() => eventStart);
        int enemyPos = 0;

        for (int i = 0; i < numEnemies; ++i)
        {
            GameObject enemyObject = Instantiate(enemy, spawnPositions[enemyPos], transform.rotation, transform);
            enemyPos = (enemyPos + 1) % spawnPositions.Length;
            EnemyController spawnedEnemy = enemyObject.GetComponent<EnemyController>();
            spawnedEnemy.SetMaxHealth(1);
            spawnedEnemy.AlwaysChasePlayer();
            yield return new WaitForSeconds(enemySpawnRate);
        }

        foreach (DoorController door in doors)
        {
            Destroy(door.gameObject);
        }
    }
    #endregion
}
