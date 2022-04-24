using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public class Node
    {
        public Vector2 pos;

        public Node(float x, float y)
        {
            pos = new Vector2(x, y);
        }

        public bool Equals(Node other)
        {
            return pos.Equals(other);
        }
    }

    private List<Node> openSet;
    private Dictionary<Node, float> gCosts, fCosts, cameFrom;
    private Node startNode, endNode;

    public AStar(List<MST.Path> paths)
    {
        openSet = new List<Node>();
        
        // TEMP Just start with the first path
        MST.Path tempFirstPath = paths[0];

        startNode = new Node(tempFirstPath.a.vertex.x, tempFirstPath.a.vertex.y);
        endNode = new Node(tempFirstPath.b.vertex.x, tempFirstPath.b.vertex.y);

        openSet.Add(startNode);

        // cameFrom = new Dictionary<Node, float>();

        gCosts = new Dictionary<Node, float>();
        gCosts[startNode] = 0;

        fCosts = new Dictionary<Node, float>();
        fCosts[startNode] = GetDistanceToEnd(startNode);

        while (openSet.Count != 0)
        {
            Node current = FindLowestFCostNode();
            if (current.Equals(endNode))
            {
                Debug.Log("found some shit");
                return;
            }
            openSet.Remove(current);
            List<Node> adjacentNodes = GetAdjacentNodes(current);

            foreach (Node adjacentNode in adjacentNodes)
            {
                float tenativeGCost = gCosts[current] + Vector2.Distance(current.pos, adjacentNode.pos);
                if (tenativeGCost < gCosts[adjacentNode])
                {
                    // cameFrom[adjacentNode] = current;
                    gCosts[adjacentNode] = tenativeGCost;
                    fCosts[adjacentNode] = tenativeGCost + GetDistanceToEnd(adjacentNode);
                    if (!NodeExistsInSet(adjacentNode))
                    {
                        openSet.Add(adjacentNode);
                    }
                }
            }
        }
    }

    private float GetDistanceToEnd(Node compareNode)
    {
        return Vector2.Distance(compareNode.pos, endNode.pos);
    }

    private List<Node> GetAdjacentNodes(Node centreNode)
    {
        List<Node> adjacentNodes = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                float xPos = centreNode.pos.x + x;
                float yPos = centreNode.pos.y + y;

                adjacentNodes.Add(new Node(xPos, yPos));
            }
        }

        return adjacentNodes;
    }

    private bool NodeExistsInSet(Node compareTo)
    {
        foreach(Node node in openSet)
        {
            float nodeX = node.pos.x;
            float nodeY = node.pos.y;

            if (nodeX == compareTo.pos.x && nodeY == compareTo.pos.y)
            {
                return true;
            }
        }
        return false;
    }

    private Node FindLowestFCostNode()
    {
        // Find the lowest f-costing node 
        List<Node> potentialNodes = new List<Node>();
        float lowestFCost = Mathf.Infinity;
        foreach(Node node in openSet)
        {
            float nodeFCost = fCosts[node]; 
            if (nodeFCost < lowestFCost)
            {
                potentialNodes.Clear();
                potentialNodes.Add(node);
                lowestFCost = nodeFCost;
            }
            else if(nodeFCost == lowestFCost)
            {
                potentialNodes.Add(node);
            }
        }

        // If more then 1 node with the same f-cost are found then find the lowest h-cost 
        if (potentialNodes.Count != 1)
        {
            List<Node> lowestHCostNodes = new List<Node>();
            float lowestHCost = Mathf.Infinity;
            foreach(Node node in potentialNodes)
            {
                float nodeHCost = GetDistanceToEnd(node);
                if (nodeHCost < lowestHCost)
                {
                    lowestHCostNodes.Clear();
                    lowestHCostNodes.Add(node);
                    lowestHCost = nodeHCost;
                }
                if (nodeHCost == lowestHCost)
                {
                    lowestHCostNodes.Add(node);
                }
            }
            potentialNodes = lowestHCostNodes;
        }

        // If more then 1 node with the same h-cost are found then pick one randomly
        if (potentialNodes.Count != 1)
        {
            int randomElement = Random.Range(0, potentialNodes.Count);
            potentialNodes = new List<Node>{potentialNodes[randomElement]};
        }

        return potentialNodes[0];
    }
}