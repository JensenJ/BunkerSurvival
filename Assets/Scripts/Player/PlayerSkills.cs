using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSkills : NetworkBehaviour
{
    GameObject gameManager = null;
    [SerializeField] public PlayerSkillController skillController = null;

    [SerializeField] int currentSkillPoints = 0;
    [SerializeField] int[] currentSkillIDLevels = null;

    PlayerSkillData[] allSkills = null;

    NetworkUtils netUtils = null; 
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        netUtils = gameManager.GetComponent<NetworkUtils>();
        skillController = gameManager.GetComponent<PlayerSkillController>();
        allSkills = skillController.GetAllSkills();
        currentSkillIDLevels = new int[allSkills.Length];
    }

    //Function to update the skill points across the network.
    void UpdateSkillPoints()
    {
        //Update environment
        PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
        if (host != null)
        {
            host.CmdUpdatePlayerSkillPoints();
        }
    }

    //Increases skill level at the specified index
    public void IncreaseSkillLevel(int skillID)
    {
        if(currentSkillPoints > 0)
        {
            currentSkillPoints--;
            currentSkillIDLevels[skillID]++;
            UpdateSkillPoints();
        }
    }
    
    //Resets all skills and adds to skill point pool
    public void ResetSkills()
    {
        for (int i = 0; i < allSkills.Length; i++)
        {
            currentSkillPoints += currentSkillIDLevels[i];
        }
        currentSkillIDLevels = new int[allSkills.Length];
        UpdateSkillPoints();
    }

    //Setter for skill points
    public void SetPlayerSkillPoints(int[] skillPoints, int skillPointCount)
    {
        currentSkillIDLevels = skillPoints;
        currentSkillPoints = skillPointCount;
    }

    //Gets skill level at the specified index
    public int GetPlayerSkillLevel(int skillID)
    {
        return currentSkillIDLevels[skillID];
    }

    public int[] GetPlayerSkills()
    {
        return currentSkillIDLevels;
    }

    public void LevelUp()
    {
        currentSkillPoints++;
        UpdateSkillPoints();
    }

    //Gets the current skill points of the player
    public int GetCurrentSkillPoint()
    {
        return currentSkillPoints;
    }
}

