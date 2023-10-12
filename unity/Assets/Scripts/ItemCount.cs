using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ItemCount : MonoBehaviour
{
    #region editor_variables
    [SerializeField]
    [Tooltip("The text component for displaying item counts")]
    private Text m_UIText;
    #endregion

    #region item_variables
    private int[] numItems;
    #endregion

    #region singletons
    private static ItemCount ic;
    #endregion

    #region unity_functions
    private void Awake()
    {
        ic = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        numItems = new int[3];
        resetItems();
    }
    #endregion

    #region accessors_and_mutators
    private void resetItems()
    {
        m_UIText.text = "Small: " + 0 + " Medium: " + 0 + " Large: " + 0;
    }

    public void addItem(int itemNum)
    {
        Debug.Assert(itemNum < numItems.Length);
        numItems[itemNum]++;
        m_UIText.text = "Small: " + numItems[0] + " Medium: " + numItems[1] + " Large: " + numItems[2];
    }

    public void removeItem(int itemNum)
    {
        Debug.Assert(itemNum < numItems.Length);
        numItems[itemNum]--;
        m_UIText.text = "Small: " + numItems[0] + " Medium: " + numItems[1] + " Large: " + numItems[2];
    }
    #endregion
}
