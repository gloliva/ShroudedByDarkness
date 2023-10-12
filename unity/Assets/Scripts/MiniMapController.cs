using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("")]
    private Image[] roomImages;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetAllRoomsInactive()
    {
        foreach(Image image in roomImages)
        {
            image.enabled = false;
        }
    }
}
