using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Max stamina value. Must be the same as player stamina")]
    private float maxStamina;

    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();   
    }

    public void SetStaminaVal(float currStamina)
    {
        slider.value = (currStamina / maxStamina);
    }
}
