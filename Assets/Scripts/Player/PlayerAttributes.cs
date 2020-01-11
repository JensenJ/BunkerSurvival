using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAttributes : NetworkBehaviour
{
    GameObject gameManager = null;
    NetworkUtils netUtils = null;

    [SerializeField] float health = 100.0f;
    [SerializeField] float stamina = 100.0f;

    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] float maxStamina = 100.0f;

    [SerializeField] public static float baseHealth = 100.0f;
    [SerializeField] public static float baseStamina = 100.0f;

    float lastHealth = 0;
    float lastMaxHealth = 0;
    float lastStamina = 0;
    float lastMaxStamina = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        netUtils = gameManager.GetComponent<NetworkUtils>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        //Checking if health value is the same as last check value
        if (lastHealth != health)
        {
            //Clamp value
            health = Mathf.Clamp(health, 0.0f, maxHealth);
            UpdateAttributes();
            //set last health equal to new health
            lastHealth = health;
        }

        //Checking if max health value is the same as last check value
        if(lastMaxHealth != maxHealth)
        {
            maxHealth = Mathf.Clamp(maxHealth, 0.0f, maxHealth);
            UpdateAttributes();
            //Set last max health equal to new max health
            lastMaxHealth = maxHealth;
        }

        //Checking if stamina value is the same as last check value
        if (lastStamina != stamina)
        {
            //Clamp value
            stamina = Mathf.Clamp(stamina, 0.0f, maxStamina);
            UpdateAttributes();
            //set last stamina equal to new stamina
            lastStamina = stamina;
        }

        //Checking if max stamina value is the same as last check value
        if(lastMaxStamina != maxStamina)
        {
            maxStamina = Mathf.Clamp(maxStamina, 0.0f, maxStamina);
            UpdateAttributes();
            //Set last max stamina equal to new max stamina
            lastMaxStamina = maxStamina;
        }
    }

    //Function to apply attributes across the network
    void UpdateAttributes()
    {
        //Get host
        PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
        if (host != null)
        {
            //run command on host
            host.CmdUpdatePlayerAttributes(health, maxHealth, stamina, maxStamina);
        }
    }

    //Functions for changing attribute values

    public void DamageHealth(float amount)
    {
        health -= amount;
    }

    public void HealHealth(float amount)
    {
        health += amount;
    }

    public void DamageStamina(float amount)
    {
        stamina -= amount;
    }
    
    public void HealStamina(float amount)
    {
        stamina += amount;
    }

    //SETTERS
    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }
    public void SetStamina(float newStamina)
    {
        stamina = newStamina;
    }
    public void SetMaxStamina(float newMaxStamina)
    {
        maxStamina = newMaxStamina;
    }

    //GETTERS
    public float GetHealth()
    {
        return health;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public float GetStamina()
    {
        return stamina;
    }
    public float GetMaxStamina()
    {
        return maxStamina;
    }
    public float GetBaseHealth()
    {
        return baseHealth;
    }
    public float GetBaseStamina()
    {
        return baseStamina;
    }
}
