using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    public float currentHealth;
    public float maxHealth;

    public float currentCalories;
    public float maxCalories;

    float distanceTraveled = 0;
    Vector3 lastPosition;

    public GameObject playerBody;

    public float currentHydrationPrecent;
    public float maxHydrationPrecent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydrationPrecent = maxHydrationPrecent;

        StartCoroutine(decreaseHydration());
    }

    IEnumerator decreaseHydration()
    {
        while (true)
        {
            currentHydrationPrecent -= 1;
            yield return new WaitForSeconds(10);
        }
    }

    void Update()
    {
        distanceTraveled += Vector3.Distance(playerBody.transform.position, lastPosition);
        lastPosition = playerBody.transform.position;

        if(distanceTraveled >= 5)
        {
            distanceTraveled = 0;
            currentCalories -= 1;
        }

        //testing health decrease
        if (Input.GetKeyDown(KeyCode.M))
        {
            currentHealth -= 10;
        }
    }

    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public void setCalories(float newCalories)
    {
        currentCalories = newCalories;
    }

    public void setHydration(float newHydration)
    {
        currentHydrationPrecent = newHydration;
    }
}
