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
    PlayerFlashLight flashlight = null;
    PlayerAttributes attributes = null;
    PlayerController controller = null;

    //Creates current skill id levels
    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        netUtils = gameManager.GetComponent<NetworkUtils>();
        skillController = gameManager.GetComponent<PlayerSkillController>();
        allSkills = skillController.GetAllSkills();
        currentSkillIDLevels = new int[allSkills.Length];

        flashlight = GetComponent<PlayerFlashLight>();
        attributes = GetComponent<PlayerAttributes>();
        controller = GetComponent<PlayerController>();
    }

    //Function to update the skill points across the network.
    void UpdateSkillPoints()
    {
        //Apply skills
        ApplySkills();
        //Update skill points
        PlayerConnectionObject host = netUtils.GetHostPlayerConnectionObject();
        if (host != null)
        {
            host.CmdUpdatePlayerSkillPoints();
        }
    }

    //Function to apply player skills
    void ApplySkills()
    {
        //Apply flashlight skills
        if(flashlight != null)
        {
            float newFlash = GetAppliedSkillPercentValue(PlayerSkill.FlashlightEfficiency, flashlight.GetBaseFlashLightCharge(), 0.05f);
            flashlight.SetMaxFlashLightCharge(newFlash);
            flashlight.SetFlashLightCharge(newFlash);
        }
        //Apply attribute skills
        if(attributes != null)
        {
            //Health
            float newHealth = GetAppliedSkillPercentValue(PlayerSkill.IncreasedHealth, attributes.GetBaseHealth(), 0.05f);
            attributes.SetMaxHealth(newHealth);
            attributes.SetHealth(newHealth);
            //Stamina
            float newStamina = GetAppliedSkillPercentValue(PlayerSkill.IncreasedStamina, attributes.GetBaseStamina(), 0.05f);
            attributes.SetMaxStamina(newStamina);
            attributes.SetStamina(newStamina);
        }
        if(controller != null)
        {
            //MovementSpeed
            //Speed
            float newSpeed = GetAppliedSkillPercentValue(PlayerSkill.FasterMovement, controller.GetBaseSpeed(), 0.02f);
            controller.SetSpeed(newSpeed);

            //Sprint speed
            float newSprintSpeed = GetAppliedSkillPercentValue(PlayerSkill.FasterMovement, controller.GetBaseSprintSpeed(), 0.02f);
            controller.SetSprintSpeed(newSprintSpeed);
        }
    }

    //Function to work out new values once skills have been applied.
    float GetAppliedSkillPercentValue(PlayerSkill skill, float baseValue, float increasePercent)
    {
        int level = GetPlayerSkillLevel(skillController.GetSkillIDFromType(skill));
        float newPercent = baseValue * increasePercent;
        return baseValue + (newPercent * level);
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
            //Check if max level for this skill has been achieved
            if(currentSkillIDLevels[skillID] >= allSkills[skillID].maxSkillLevel)
            {
                valid = false;
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

