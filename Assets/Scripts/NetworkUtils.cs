using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkUtils : MonoBehaviour
{
    //Function to return the host player, mainly used for command access
    public PlayerConnectionObject GetHostPlayerConnectionObject()
    {
        //Get all players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //Loop over them
        for (int i = 0; i < players.Length; i++)
        {
            //Get player connection object
            PlayerConnectionObject playerObject = players[i].GetComponent<PlayerConnectionObject>();
            //If this is the local player
            if (playerObject.isLocalPlayer == true)
            {
                //returns playerobject;
                return playerObject;
            }
        }
        return null;
    }
}
