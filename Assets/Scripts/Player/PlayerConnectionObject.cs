using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnectionObject : NetworkBehaviour
{
    public GameObject playerGameObject;
    [SyncVar(hook = "OnPlayerNameChange")] public string playerName = "Player";

    // Start is called before the first frame update
    void Start()
    {
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

    //Called by hook on playername syncvar
    void OnPlayerNameChange(string newName) 
    {
        //Print name change
        Debug.Log("PlayerName Changed:" + playerName + " to " + newName);
        //Change game object name
        gameObject.name = "PlayerConnectionObject(" + newName + ")";
        //Setting manually as when a hook is used the local value does not get updated
        playerName = newName;
    }

    /////////////////////////////// COMMANDS ///////////////////////////////
    //Commands are only executed on the host client / server
    //Commands guarantee that the function is running on the server

    //Command to spawn player on server
    [Command]
    void CmdSpawnPlayerGameObject()
    {
        //Creating object on server
        GameObject go = Instantiate(playerGameObject);

        go.transform.position = transform.position;
        transform.position = new Vector3(0, 0, 0);

        //Spawn object on all clients
        NetworkServer.Spawn(go, connectionToClient);
    }

    //Command to change player name on server
    [Command]
    void CmdChangePlayerName(string newName)
    {
        playerName = newName;
       //RpcChangePlayerName(newName);
    }

    /////////////////////////////// RPC ///////////////////////////////
    //RPCs are functions that are only executed on clients
    //[ClientRpc]
    //void RpcChangePlayerName(string newName)
    //{
    //    playerName = newName;
    //}
}
