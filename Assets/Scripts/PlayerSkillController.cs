using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSkillController : NetworkBehaviour
{
    //Array containing all skills
    [SerializeField] private PlayerSkillData[] playerSkills = null;

    public PlayerSkillData[] GetAllSkills()
    {
        return playerSkills;
    }

    public PlayerSkillData GetSkillDataFromID(int id)
    {
        //If skill id is not valid
        if(playerSkills.Length > id || id < 0)
        {
            return playerSkills[0];
        }
        else
        {
            return playerSkills[id];
        }
    }

    public bool IsValidSkill(int id)
    {
        for (int i = 0; i < playerSkills.Length; i++)
        {
            if(id == i)
            {
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public struct PlayerSkillData
{
    public string skillName;
    public string skillDescription;
    public int skillID;
    public int[] skillRequirementIDs;
    public int skillCost;
}