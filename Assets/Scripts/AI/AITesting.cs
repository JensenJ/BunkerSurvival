using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITesting : MonoBehaviour
{
    private Pathfinding pathfinding;
    [SerializeField] LayerMask[] unwalkableMasks = null;
    [SerializeField] Vector2Int gridSize = new Vector2Int();
    [SerializeField] Vector3 gridOriginPosition= new Vector3();
    [SerializeField] float walkableCheckRadius = 0.0f;
    [SerializeField] bool showDebug = false;

    private void Start()
    {
        gridOriginPosition += transform.position;
        pathfinding = new Pathfinding(gridSize.x, gridSize.y, gridOriginPosition, unwalkableMasks, walkableCheckRadius, showDebug);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            int x = Random.Range(0, pathfinding.GetGrid().GetWidth());
            int y = Random.Range(0, pathfinding.GetGrid().GetHeight());

            print(x + ", " + y);

            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    if (showDebug)
                    {
                        Debug.DrawLine(new Vector3(gridOriginPosition.x + path[i].x + 0.5f, 1.1f, gridOriginPosition.z + path[i].y + 0.5f), 
                            new Vector3(gridOriginPosition.x + path[i + 1].x + 0.5f, 1.1f, gridOriginPosition.z + path[i + 1].y + 0.5f), Color.green, 10f);
                    }
                }
            }
            else
            {
                Debug.Log("Path unavailable");
            }
        }
    }
}
