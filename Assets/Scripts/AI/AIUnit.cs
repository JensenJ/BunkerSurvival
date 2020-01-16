using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AIUnit : NetworkBehaviour
{
    //Different AI states
    private enum AIState
    {
        None,
        Roaming,
        Chase,
        Patrol
    }

    [SerializeField] AIGrid aiGrid = null;
    [SerializeField] public float unitSpeed = 1.0f;
    [SerializeField] AIState state;
    [SerializeField] Vector3 targetPos;

    private float unitHeight = 1.0f;

    float waitTime = 1.0f;

    List<Vector3> currentPath = null;
    int currentPathIndex = 0;

    void Awake()
    {
        targetPos = transform.position;
        state = AIState.None;
    }

    // Start is called before the first frame update
    void Start()
    {
        //If hasnt been assigned
        if(aiGrid == null)
        {
            // Try find it
            aiGrid = GameObject.FindGameObjectWithTag("AIGrid").GetComponent<AIGrid>();
            //If still null
            if(aiGrid == null)
            {
                //Log error
                Debug.LogError("AI grid has not been assigned or could not be found, there can only be one grid in a scene.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //State machine for AI behaviour
        switch (state)
        {
            case AIState.None:
                //Check for keyboard press
                if (Input.GetKeyDown(KeyCode.R))
                {
                    //Randomly generate position and attempt to move there
                    targetPos = new Vector3(Random.Range(0, 50), unitHeight, Random.Range(0, 50));
                    Debug.Log("Moving to " + targetPos);
                    SetTargetPosition(targetPos);
                }
                break;
            case AIState.Roaming:
                //If at destination and roam wait time has exceeded, calculate new roam position
                if (Vector3.Distance(transform.position, targetPos) < 1.0f && Time.time >= waitTime)
                {
                    int x = (int)transform.position.x + Random.Range(-10, 10);
                    int z = (int)transform.position.z + Random.Range(-10, 10);
                    targetPos = new Vector3(x, unitHeight, z);
                    Debug.Log("Roaming to " + targetPos);
                    SetTargetPosition(targetPos);
                    waitTime = Random.Range(0.0f, 10.0f) + Time.time;
                }
                break;
            case AIState.Chase:
                if (Time.time >= waitTime)
                {
                    targetPos = GetNearestThreatPosition();
                    targetPos.y = unitHeight;
                    Debug.Log("Chasing to " + targetPos);
                    SetTargetPosition(targetPos);
                    waitTime = Time.time + 2.0f;
                }
                break;
        }
        Move();
    }

    //Function for moving
    private void Move()
    {
        //If there is a path
        if(currentPath != null)
        {
            //Get target pos
            Vector3 targetPos = currentPath[currentPathIndex];
            //If within a certain range of area, stop moving
            if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                //Actual transformation
                Vector3 moveDir = (targetPos - transform.position).normalized;
                transform.position = transform.position + moveDir * unitSpeed * Time.deltaTime;
            }
            else
            {
                //arrived at destination (each node), increment path index
                currentPathIndex++;
                //If at final node, stop moving
                if(currentPathIndex >= currentPath.Count)
                {
                    StopMoving();
                }
            }
        }
    }

    //Function to stop moving
    public void StopMoving()
    {
        currentPath = null;
    }

    //Sets the target position for movement.
    public void SetTargetPosition(Vector3 target)
    {
        currentPathIndex = 0;
        currentPath = Pathfinding.Instance.FindPath(transform.position, target);

        if(currentPath != null && currentPath.Count > 1)
        {
            currentPath.RemoveAt(0);
        }

        if(currentPath == null)
        {
            Debug.Log("Path not found");
            targetPos = transform.position;
        }
    }

    //Function to return the nearest threat's position
    public Vector3 GetNearestThreatPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        List<GameObject> targets = new List<GameObject>();

        //Add all players found to target list
        for (int i = 0; i < players.Length; i++)
        {
            targets.Add(players[i]);
        }

        //Add all buildings found to target list
        for (int i = 0; i < buildings.Length; i++)
        {
            targets.Add(buildings[i]);
        }

        GameObject nearestTarget = null;
        float nearestDistance = float.MaxValue;

        Debug.Log(targets.Count);

        //Get nearest target
        for (int i = 0; i < targets.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, targets[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = targets[i];
            }
        }

        //If no targets found
        if(nearestTarget == null)
        {
            Vector3 selfPos = transform.position;
            selfPos.y = unitHeight;
            return selfPos;
        }

        Vector3 pos = nearestTarget.transform.position;
        pos.y = unitHeight;
        return pos;
    }
}
