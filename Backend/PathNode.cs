using System.Collections;
using System.Collections.Generic;

public class PathNode
{
    public int column;
    public int row;

    public int gCost;   //Walking Cost from the start node
    public int hCost;   //Heuristic Cost to reach end node
    public int fCost;   //F = G + H

    private object nodeLock = new object();

    private bool isWalkable;
    public bool IsWalkable => isWalkable;

    private bool hasRobot;
    public bool HasRobot => hasRobot;

    public PathNode cameFromNode;

    public PathNode(int column, int row)
    {
        this.column = column;
        this.row = row;
        isWalkable = true;
        hasRobot = false;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        lock (nodeLock) 
        {
            this.isWalkable = isWalkable;
        };
    }

    public void SetHasRobot(bool hasRobot)
    {
        lock (nodeLock)
        {
            this.hasRobot = hasRobot;
        };
    }

    public override string ToString()
    {
        return column + "," + row;
    }
}
