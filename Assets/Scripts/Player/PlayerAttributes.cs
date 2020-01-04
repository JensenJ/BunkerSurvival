using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    GameObject gameManager = null;
    NetworkUtils netUtils = null;

    [SerializeField] public float health = 100.0f;
    [SerializeField] public float stamina = 100.0f;

    float lastHealth;
    float lastStamina;

    [SerializeField] public float maxHealth = 100.0f;
    [SerializeField] public float maxStamina = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        netUtils = gameManager.GetComponent<NetworkUtils>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checking if health value is the same as last check value
        if (lastHealth != health)
        {
            //Get host
            PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
            if(host != null)
            {
                //run command on host
                host.CmdUpdatePlayerAttributes();
            }
            //set last health equal to new health
            lastHealth = health;
        }
        //Checking if stamina value is the same as last check value
        if(lastStamina != stamina)
        {
            //Get host
            PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
            if(host != null)
            {
                //run command on host
                host.CmdUpdatePlayerAttributes();
            }
            //set last stamina equal to new stamina
            lastStamina = stamina;
        }
    }
}
