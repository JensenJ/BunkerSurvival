using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnectionObject : NetworkBehaviour
{
    public GameObject gameManager = null;
    public GameObject playerGameObjectPrefab = null;
    public string playerName = "Player";
    GameObject playerGameObject = null;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");

        if(isLocalPlayer == false)
        {
            //This object belongs to another player
            return;
        }

        //Spawn object
        Debug.Log("PlayerObject::Start - Spawning player game object");

        CmdSpawnPlayerGameObject();
        CmdUpdateEnvironment();
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
        //Creating object on server
        playerGameObject = Instantiate(playerGameObjectPrefab);
        Debug.Log("Created new object, player object: " + playerGameObject);

        playerGameObject.transform.position = transform.position;
        playerGameObject.transform.rotation = transform.rotation;

        RpcSetupNewPlayerConnection();

        //Spawn object on all clients
        NetworkServer.Spawn(playerGameObject, connectionToClient);
    }

    //Command to change player name on server
    [Command]
    public void CmdChangePlayerName(string newName)
    {
       RpcChangePlayerName(newName);
       playerName = newName;
    }

    //Command to update environment
    [Command]
    public void CmdUpdateEnvironment()
    {
        print("CMD: Updating Environment");
        EnvironmentController controller = gameManager.GetComponent<EnvironmentController>();
        RpcUpdateEnvironment(controller.timeMultiplier, controller.currentTimeOfDay, controller.days, controller.secondsInFullDay, controller.temperature, controller.windStrength, controller.windAngle);
    }


    /////////////////////////////// RPC ///////////////////////////////
    //RPCs are functions that are only executed on clients

    [ClientRpc]
    void RpcSetupNewPlayerConnection()
    {
        //Reset position to 0, 0, 0
        transform.position = new Vector3(0, 0, 0);
        //Reset rotation to 0, 0, 0
        transform.rotation = Quaternion.identity;
    }

    [ClientRpc]
    void RpcChangePlayerName(string newName)
    {
        //Print name change
        Debug.Log("PlayerName Changed:" + playerName + " to " + newName);
        //Change game object name
        gameObject.name = "PlayerConnectionObject(" + newName + ")";
        //Setting manually as when a hook is used the local value does not get updated
        playerName = newName;
    }

    [ClientRpc]
    void RpcUpdateEnvironment(float m_timeMultiplier, float m_currentTime, int m_days, float m_secondsInFullDay, float m_temperature, float m_windStrength, float m_windAngle)
    {
        print("RPC: Updating Environment");
        EnvironmentController controller = gameManager.GetComponent<EnvironmentController>();
        controller.timeMultiplier = m_timeMultiplier;
        controller.currentTimeOfDay = m_currentTime;
        controller.days = m_days;
        controller.secondsInFullDay = m_secondsInFullDay;
        controller.temperature = m_temperature;
        controller.windStrength = m_windStrength;
        controller.windAngle = m_windAngle;
    }
}
