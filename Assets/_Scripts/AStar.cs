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
            // this.x = x;
            // this.y = y;
            pos = new Vector2(x, y);
            Update(0, 0);
        }

        public void Update(float g, float h)
        {
            this.g = g;
            this.h = h;

            f = g + h;
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
        int fuck = 5;
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

                // Create new node
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
}