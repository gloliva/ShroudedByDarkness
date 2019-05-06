using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightItemController : MonoBehaviour
{
    #region light_variables
    [SerializeField]
    [Tooltip("This item's light type")]
    private int lightType;
    #endregion

    #region unity_functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().AddLightToInventory(lightType);
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region accessors
    public int GetLightType()
    {
        return lightType;
    }
    #endregion
}
