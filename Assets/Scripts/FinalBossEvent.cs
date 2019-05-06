using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossEvent : MonoBehaviour
{
    #region inspector_fields
    [SerializeField]
    [Tooltip("Final boss game object")]
    private GameObject finalBoss;

    [SerializeField]
    [Tooltip("Exit door game object")]
    private GameObject exitDoor;
    #endregion

    #region private_fields
    private bool eventStart;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        eventStart = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !eventStart)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            Debug.Log("End key num: " + player.GetEndDoorKeysNum());
            if (player.GetEndDoorKeysNum() == 2)
            {
                eventStart = true;
                finalBoss.gameObject.SetActive(true);
                finalBoss.GetComponent<EnemyController>().AlwaysChasePlayer();
                StartCoroutine(OpenExitDoor());
            }
        }
    }

    IEnumerator OpenExitDoor()
    {
        EnemyController finalBossController = finalBoss.GetComponent<EnemyController>();
        yield return new WaitUntil(() => finalBossController.EnemyIsDead());

        DoorController doorController = exitDoor.GetComponent<DoorController>();
        doorController.OpenDoorOverride();
    }
}
