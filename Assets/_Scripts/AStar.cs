using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public class Node
    {
        public float f, g, h;

        // public float x, y;
        public Vector2 pos;

        public Node(float x, float y)
        {
            pos = new Vector2(x, y);
            Update(0, 0);
        }

        public void Update(float g, float h)
        {
            this.g = g;
            this.h = h;

            f = g + h;
        }

        public bool Equals(Node other)
        {
            return pos.Equals(other);
        }
    }

    private List<Node> openSet;
    private Node startNode, endNode;

    public AStar(List<MST.Path> paths)
    {
        openSet = new List<Node>();
        
        // TEMP Just start with the first path
        MST.Path tempFirstPath = paths[0];

        startNode = new Node(tempFirstPath.a.vertex.x, tempFirstPath.a.vertex.y);
        endNode = new Node(tempFirstPath.b.vertex.x, tempFirstPath.b.vertex.y);

        openSet.Add(startNode);
        CalculateSurrondingNodes(startNode);
        
        // while (openSet.Count != 0)
        // {
        //     // Find the lowest cost f-cost 
        //     Node node = FindLowestFCostNode();
        //     if (node.Equals(endNode))
        //     {
        //         Debug.Log("Found a path...");
        //         return; 
        //     }

        //     openSet.Remove(node);
        //     int fuck = 3;
        // }
    }

    private float GetDistanceToEnd(Node compareNode)
    {
        return Vector2.Distance(compareNode.pos, endNode.pos);
    }

    private void CalculateSurrondingNodes(Node centreNode)
    {
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                float xPos = centreNode.pos.x + x;
                float yPos = centreNode.pos.y + y;

                float gCost = 0;
                float hCost = 0;

                // Update existing node
                Node existingNode = FindNodeInSet(openSet, xPos, yPos);
                if (existingNode != null)
                {
                    // Update the gCost
                    gCost = Vector2.Distance(centreNode.pos, existingNode.pos) + centreNode.g;
                    hCost = existingNode.h;
                    existingNode.Update(gCost, hCost);
                    continue;    
                }

                // Create and initialize the set
                Node adjacentNode = new Node(xPos, yPos);
                gCost = Vector2.Distance(centreNode.pos, adjacentNode.pos) + centreNode.g;
                hCost = GetDistanceToEnd(adjacentNode);
                adjacentNode.Update(gCost, hCost);
                openSet.Add(adjacentNode);
            }
        }
    }

    private Node FindNodeInSet(List<Node> set, float x, float y)
    {
        foreach(Node node in set)
        {
            float nodeX = node.pos.x;
            float nodeY = node.pos.y;

            if (nodeX == x && nodeY == y)
            {
                return node;
            }
        }
        return null;
    }

    private Node FindLowestFCostNode()
    {
        // Find the lowest f-costing node 
        List<Node> potentialNodes = new List<Node>();
        float lowestFCost = Mathf.Infinity;
        foreach(Node node in openSet)
        {
            if (node.f < lowestFCost)
            {
                potentialNodes.Clear();
                potentialNodes.Add(node);
                lowestFCost = node.f;
            }
            else if(node.f == lowestFCost)
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
                if (node.h < lowestHCost)
                {
                    lowestHCostNodes.Clear();
                    lowestHCostNodes.Add(node);
                    lowestHCost = node.h;
                }
                if (node.h == lowestHCost)
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