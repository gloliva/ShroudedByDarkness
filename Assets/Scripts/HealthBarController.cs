using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("A list of each individual health bar object")]
    private GameObject[] healthBars;

    private int currHealth;
    private int currPosition;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = healthBars.Length;
        currPosition = healthBars.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DecreaseHealth(int health)
    {
        for (int i = 0; i < currHealth - health; ++i)
        {
            RemoveBar();
        }

        currHealth = health;
    }

    public void IncreaseHealth(int health)
    {
        for (int i = 0; i < health - currHealth; ++i)
        {
            AddBar();
        }

        currHealth = health;
    }

    private void RemoveBar()
    {
        if (currPosition >= 0)
        {
            healthBars[currPosition--].SetActive(false);
        }
    }

    private void AddBar()
    {
        Debug.Assert(currPosition < healthBars.Length);
        healthBars[++currPosition].SetActive(true);
    }
}
