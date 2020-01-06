using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSkills : NetworkBehaviour
{
    GameObject gameManager = null;
    [SerializeField] public PlayerSkillController skillController = null;

    [SerializeField] int currentSkillPoints = 0;
    [SerializeField] List<int> currentSkillIDs = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        if(skillController != null || gameManager != null)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController");
            skillController = gameManager.GetComponent<PlayerSkillController>();
        }
    }

    //Adds a skill to players list
    public void AddSkillID(int id)
    {
        //If the skill has a valid is and the player does NOT already have it
        if (skillController.IsValidSkill(id) && !PlayerHasSkill(id))
        {
            currentSkillIDs.Add(id);
        }
    }

    //Removes a skill from players list
    public void RemoveSkillID(int id)
    {
        if (skillController.IsValidSkill(id) && PlayerHasSkill(id))
        {
            currentSkillIDs.Remove(id);
        }
    }

    //Function to find if the player possesses the skill id
    public bool PlayerHasSkill(int id)
    {
        for (int i = 0; i < currentSkillIDs.Count; i++)
        {
            if(id == currentSkillIDs[i])
            {
                return true;
            }
        }
        return false;
    }

    //Returns all player skill ids
    public int[] GetCurrentPlayerSkills()
    {
        return currentSkillIDs.ToArray();
    }

    //Gets the current skill points of the player
    public int GetCurrentSkillPoint()
    {
        return currentSkillPoints;
    }
}

