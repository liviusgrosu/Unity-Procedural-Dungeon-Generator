using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MST
{
    public class Point
    {
        public Triangulation.Vertex vertex;
        public Dictionary<Triangulation.Vertex, float> connectingPaths;

        public Point(Triangulation.Vertex vertex)
        {
            this.vertex = vertex;
            connectingPaths = new Dictionary<Triangulation.Vertex, float>();
        }
    }

    private List<Point> points;
    private List<Point> visitedPoints;

    public MST(List<Triangulation.Vertex> vertices, List<Triangulation.Edge> edges)
    {
        points = new List<Point>();
        // Need edges and vertices
        CreateGraph(vertices, edges);
    }

    private void CreateGraph(List<Triangulation.Vertex> vertices, List<Triangulation.Edge> edges)
    {
        // Create the starting point
        foreach(Triangulation.Vertex vertex in vertices)
        {
            Point point = new Point(vertex);
            FindAllEdgesConnected(point, edges);
            points.Add(point);
        }
    }

    private void FindAllEdgesConnected(Point point, List<Triangulation.Edge> edges)
    {        
        foreach(Triangulation.Edge edge in edges)
        {            
            Triangulation.Vertex connectingPoint;

            if (edge.u.x == point.vertex.x && edge.u.y == point.vertex.y)
            {
                connectingPoint = edge.v;
            }
            else if (edge.v.x == point.vertex.x && edge.v.y == point.vertex.y)
            {
                connectingPoint = edge.u;
            }
            else
            {
                continue;
            }
            
            float distanceCost = Vector2.Distance(new Vector2(point.vertex.x, point.vertex.y), new Vector2(connectingPoint.x, connectingPoint.y));
            point.connectingPaths.Add(connectingPoint, distanceCost);
        }
    }
}