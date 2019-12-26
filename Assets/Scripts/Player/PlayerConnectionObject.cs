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
    void CmdChangePlayerName(string newName)
    {
       RpcChangePlayerName(newName);
       playerName = newName;
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
}
