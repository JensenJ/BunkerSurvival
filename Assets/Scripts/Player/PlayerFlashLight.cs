using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerFlashLight : NetworkBehaviour
{
    [SerializeField] GameObject flashLight = null;
    [SerializeField] public bool flashLightStatus = false;

    [SerializeField] public float flashLightBattery = 100.0f;
    [SerializeField] public float flashLightMaxBattery = 100.0f;

    float lastBattery = 0.0f;
    float lastMaxBattery = 0.0f;

    [SerializeField] public float flashLightRechargeRate = 0.05f;
    [SerializeField] public float flashLightDrainRate = 0.1f;

    NetworkUtils netUtils = null;
    GameObject gameManager = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (flashLight == null)
        {
            flashLight = transform.GetChild(0).GetChild(0).gameObject;
        }
        ToggleFlashLight(flashLightStatus);

        gameManager = GameObject.FindGameObjectWithTag("GameController");
        netUtils = gameManager.GetComponent<NetworkUtils>();
    }

    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        //Checking if battery value is the same as last check value
        if (lastBattery != flashLightBattery)
        {
            //Clamp value
            flashLightBattery = Mathf.Clamp(flashLightBattery, 0.0f, flashLightMaxBattery);
            UpdateFlashLightStatus();
            //set last battery equal to new battery
            lastBattery = flashLightBattery;
        }

        //Checking if battery value is the same as last check value
        if (lastMaxBattery != flashLightMaxBattery)
        {
            //Clamp value
            flashLightMaxBattery = Mathf.Clamp(flashLightMaxBattery, 0.0f, flashLightMaxBattery);
            UpdateFlashLightStatus();
            //set last max battery equal to new max battery
            lastMaxBattery = flashLightMaxBattery;
        }

        //Flashlight draining logic
        //If flashlight is on, drain it
        if(flashLightStatus == true)
        {
            DrainFlashLight(flashLightDrainRate);
            if(flashLightBattery <= 0.0f)
            {
                ToggleFlashLight(false);
            }
        }
        //Otherwise, recharge it
        else
        {
            if(flashLightBattery < flashLightMaxBattery)
            {
                RechargeFlashLight(flashLightRechargeRate);
            }
        }
    }

    //Function to update flash light status across the network.
    public void UpdateFlashLightStatus()
    {
        PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
        if (host != null)
        {
            host.CmdUpdateFlashLightStatus(flashLightStatus, flashLightBattery, flashLightMaxBattery);
        }
    }

    //Function to toggle flash light
    public void ToggleFlashLight(bool status)
    {
        flashLightStatus = status;
        flashLight.SetActive(status);
    }

    //Function to recharge flash light
    public void RechargeFlashLight(float amount)
    {
        flashLightBattery += amount;
    }

    //Function to drain flash light
    public void DrainFlashLight(float amount)
    {
        flashLightBattery -= amount;
    }

    //SETTERS
    public void SetFlashLightCharge(float amount)
    {
        flashLightBattery = amount;
    }
    public void SetMaxFlashLightCharge(float amount)
    {
        flashLightMaxBattery = amount;
    }


    //GETTERS
    public float GetFlashLightCharge()
    {
        return flashLightBattery;
    }

    public float GetMaxFlashLightCharge()
    {
        return flashLightMaxBattery;
    }
}
