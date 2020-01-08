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

    //Returns skill id based on first entry of skill type, (all skills types should be unique to that skill)
    public int GetSkillIDFromType(PlayerSkill playerSkill)
    {
        //for every skill
        for (int i = 0; i < playerSkills.Length; i++)
        {
            if (playerSkills[i].skillType == playerSkill)
            {
                return i;
            }
        }
        return 0;
    }

    //Checks whether the skill is of a valid id
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
public enum PlayerSkill 
{ 
    None,
    IncreasedHealth,
    IncreasedStamina,
    FasterMovement,
    FlashlightEfficiency
}

//Struct for player skills
[System.Serializable]
public struct PlayerSkillData
{
    public PlayerSkill skillType;
    public string skillDescription;
    public int skillLevel;
    public int maxSkillLevel;
    public PlayerSkill[] skillRequirements;
}