using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupEvent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The enemy to spawn")]
    private GameObject enemy;

    private GameObject meleeWeapon;
    private Light weaponLight;
    private bool eventOccurred;

    // Start is called before the first frame update
    void Start()
    {
        eventOccurred = false;
        meleeWeapon = GetComponentInChildren<Weapon_Close>().gameObject;
        weaponLight = GetComponentInChildren<Light>();
        StartCoroutine(StartWeaponEvent());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator StartWeaponEvent()
    {
        yield return new WaitUntil(() => meleeWeapon == null);
        Instantiate(enemy, new Vector3(177, 23, 0), transform.rotation, transform);
        Instantiate(enemy, new Vector3(217, 23, 0), transform.rotation, transform);
        Instantiate(enemy, new Vector3(197, 43, 0), transform.rotation, transform);
        weaponLight.enabled = false;

        EnemyController[] enemies = GetComponentsInChildren<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.AlwaysChasePlayer();
        }
    }
}
