using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITesting : MonoBehaviour
{
    private Pathfinding pathfinding;
    [SerializeField] LayerMask[] unwalkableMasks;
    [SerializeField] Vector2Int gridSize;
    [SerializeField] float walkableCheckRadius;
    [SerializeField] bool showDebug;

    private void Start()
    {
        pathfinding = new Pathfinding(gridSize.x, gridSize.y, unwalkableMasks, walkableCheckRadius, showDebug);
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
                        Debug.DrawLine(new Vector3(path[i].x + 0.5f, 1.1f, path[i].y + 0.5f), new Vector3(path[i + 1].x + 0.5f, 1.1f, path[i + 1].y + 0.5f), Color.green, 5f);
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
