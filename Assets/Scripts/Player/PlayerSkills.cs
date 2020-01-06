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

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        skillController = gameManager.GetComponent<PlayerSkillController>();
        allSkills = skillController.GetAllSkills();
        currentSkillIDLevels = new int[allSkills.Length];
    }

    //Increases skill level at the specified index
    public void IncreaseSkillLevel(int skillID)
    {
        if(currentSkillPoints > 0)
        {
            currentSkillPoints--;
            currentSkillIDLevels[skillID]++;
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
    }

    //Gets skill level at the specified index
    public int GetPlayerSkillLevel(int skillID)
    {
        return currentSkillIDLevels[skillID];
    }

    public void LevelUp()
    {
        currentSkillPoints++;
    }

    //Gets the current skill points of the player
    public int GetCurrentSkillPoint()
    {
        return currentSkillPoints;
    }
}

