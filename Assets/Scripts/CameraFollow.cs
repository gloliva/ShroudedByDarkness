using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject player;
    //Distance away from camera
    public float cameraDistance;

    void Awake()
    {
        player = GameObject.Find("testplayer");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = player.transform.position;
        temp.z = temp.z - cameraDistance;
        // Assign value to Camera position
        transform.position = temp;
    }
}
