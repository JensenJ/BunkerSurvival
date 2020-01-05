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
        CmdUpdateEnvironment();
        CmdChangePlayerName("Player" + Random.Range(1, 100));
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
        Destroy(playerGameObject);
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
    public void CmdToggleFlashLight()
    {
        Debug.Log("CMD: Toggle Flash Light");
        PlayerFlashLight flashlight = playerGameObject.GetComponent<PlayerFlashLight>();
        if (flashlight != null)
        {
            flashlight.flashLightStatus = !flashlight.flashLightStatus;
            RpcUpdateFlashLightStatus(flashlight.flashLightStatus);
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
            attributes.health = attributes.GetHealth();
            attributes.maxHealth = attributes.GetMaxHealth();
            attributes.stamina = attributes.GetStamina();
            attributes.maxStamina = attributes.GetMaxStamina();

            RpcUpdatePlayerAttributes(attributes.health, attributes.maxHealth, attributes.stamina, attributes.maxStamina);
        }
    }

    /////////////////////////////// RPC ///////////////////////////////
    //RPCs are functions that are only executed on clients

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
        }
    }

    [ClientRpc]
    void RpcChangePlayerName(string newName)
    {
        //Change game object name
        gameObject.name = "PlayerConnectionObject(" + newName + ")";
        //Setting manually as when a hook is used the local value does not get updated
        playerName = newName;
        playerGameObject.name = "Player(" + newName + ")";
    }

    [ClientRpc]
    void RpcUpdateEnvironment(float m_timeMultiplier, float m_currentTime, int m_days, float m_secondsInFullDay, float m_temperature, float m_windStrength, float m_windAngle)
    {
        EnvironmentController controller = gameManager.GetComponent<EnvironmentController>();
        controller.timeMultiplier = m_timeMultiplier;
        controller.currentTimeOfDay = m_currentTime;
        controller.days = m_days;
        controller.secondsInFullDay = m_secondsInFullDay;
        controller.temperature = m_temperature;
        controller.windSpeed = m_windStrength;
        controller.windAngle = m_windAngle;
    }

    [ClientRpc]
    void RpcUpdateFlashLightStatus(bool status)
    {
        PlayerFlashLight flashlight = playerGameObject.GetComponent<PlayerFlashLight>();
        if(flashlight != null)
        {
            flashlight.ToggleFlashLight(status);
        }
    }

    [ClientRpc]
    void RpcUpdatePlayerAttributes(float health, float maxHealth, float stamina, float maxStamina)
    {
        PlayerAttributes attributes = playerGameObject.GetComponent<PlayerAttributes>();
        if(attributes != null)
        {
            attributes.health = health;
            attributes.maxHealth = maxHealth;
            attributes.stamina = stamina;
            attributes.maxStamina = maxStamina;
        }
    }
}
