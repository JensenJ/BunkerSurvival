using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAttributes : NetworkBehaviour
{
    GameObject gameManager = null;
    NetworkUtils netUtils = null;

    [SerializeField] public float health = 100.0f;
    [SerializeField] public float stamina = 100.0f;

    [SerializeField] public float maxHealth = 100.0f;
    [SerializeField] public float maxStamina = 100.0f;

    float lastHealth = 0;
    float lastStamina = 0;

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

            //Get host
            PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
            if (host != null)
            {
                //run command on host
                host.CmdUpdatePlayerAttributes(health, maxHealth, stamina, maxStamina);
            }

            //set last health equal to new health
            lastHealth = health;
        }
        //Checking if stamina value is the same as last check value
        if (lastStamina != stamina)
        {
            //Clamp value
            stamina = Mathf.Clamp(stamina, 0.0f, maxStamina);

            //Get host
            PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
            if (host != null)
            {
                //run command on host
                host.CmdUpdatePlayerAttributes(health, maxHealth, stamina, maxStamina);
            }

            //set last stamina equal to new stamina
            lastStamina = stamina;
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
}
