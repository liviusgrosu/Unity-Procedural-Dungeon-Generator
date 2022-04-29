using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public List<List<Node>> TotalPaths;
    private List<Node> openSet;
    private Dictionary<Node, Node> cameFrom;
    private Dictionary<Node, float> gCosts, fCosts;
    private Node startNode, endNode;

    public AStar(List<MST.Path> paths)
    {
        TotalPaths = new List<List<Node>>();
        openSet = new List<Node>();
        cameFrom = new Dictionary<Node, Node>();
        gCosts = new Dictionary<Node, float>();
        fCosts = new Dictionary<Node, float>();

        foreach(MST.Path path in paths)
        {
            TotalPaths.Add(GeneratePath(path));
        }
    }
    
    public class Node
    {
        public Vector2 pos;

        public Node(float x, float y)
        {
            pos = new Vector2(x, y);
        }

        public bool Equals(Node other)
        {
            return pos.Equals(other.pos);
        }
    }

    List<Node> GeneratePath(MST.Path path)
    {
        // Initialize
        startNode = new Node(path.a.vertex.x, path.a.vertex.y);
        endNode = new Node(path.b.vertex.x, path.b.vertex.y);

        // Clear any previosuly used lists
        openSet.Clear();
        openSet.Add(startNode);

        cameFrom.Clear();
        
        gCosts.Clear();
        gCosts[startNode] = 0;

        fCosts.Clear();
        fCosts[startNode] = GetDistanceToEnd(startNode);

        while (openSet.Count != 0)
        {
            // Find the lowest f score node and remove it from the open set
            Node current = FindLowestFCostNode();
            if (current.Equals(endNode))
            {
                // Return the lowest costing path
                return ReconstructPath(current);
            }

            openSet.Remove(current);
            List<Node> adjacentNodes = GetAdjacentNodes(current);

            // Go through all the adjacent nodes, update their costs and add them to the open set
            foreach (Node adjacentNode in adjacentNodes)
            {
                float tenativeGCost = GetCostValue(gCosts, current) + Vector2.Distance(current.pos, adjacentNode.pos);
                if (tenativeGCost < GetCostValue(gCosts, adjacentNode))
                {
                    cameFrom[adjacentNode] = current;
                    gCosts[adjacentNode] = tenativeGCost;
                    fCosts[adjacentNode] = tenativeGCost + GetDistanceToEnd(adjacentNode);
                    if (!NodeExistsInSet(adjacentNode))
                    {
                        openSet.Add(adjacentNode);
                    }
                }
            }
        }

        return null;
    }

    private List<Node> ReconstructPath(Node current)
    {
        // Traverse back on the previous nodes to get the least costing path
        List<Node> totalPath = new List<Node>() { current };
        Node previous;

        while (cameFrom.ContainsKey(current))
        {
            previous = current;
            current = cameFrom[current];
            totalPath.Add(current);
        }

        return totalPath;
    }

    private float GetDistanceToEnd(Node compareNode)
    {
        // Get distance to the end node from the current
        return Vector2.Distance(compareNode.pos, endNode.pos);
    }

    private List<Node> GetAdjacentNodes(Node centreNode)
    {
        // Find all adjacent nodes
        List<Node> adjacentNodes = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                // Only include directly adjacent nodes
                if (Mathf.Abs(x + y) != 1)
                {
                    // Dont inlcude the centre node
                    continue;
                }

                float xPos = centreNode.pos.x + x;
                float yPos = centreNode.pos.y + y;

                adjacentNodes.Add(new Node(xPos, yPos));
            }
        }

        return adjacentNodes;
    }

    private bool NodeExistsInSet(Node compareTo)
    {
        // Check if node exists in a set
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

    private float GetCostValue(Dictionary<Node, float> dict, Node key)
    {
        // Get cost of key
        return DictionaryExtensions.GetValueOrDefault(dict, key, Mathf.Infinity);
    }
}