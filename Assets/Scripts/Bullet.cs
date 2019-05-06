using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    [Tooltip("bullet damage")]
    private int damage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.transform.GetComponent<EnemyController>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("FinalEnemy"))
        {
            collision.gameObject.transform.GetComponent<EnemyController>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("RangedEnemy"))
        {
            collision.gameObject.transform.GetComponent<EnemyRangedController>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }
}
