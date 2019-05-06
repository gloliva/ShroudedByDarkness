using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickupEvent : MonoBehaviour
{
    #region inspector_fields
    [SerializeField]
    [Tooltip("The enemy to spawn")]
    private GameObject enemy;

    [SerializeField]
    [Tooltip("The key to start the event")]
    private GameObject key;

    [SerializeField]
    [Tooltip("Points in global space where enemies spawn from")]
    private Vector3[] spawnPositions;

    [SerializeField]
    [Tooltip("Number of enemies to spawn")]
    private int numEnemies;

    [SerializeField]
    [Tooltip("Time between enemy spawns")]
    private float enemySpawnRate;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemySpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EnemySpawnRoutine()
    {
        yield return new WaitUntil(() => key == null);

        int enemyPos = 0;

        for (int i = 0; i < numEnemies; ++i)
        {
            GameObject enemyObject = Instantiate(enemy, spawnPositions[enemyPos], transform.rotation, transform);
            enemyPos = (enemyPos + 1) % spawnPositions.Length;
            EnemyController spawnedEnemy = enemyObject.GetComponent<EnemyController>();
            spawnedEnemy.AlwaysChasePlayer();
            yield return new WaitForSeconds(enemySpawnRate);
        }
    }
}
