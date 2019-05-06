using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAreaController : MonoBehaviour {

    private Transform finalEnemy;
    private Light enemyLight;
    private float time;
    private bool endStart;

    public GameObject finalEnemyObject;
    public Sprite deadPerson;
    public Vector2 enemyScale;
    public GameObject camera;

    #region endAnimations
    private bool moveCam;
    private bool decreaseLight;
    private bool turnOnDirectional;
    #endregion

    private void Awake()
    {
        moveCam = false;
        decreaseLight = false;
        turnOnDirectional = false;
        finalEnemy = finalEnemyObject.transform;
        //camera = GameObject.FindGameObjectWithTag("VCam");
        enemyLight = finalEnemyObject.GetComponentInChildren<Light>();
        time = 0;
        endStart = false;
    }

    private void Update()
    {
        if (moveCam)
        {
            Vector3 finalCamPos = new Vector3(finalEnemy.position.x, finalEnemy.position.y, Camera.main.transform.position.z);
            time += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, finalCamPos, time / 500);
            Debug.Log("camera " + Camera.main.transform.position);
            Debug.Log("enemy " + finalCamPos);
            if ((Camera.main.transform.position - finalCamPos).magnitude < 0.5)
            {
                moveCam = false;
                decreaseLight = true;
                time = 0;
            }
        }
        else if (decreaseLight)
        {
            time += Time.deltaTime;
            enemyLight.intensity = Mathf.Lerp(8, 0, time / 5);
            if (enemyLight.intensity == 0)
            {
                GameObject.FindGameObjectWithTag("FinalEnemy").GetComponent<SpriteRenderer>().sprite = deadPerson;
                GameObject.FindGameObjectWithTag("FinalEnemy").transform.localScale = enemyScale;
                GameObject.FindGameObjectWithTag("FinalEnemy").GetComponent<SpriteRenderer>().sortingOrder = 0;
                finalEnemy.localScale = new Vector3(5, 5, 1);
                //GameObject.FindGameObjectWithTag("DirectionalLight").SetActive(true);
                time = 0;
                decreaseLight = false;
                turnOnDirectional = true;
            }
        }
        else if (turnOnDirectional)
        {
            time += Time.deltaTime;
            enemyLight.intensity = Mathf.Lerp(0, 8, time / 5);
            if (enemyLight.intensity == 8)
            {
                End();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().freeze();
            camera.SetActive(false);
            moveCam = true;
        }
    }

    private void End()
    {
        if (!endStart)
        {
            endStart = true;
            StartCoroutine(EndRoutine());
        }
    }

    IEnumerator EndRoutine()
    {
        float currTime = 0f;
        while (currTime < 1f)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        GameObject.FindObjectOfType<AudioManager>().Play("PistolShot");
        GameObject.FindObjectOfType<AudioManager>().Play("PlayerDeath");

        currTime = 0f;
        while (currTime < 1f)
        {
            currTime += Time.deltaTime;
            yield return null;
        }

        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        gm.GoToEndScene();
    }
}
