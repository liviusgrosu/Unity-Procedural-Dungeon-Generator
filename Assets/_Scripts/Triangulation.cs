using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Triangulation
{
    public class Vertex
    {
        public float x, y;
        public Vertex(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"[{x},{y}]";
        }
    }

    public class Edge
    {
        public Vertex u, v;

        public Edge(Vertex u, Vertex v)
        {
            this.u = u;
            this.v = v;
        }
    }

    public class Triangle
    {
        public Vertex v0, v1, v2;
        
        public Triangle (Vertex v0, Vertex v1, Vertex v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }
    }
    private List<Vertex> vertices;
    private List<Edge> edges;
    private List<Triangle> triangles;
    public static event Action<Vector2> DisplayPoint;
    public static event Action ClearDisplayedPoints;

    public Triangulation(List<Vector2> pointList)
    {
        vertices = new List<Vertex>();
        edges = new List<Edge>();
        triangles = new List<Triangle>();

        // TODO: might need to sort the vertices by the X values

        // Convert pointList to list of vertices
        foreach(Vector2 point in pointList)
        {
            vertices.Add(new Vertex(point.x, point.y));
        }

        // Create super triangle
        float minX = pointList[0].x;
        float maxX = pointList[0].x;

        float minY = pointList[0].y;
        float maxY = pointList[0].y;

        foreach(Vector2 point in pointList)
        {
            if (point.x < minX)
            {
                minX = point.x;
            }
            if (point.x > maxX)
            {
                maxX = point.x;
            }
            if (point.y < minY)
            {
                minY = point.y;
            }
            if (point.y > maxY)
            {
                maxY = point.y;
            }
        }
        
        Vertex v0 = new Vertex(minX - 1f, minY - 1f);
        Vertex v1 = new Vertex(maxX + 1f, minY - 1f);
        Vertex v2 = new Vertex(maxX / 2f, maxY + 1f);

        DisplayPoint?.Invoke(new Vector2(v0.x, v0.y));
        DisplayPoint?.Invoke(new Vector2(v1.x, v1.y));
        DisplayPoint?.Invoke(new Vector2(v2.x, v2.y));

        Triangle superTriangle = new Triangle(v0, v1, v2);
        triangles.Add(superTriangle);

        foreach(Vertex vertex in vertices)
        {
            List<Triangle> badTriangles = new List<Triangle>();
            foreach(Triangle triangle in triangles)
            {
                if (IsPointInCirlce(triangle, vertex))
                {
                    badTriangles.Add(triangle);
                    DisplayPoint?.Invoke(new Vector2(vertex.x, vertex.y));
                    return;
                }
            }
        }
    }

    private bool IsPointInCirlce(Triangle triangle, Vertex vertex)
    {
        // TODO: Make triangle counter-clockwise order

        float ax = triangle.v0.x - vertex.x;
        float ay = triangle.v0.y - vertex.y;

        float bx = triangle.v1.x - vertex.x;
        float by = triangle.v1.y - vertex.y;

        float cx = triangle.v2.x - vertex.x;
        float cy = triangle.v2.y - vertex.y;

        return  (((ax * ax) + (ay * ay)) * ((bx * cy) - (cx * by)) -
                ((bx * bx) + (by * by)) * ((ax * cy) - (cx * ay)) +
                ((cx * cx) + (cy * cy)) * ((ax * by) - (by * ay))) > 0;
    }
}