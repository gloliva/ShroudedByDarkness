using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLineOfSight : MonoBehaviour
{
    #region tracking_variables
    private EnemyController enemy;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("PlayerLight") || collision.gameObject.CompareTag("Player")) && !enemy.overrideTrackPlayer)
        {
            enemy.playerTransform = collision.GetComponent<Transform>();
            enemy.isMoving = true;
            enemy.trackPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("PlayerLight") || collision.gameObject.CompareTag("Player")) && !enemy.overrideTrackPlayer)
        {
            enemy.trackPlayer = false;
        }
    }
}
