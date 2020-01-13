using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridSystem<PathNode> grid;
    public int x;
    public int y;
    Vector3 gridOriginPosition;

    public int gCost;
    public int hCost;
    public int fCost;

    LayerMask[] unwalkableMasks;
    public PathNode previousNode;
    public PathNode(GridSystem<PathNode> grid, int x, int y, Vector3 gridOriginPosition, LayerMask[] unwalkableMasks)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.unwalkableMasks = unwalkableMasks;
        this.gridOriginPosition = gridOriginPosition;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + ", " + y;
    }

    public bool CheckWalkable(float checkRadius)
    {
        Vector3 worldPosition = new Vector3(gridOriginPosition.x + x, 0, gridOriginPosition.z + y) + Vector3.one * grid.GetCellSize() * 0.5f;
        for (int i = 0; i < unwalkableMasks.Length; i++)
        {
            bool canWalk = !Physics.CheckSphere(worldPosition, checkRadius, unwalkableMasks[i]);
            if(canWalk == false)
            {
                return false;
            }
        }
        return true;
    }
}
