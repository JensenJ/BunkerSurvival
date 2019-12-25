using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerObject : NetworkBehaviour
{
    public GameObject playerGameObject;

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
        
    }

    /////////////////////////////// COMMANDS ///////////////////////////////
    //Commands are only executed on the host client / server
    //Commands guarantee that the function is running on the server

    [Command]
    void CmdSpawnPlayerGameObject()
    {
        //Creating object on server
        GameObject go = Instantiate(playerGameObject);

        //Send object to all clients
        NetworkServer.Spawn(go);
    }
}
