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

    //Creates current skill id levels
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
    public void IncreaseSkillLevel(PlayerSkill skill)
    {
        if(currentSkillPoints > 0)
        {
            //Get requirements for the skill to increase level to
            int skillID = skillController.GetSkillIDFromType(skill);

            PlayerSkill[] requirements = allSkills[skillID].skillRequirements;

            bool valid = true;
            //Check if the player possesses the required skills before applying
            for (int i = 0; i < requirements.Length; i++)
            {
                int skillIndex = skillController.GetSkillIDFromType(requirements[i]);
                //If they do not possess the skill
                if (currentSkillIDLevels[skillIndex] <= 0)
                {
                    valid = false;
                }
            }
            //If all criteria are met
            if(valid == true)
            {
                //Apply skill level increase
                currentSkillPoints--;
                currentSkillIDLevels[skillID]++;
                UpdateSkillPoints();
            }
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

