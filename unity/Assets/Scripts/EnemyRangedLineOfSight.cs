using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedLineOfSight : MonoBehaviour
{
    #region tracking_variables
    private EnemyRangedController enemy;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<EnemyRangedController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLight") || collision.gameObject.CompareTag("Player"))
        {
            enemy.playerTransform = collision.GetComponent<Transform>();
            enemy.trackPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLight") || collision.gameObject.CompareTag("Player"))
        {
            enemy.trackPlayer = false;
        }
    }
}
