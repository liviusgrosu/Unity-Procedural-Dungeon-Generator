using System;
using System.Linq;
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

        public bool Equals(Vertex other)
        {
            return x == other.x && y == other.y;
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
        public Vertex a, b, c;

        public Triangle (Vertex a, Vertex b, Vertex c)
        {
            // Order these in counter clock wise
            (this.a, this.b, this.c) = SortVertices(a, b, c);
        }
    }
    public List<Vertex> vertices;
    public List<Edge> allEdges;
    private List<Triangle> triangulation;
    public List<Triangle> finishedTriangulation;

    public Triangulation(List<Vector2> pointList)
    {
        vertices = new List<Vertex>();
        triangulation = new List<Triangle>();
        finishedTriangulation = new List<Triangle>();
        allEdges = new List<Edge>();

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

        // Find the minimun point
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
        
        // Create the super triangle given its points
        Vertex v0 = new Vertex(minX - 100f, minY - 100f);
        Vertex v1 = new Vertex(maxX + 100f, minY - 100f);
        Vertex v2 = new Vertex(maxX / 2f, maxY + 100f);

        Triangle superTriangle = new Triangle(v0, v1, v2);
        
        triangulation.Add(superTriangle);

        foreach(Vertex vertex in vertices)
        {
            List<Triangle> badTriangles = new List<Triangle>();
            // If the point exists in the circumcircle of a triangle then the triangle is considered bad
            foreach(Triangle triangle in triangulation)
            {
                if (IsPointInCirlce(triangle, vertex))
                {
                    badTriangles.Add(triangle);
                }
            }
            List<Edge> polygon = new List<Edge>();
            
            foreach(Triangle triangle in badTriangles)
            {
                // Get all the edges of the triangle
                List<Edge> edges = new List<Edge>();
                edges.Add(new Edge(triangle.a, triangle.b));
                edges.Add(new Edge(triangle.b, triangle.c));
                edges.Add(new Edge(triangle.c, triangle.a));

                foreach(Edge edge in edges)
                {
                    // Check if edge is shared by another bad triangle
                    if (!CheckIfEdgeIsShared(edge, badTriangles.Except(new List<Triangle>{triangle}).ToList()))
                    {
                        // Add that edge to the triangle
                        polygon.Add(edge);
                    }
                }
            }

            // Remove all bad triangles from the triangulation list
            foreach(Triangle triangle in badTriangles)
            {
                triangulation.Remove(triangle);
            }

            // Add the new triangle from the polygon to the triangulation list
            foreach(Edge edge in polygon)
            {
                Triangle newTriangle = new Triangle(edge.u, edge.v, vertex);
                triangulation.Add(newTriangle);
            }
        }

        // Add triangle to finished list if it touches the super triangle
        foreach(Triangle triangle in triangulation)
        {
            if (!CheckIfVerticesShared(triangle, superTriangle))
            {
                finishedTriangulation.Add(triangle);
            }
        }

        // Remove any overlapping edges
        RemoveOverlappingEdges();
    }

    private bool IsPointInCirlce(Triangle triangle, Vertex vertex)
    {
        // Check if point lies within the circumcircle of a triangle
        float ax = triangle.a.x - vertex.x;
        float ay = triangle.a.y - vertex.y;

        float bx = triangle.b.x - vertex.x;
        float by = triangle.b.y - vertex.y;

        float cx = triangle.c.x - vertex.x;
        float cy = triangle.c.y - vertex.y;

        // Calculate the determinant and check if its greater then 0
        // If it is then the point lies within the triangles circumcircle
        return  ((ax * ax + ay * ay) * (bx * cy - cx * by) -
                (bx * bx + by * by) * (ax * cy - cx * ay) +
                (cx * cx + cy * cy) * (ax * by - bx * ay)) > 0;
    }

    private bool CheckIfEdgeIsShared(Edge edge, List<Triangle> triangles)
    {
        // Check the edge is shared by another triangle
        foreach(Triangle triangle in triangles)
        {
            if (triangle.a == edge.u && triangle.b == edge.v || triangle.a == edge.v && triangle.b == edge.u ||
                triangle.b == edge.u && triangle.c == edge.v || triangle.b == edge.v && triangle.c == edge.u ||
                triangle.c == edge.u && triangle.a == edge.v || triangle.c == edge.v && triangle.a == edge.u)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckIfEdgeIsShared(Edge edge)
    {
        // Check if edge exists in the edge list
        foreach(Edge currEdge in allEdges)
        {
            if ((edge.u.x == currEdge.u.x && edge.u.y == currEdge.u.y &&
                edge.v.x == currEdge.v.x && edge.v.y == currEdge.v.y) ||
                (edge.u.x == currEdge.v.x && edge.u.y == currEdge.v.y &&
                edge.v.x == currEdge.u.x && edge.v.y == currEdge.u.y))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckIfVerticesShared(Triangle triangleA, Triangle triangleB)
    {
        // Check if two triangle share a vertex
        return (triangleA.a.Equals(triangleB.a) || triangleA.a.Equals(triangleB.b) || triangleA.a.Equals(triangleB.c) ||
                triangleA.b.Equals(triangleB.a) || triangleA.b.Equals(triangleB.b) || triangleA.b.Equals(triangleB.c) ||
                triangleA.c.Equals(triangleB.a) || triangleA.c.Equals(triangleB.b) || triangleA.c.Equals(triangleB.c)
                );
    }

    private static Vertex FindCenteroid(Vertex a, Vertex b, Vertex c)
    {
        // Find the centre of a triangle
        float x = 0;
        float y = 0;

        x = (a.x + b.x + c.x) / 3f;
        y = (a.y + b.y + c.y) / 3f;

        return new Vertex(x, y); 
    }

    public static (Vertex, Vertex, Vertex) SortVertices(Vertex a, Vertex b, Vertex c)
    {
        // Sort vertices in a clockwise order such that the function IsPointInCirlce works properly
        List<Vertex> newVertices = new List<Vertex> { a, b, c };
        Vertex centre = FindCenteroid(a, b, c);        

        Dictionary<Vertex, int> vertexAngle = new Dictionary<Vertex, int>();

        for(int i = 0; i < newVertices.Count(); i++)
        {
            int angleMax = 0;
            for(int j = 0; j < newVertices.Count(); j++)
            {
                if (i == j)
                {
                    continue;
                }
                // Get the angle between the vertex and centre
                float a1 = ((Mathf.Rad2Deg * Mathf.Atan2( newVertices[i].x - centre.x, newVertices[i].y - centre.y )) + 360) % 360;
                float a2 = ((Mathf.Rad2Deg * Mathf.Atan2( newVertices[j].x - centre.x, newVertices[j].y - centre.y )) + 360) % 360;
                int angleResult = (int)(a1 - a2);
                if (angleResult > angleMax)
                {
                    angleMax = angleResult;
                }
            }
            vertexAngle.Add(newVertices[i], angleMax);
        }

        // Sort vertices by their angles
        Dictionary<Vertex, int> sortedAngles = vertexAngle.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        return (sortedAngles.ElementAt(0).Key, sortedAngles.ElementAt(1).Key, sortedAngles.ElementAt(2).Key);
    }

    private void RemoveOverlappingEdges()
    {
        // Remove any edges that overlap
        foreach(Triangle triangle in finishedTriangulation)
        {
            Edge ab = new Edge(triangle.a, triangle.b);
            Edge bc = new Edge(triangle.b, triangle.c);
            Edge ca = new Edge(triangle.c, triangle.a);

            if (!CheckIfEdgeIsShared(ab))
            {
                allEdges.Add(ab);
            }

            if (!CheckIfEdgeIsShared(bc))
            {
                allEdges.Add(bc);
            }

            if (!CheckIfEdgeIsShared(ca))
            {
                allEdges.Add(ca);
            }
        }
    }
}