using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnectionObject : NetworkBehaviour
{
    public GameObject gameManager = null;
    public GameObject playerGameObjectPrefab = null;
    public string playerName = "Player";
    public GameObject playerGameObject = null;

    NetworkUtils netUtils = null;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        netUtils = gameManager.GetComponent<NetworkUtils>();

        if(isLocalPlayer == false)
        {
            //This object belongs to another player
            return;
        }

        //Spawn object
        Debug.Log("PlayerObject::Start - Spawning player game object");

        CmdSpawnPlayerGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CmdChangePlayerName("Player" + Random.Range(1, 100));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CmdResetPlayerConnections();
        }
    }

    void OnDestroy()
    {
        if(playerGameObject != null)
        {
            playerGameObject.GetComponent<PlayerController>().EnableCursor();
        }
        Destroy(playerGameObject);
    }

    //Function to update all commands and refresh all clients, useful for when a new player joins
    void Resync()
    {
        CmdUpdateEnvironment();
        CmdChangePlayerName("Player" + Random.Range(1, 100));
        CmdUpdatePlayerSkillPoints();

        PlayerFlashLight flashlight = playerGameObject.GetComponent<PlayerFlashLight>();
        if(flashlight != null)
        {
            CmdUpdateFlashLightStatus(flashlight.flashLightStatus);
            CmdUpdateFlashLightBattery(flashlight.GetFlashLightCharge(), flashlight.GetMaxFlashLightCharge());
        }

        PlayerAttributes attributes = playerGameObject.GetComponent<PlayerAttributes>();
        if (attributes != null)
        {
            CmdUpdatePlayerAttributes(attributes.GetHealth(), attributes.GetMaxHealth(), attributes.GetStamina(), attributes.GetMaxStamina());
        }
    }

    /////////////////////////////// COMMANDS ///////////////////////////////
    //Commands are only executed on the host client / server
    //Commands guarantee that the function is running on the server

    //Command to spawn player on server
    [Command]
    void CmdSpawnPlayerGameObject()
    {
        Debug.Log("CMD: Spawning new player");
        //Creating object on server
        playerGameObject = Instantiate(playerGameObjectPrefab);

        playerGameObject.transform.position = transform.position;
        playerGameObject.transform.rotation = transform.rotation;

        //Spawn object on all clients
        NetworkServer.Spawn(playerGameObject, connectionToClient);

        RpcResetPlayerConnection();
    }

    //Command to reset player connections
    [Command]
    public void CmdResetPlayerConnections()
    {
        RpcResetPlayerConnection();
    }

    //Command to change player name on server
    [Command]
    public void CmdChangePlayerName(string newName)
    {
        Debug.Log("CMD: Player name change requested: " + playerName + " to " + newName);
        RpcChangePlayerName(newName);
        playerName = newName;
    }

    //Command to update environment
    [Command]
    public void CmdUpdateEnvironment()
    {
        Debug.Log("CMD: Updating Environment");
        EnvironmentController controller = gameManager.GetComponent<EnvironmentController>();
        RpcUpdateEnvironment(controller.timeMultiplier, controller.currentTimeOfDay, controller.days, controller.secondsInFullDay, controller.temperature, controller.windSpeed, controller.windAngle);
    }
    
    //Command to toggle flashlight
    [Command]
    public void CmdUpdateFlashLightStatus(bool flashLightStatus)
    {
        Debug.Log("CMD: Update Flash Light Status");
        PlayerFlashLight flashlight = playerGameObject.GetComponent<PlayerFlashLight>();
        if (flashlight != null)
        {
            RpcUpdateFlashLightStatus(flashLightStatus);
        }
    }

    //Command to update flashlight battery info
    [Command]
    public void CmdUpdateFlashLightBattery(float flashLightBattery, float flashLightMaxBattery)
    {
        Debug.Log("CMD: Update Flash Light Battery");
        PlayerFlashLight flashlight = playerGameObject.GetComponent<PlayerFlashLight>();
        if (flashlight != null)
        {
            RpcUpdateFlashLightBattery(flashLightBattery, flashLightMaxBattery);

            flashlight.SetFlashLightCharge(flashLightBattery);
            flashlight.SetMaxFlashLightCharge(flashLightMaxBattery);
        }
    }

    //Command to update player attributes
    [Command]
    public void CmdUpdatePlayerAttributes(float health, float maxHealth, float stamina, float maxStamina)
    {
        Debug.Log("CMD: Update Player Attributes");
        PlayerAttributes attributes = playerGameObject.GetComponent<PlayerAttributes>();
        if(attributes != null)
        {
            RpcUpdatePlayerAttributes(attributes.GetHealth(), attributes.GetMaxHealth(), attributes.GetStamina(), attributes.GetMaxStamina());

            attributes.SetHealth(health);
            attributes.SetMaxHealth(maxHealth);
            attributes.SetStamina(stamina);
            attributes.SetMaxStamina(maxStamina);
        }
    }
    
    //Command to update player skills points
    [Command]
    public void CmdUpdatePlayerSkillPoints()
    {
        Debug.Log("CMD: Update Player Skillpoints");
        PlayerSkills playerSkills = playerGameObject.GetComponent<PlayerSkills>();
        if(playerSkills != null)
        {
            int[] playerSkillPoints = playerSkills.GetPlayerSkills();
            int newSkillPointCount = playerSkills.GetCurrentSkillPoint();

            RpcUpdatePlayerSkillPoints(newSkillPointCount, playerSkillPoints);
        }
    }

    /////////////////////////////// RPC ///////////////////////////////
    //RPCs (remote procedure calls) are functions that are only executed on clients

    //RPC to setup player connections and get the correct game object for that connection object
    [ClientRpc]
    void RpcResetPlayerConnection()
    {
        //Reset position to 0, 0, 0
        transform.position = new Vector3(0, 0, 0);
        //Reset rotation to 0, 0, 0
        transform.rotation = Quaternion.identity;

        GameObject[] playerConnectionObjects = netUtils.GetAllPlayerConnectionObjects();
        GameObject[] playerObjects = netUtils.GetAllPlayerObjects();

        //For every player connection
        for (int i = 0; i < playerConnectionObjects.Length; i++)
        {
            //Link connection object to player object
            PlayerConnectionObject playerConnection = playerConnectionObjects[i].GetComponent<PlayerConnectionObject>();
            playerConnection.playerGameObject = playerObjects[i];
            if(playerConnection.isLocalPlayer == true)
            {
                playerConnection.Resync();
            }
        }
    }

    //RPC to set the player name
    [ClientRpc]
    void RpcChangePlayerName(string newName)
    {
        //Change game object name
        gameObject.name = "PlayerConnectionObject(" + newName + ")";
        //Setting manually as when a hook is used the local value does not get updated
        playerName = newName;
        playerGameObject.name = "Player(" + newName + ")";
    }

    //Rpc to update environment controller data
    [ClientRpc]
    void RpcUpdateEnvironment(float m_timeMultiplier, float m_currentTime, int m_days, float m_secondsInFullDay, float m_temperature, float m_windStrength, float m_windAngle)
    {
        EnvironmentController controller = gameManager.GetComponent<EnvironmentController>();
        if(controller != null)
        {
            controller.timeMultiplier = m_timeMultiplier;
            controller.currentTimeOfDay = m_currentTime;
            controller.days = m_days;
            controller.secondsInFullDay = m_secondsInFullDay;
            controller.temperature = m_temperature;
            controller.windSpeed = m_windStrength;
            controller.windAngle = m_windAngle;
        }
    }

    //RPC to set the flash light status
    [ClientRpc]
    void RpcUpdateFlashLightStatus(bool status)
    {
        PlayerFlashLight flashlight = playerGameObject.GetComponent<PlayerFlashLight>();
        if(flashlight != null)
        {
            flashlight.ToggleFlashLight(status);
        }
    }

    //RPC to update flash light battery
    [ClientRpc]
    void RpcUpdateFlashLightBattery(float flashLightBattery, float flashLightMaxBattery)
    {
        PlayerFlashLight flashlight = playerGameObject.GetComponent<PlayerFlashLight>();
        if (flashlight != null)
        {
            //Only sync for local player, prevents incorrect data being synced, fixes issue #5 on GitHub
            if (hasAuthority)
            {
                return;
            }

            flashlight.SetFlashLightCharge(flashLightBattery);
            flashlight.SetMaxFlashLightCharge(flashLightMaxBattery);
        }
    }

    //RPC to update player attributes such as health
    [ClientRpc]
    void RpcUpdatePlayerAttributes(float health, float maxHealth, float stamina, float maxStamina)
    {
        PlayerAttributes attributes = playerGameObject.GetComponent<PlayerAttributes>();
        if(attributes != null)
        {
            //Only sync for local player, prevents incorrect data being synced, fixes issue #5 on GitHub
            if (hasAuthority)
            {
                return;
            }

            attributes.SetHealth(health);
            attributes.SetMaxHealth(maxHealth);
            attributes.SetStamina(stamina);
            attributes.SetMaxStamina(maxStamina);
        }
    }

    //RPC to update player skill points
    [ClientRpc]
    public void RpcUpdatePlayerSkillPoints(int newSkillPointCount, int[] playerSkillPoints)
    {
        PlayerSkills playerSkills = playerGameObject.GetComponent<PlayerSkills>();
        if (playerSkills != null)
        {
            playerSkills.SetPlayerSkillPoints(playerSkillPoints, newSkillPointCount);
        }
    }
}
