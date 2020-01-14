using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AIUnit : NetworkBehaviour
{

    [SerializeField] AIGrid aiGrid = null;
    [SerializeField] public float unitSpeed = 1.0f;

    List<Vector3> currentPath = null;
    int currentPathIndex = 0;

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
        //Check for keyboard press
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Randomly generate position and attempt to move there
            Vector3 targetPos = new Vector3(Random.Range(0, 50), 1.0f, Random.Range(0, 50));
            Debug.Log("Moving to" + targetPos);
            SetTargetPosition(targetPos);
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
            if (Vector3.Distance(transform.position, targetPos) > 0.5f)
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
    public void SetTargetPosition(Vector3 targetPos)
    {
        currentPathIndex = 0;
        currentPath = Pathfinding.Instance.FindPath(transform.position, targetPos);

        if(currentPath != null && currentPath.Count > 1)
        {
            currentPath.RemoveAt(0);
        }
    }
}
