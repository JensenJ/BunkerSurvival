using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerFlashLight : NetworkBehaviour
{
    [SerializeField] GameObject flashLight = null;
    [SerializeField] public bool flashLightStatus = false;
    // Start is called before the first frame update
    void Start()
    {
        if (flashLight == null)
        {
            flashLight = transform.GetChild(0).GetChild(0).gameObject;
            
        }
        ToggleFlashLight(flashLightStatus);
    }

    public void ToggleFlashLight(bool status)
    {
        flashLightStatus = status;
        flashLight.SetActive(status);
    }
}
