using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AIUnit : NetworkBehaviour
{

    IEnumerator movementCoroutine;

    [SerializeField] AIGrid aiGrid = null;

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
            Vector3 targetPos = new Vector3(Random.Range(0, 50), 1, Random.Range(0, 50));
            Debug.Log("Target position: " + targetPos);

            //Coroutine for movement
            if(movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }
            movementCoroutine = MoveUnit(targetPos);
            StartCoroutine(movementCoroutine);
        }
    }

    //Coroutine for moving a unit
    IEnumerator MoveUnit(Vector3 targetPosition)
    {
        List<Vector3> positions = aiGrid.GetPath(transform.position, targetPosition);
        if(positions != null)
        {
            for (int i = 0; i < positions.Count - 1; i++)
            {
                transform.position = positions[i];
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            Debug.Log("No path available");
        }
    }
}
