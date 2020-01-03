using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkUtils : MonoBehaviour
{
    //Function to return the host player, mainly used for command access
    public PlayerConnectionObject GetHostPlayerConnectionObject()
    {
        //Get all players
        GameObject[] playerConnections = GetAllPlayerConnectionObjects();
        //Loop over them
        for (int i = 0; i < playerConnections.Length; i++)
        {
            //Get player connection object
            PlayerConnectionObject playerObject = playerConnections[i].GetComponent<PlayerConnectionObject>();
            //If this is the local player
            if (playerObject.isLocalPlayer == true)
            {
                //returns playerobject;
                return playerObject;
            }
        }
        return null;
    }

    //Returns an array of all player objects
    public GameObject[] GetAllPlayerObjects()
    {
        return GameObject.FindGameObjectsWithTag("Player");
    }

    //Returns an array of all player connection objects
    public GameObject[] GetAllPlayerConnectionObjects()
    {
        return GameObject.FindGameObjectsWithTag("PlayerConnection");
    }

    public PlayerController GetPlayerControllerObjectFromConnection(PlayerConnectionObject playerConnection)
    {
        //Finding all player objects and connections
        GameObject[] playerControllers = GetAllPlayerObjects();
        GameObject[] playerConnections = GetAllPlayerConnectionObjects();

        //Find id of passed in player connection in array
        int connectionID = 0;
        for (int i = 0; i < playerConnections.Length; i++)
        {
            //If equal
            if(playerConnection == playerConnections[i])
            {
                //Set id and break out of loop
                connectionID = i;
                break;
            }
        }
        
        //Get item within array and return
        PlayerController controller = playerControllers[connectionID].GetComponent<PlayerController>();
        return controller;
    }

    public PlayerConnectionObject GetPlayerConnectionObjectFromController(PlayerController playerController)
    {
        //Finding all player objects and connections
        GameObject[] playerControllers = GetAllPlayerObjects();
        GameObject[] playerConnections = GetAllPlayerConnectionObjects();

        //Find id of passed in player controller in array
        int connectionID = 0;
        for (int i = 0; i < playerControllers.Length; i++)
        {
            //If equal
            if (playerController == playerControllers[i])
            {
                //Set id and break out of loop
                connectionID = i;
                break;
            }
        }

        //Get item within array and return
        PlayerConnectionObject connection = playerConnections[connectionID].GetComponent<PlayerConnectionObject>();
        return connection;
    }
}
