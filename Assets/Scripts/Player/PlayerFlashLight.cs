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

    [SerializeField] public static float baseFlashLightBattery = 100.0f;

    float lastBattery = 0.0f;
    float lastMaxBattery = 0.0f;

    [SerializeField] public float flashLightRechargeRate = 5f;
    [SerializeField] public float flashLightDrainRate = 3f;

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
            UpdateFlashLightBattery();
            //set last battery equal to new battery
            lastBattery = flashLightBattery;
        }

        //Checking if battery value is the same as last check value
        if (lastMaxBattery != flashLightMaxBattery)
        {
            //Clamp value
            flashLightMaxBattery = Mathf.Clamp(flashLightMaxBattery, 0.0f, flashLightMaxBattery);
            UpdateFlashLightBattery();
            //set last max battery equal to new max battery
            lastMaxBattery = flashLightMaxBattery;
        }

        //Flashlight draining logic
        //If flashlight is on, drain it
        if(flashLightStatus == true)
        {
            DrainFlashLight(flashLightDrainRate * Time.deltaTime);
            if(flashLightBattery <= 0.0f)
            {
                ToggleFlashLight(false);
            }
        }
        //Otherwise, recharge it
        else
        {
            if(GetFlashLightCharge() < GetMaxFlashLightCharge())
            {
                RechargeFlashLight(flashLightRechargeRate * Time.deltaTime);
            }
        }
    }

    //Function to update flash light status across the network.
    public void UpdateFlashLightStatus()
    {
        PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
        if (host != null)
        {
            host.CmdUpdateFlashLightStatus(flashLightStatus);
        }
    }

    //Function to update flash light battery across the network.
    public void UpdateFlashLightBattery()
    {
        PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
        if (host != null)
        {
            host.CmdUpdateFlashLightBattery(flashLightBattery, flashLightMaxBattery);
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

    public bool GetFlashLightStatus()
    {
        return flashLightStatus;
    }

    public float GetBaseFlashLightCharge()
    {
        return baseFlashLightBattery;
    }
}
