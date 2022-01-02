using System;
using System.Collections;
using System.Collections.Generic;

public class Grid
{

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private List<PathNode> openList;    //Nodes queued up for searching
    private List<PathNode> closedList;  //Nodes that have already been searched

    private readonly object lockCompleted = new object();

    private PathNode[,] gridArray; 

    private object gridLock = new object();
    
    private bool printToConsole;

    private string warehouseName;
    
    public string WarehouseName => warehouseName;

    public Grid(string warehouseName, int rows, int columns)
    {
        this.warehouseName = warehouseName;
        SetUpGrid(rows, columns);
    }

    public struct GridLocation
    {
        public GridLocation(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
        public int row;
        public int column;
    }

    private void SetUpGrid(int rows, int columns)
    {
        gridArray = new PathNode[rows, columns];
        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                gridArray[row, column] = new PathNode(column, row);
            }
        }
    }

    /*
    * @param startRow - Row Number of start position 
    * @param startColumn - Column Number of start position
    * @param endRow - Row number of end position
    * @param endColumn - Column number of end position
    * @return A list of PathNodes, in order of a valid path from start position to end position
    */
    public List<PathNode> FindPath(int startRow, int startColumn, int endRow, int endColumn)
    {
        lock (lockCompleted)
        {
            PathNode startNode = gridArray[startRow, startColumn];
            PathNode endNode = gridArray[endRow, endColumn];

            openList = new List<PathNode> { startNode };
            closedList = new List<PathNode>();

            //Reset
            for (int column = 0; column < gridArray.GetLength(1); column++)
            {
                for (int row = 0; row < gridArray.GetLength(0); row++)
                {
                    PathNode pathNode = gridArray[row, column];
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    //reached final node
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neightbourNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neightbourNode)) continue;
                    if (!neightbourNode.IsWalkable)
                    {
                        closedList.Add(neightbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neightbourNode);
                    if (tentativeGCost < neightbourNode.gCost)
                    {
                        neightbourNode.cameFromNode = currentNode;
                        neightbourNode.gCost = tentativeGCost;
                        neightbourNode.hCost = CalculateDistanceCost(neightbourNode, endNode);
                        neightbourNode.CalculateFCost();

                        if (!openList.Contains(neightbourNode))
                        {
                            openList.Add(neightbourNode);
                        }
                    }
                }
            }

            //Out of nodes on the openList
            return null;
        }
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.column - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(currentNode.row, currentNode.column - 1));
            // Left Down
            if (currentNode.row - 1 >= 0) neighbourList.Add(GetNode(currentNode.row - 1, currentNode.column - 1));
            // Left Up
            if (currentNode.row + 1 < gridArray.GetLength(0)) neighbourList.Add(GetNode(currentNode.row + 1, currentNode.column - 1));
        }
        if (currentNode.column + 1 < gridArray.GetLength(1))
        {
            // Right
            neighbourList.Add(GetNode(currentNode.row, currentNode.column + 1));
            // Right Down
            if (currentNode.row - 1 >= 0) neighbourList.Add(GetNode(currentNode.row - 1, currentNode.column + 1));
            // Right Up
            if (currentNode.row + 1 < gridArray.GetLength(0)) neighbourList.Add(GetNode(currentNode.row + 1, currentNode.column + 1));
        }
        // Down
        if (currentNode.row - 1 >= 0) neighbourList.Add(GetNode(currentNode.row - 1, currentNode.column));
        // Up
        if (currentNode.row + 1 < gridArray.GetLength(0)) neighbourList.Add(GetNode(currentNode.row + 1, currentNode.column));

        return neighbourList;
    }

    public PathNode GetNode(int row, int column)
    {
        return gridArray[row, column];
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Math.Abs(a.column - b.column);
        int yDistance = Math.Abs(a.row - b.row);
        int remaining = Math.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Math.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;

    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    public void SetUnWalkable(int row, int column, bool isWalkable)
    {
        lock (gridLock)
        {
            gridArray[row, column].SetIsWalkable(isWalkable);
        };
    }

    public void SetHasRobot(int row, int column, bool hasRobot)
    {
        lock (gridLock)
        {
            gridArray[row, column].SetHasRobot(hasRobot);
        };
    }

    public void SetPrintToConsoleFlag(bool printToConsole){
        this.printToConsole = printToConsole;
        PrintGridToConsole(printToConsole);
    }

    public void PrintGridToConsole(bool isStocking)
    {
        if(printToConsole){
            Console.Clear();
            Console.WriteLine(warehouseName + ":\n");

            string gridToPrint = "";

            for (int row = gridArray.GetLength(0) - 1; row >= -1; row--)
            {
                if (row == -1)
                {
                    for (int column = -1; column < gridArray.GetLength(1); column++)
                    {
                        if(column == -1)
                        {
                            gridToPrint += " ";
                        }
                        else
                        {
                            gridToPrint += column.ToString();
                        }
                        gridToPrint += " ";
                    }
                }
                else
                {
                    for (int column = -1; column < gridArray.GetLength(1); column++)
                    {
                        if (column == -1)
                        {
                            gridToPrint += row.ToString();
                        }
                        else if (!gridArray[row, column].IsWalkable)
                        {
                            gridToPrint += "|";
                        }
                        else if (gridArray[row, column].HasRobot)
                        {
                            if (isStocking)
                            {
                                gridToPrint += "S";
                            }
                            else
                            {
                                gridToPrint += "R";
                            }                        
                        }
                        else
                        {
                            gridToPrint += " ";
                        }
                        gridToPrint += " ";
                    }
                }
                gridToPrint += "\n";
            }
            Console.WriteLine(gridToPrint);
        }
    }

}
