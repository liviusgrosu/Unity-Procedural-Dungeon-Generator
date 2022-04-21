using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MST
{
    public class Point
    {
        public Triangulation.Vertex vertex;
        public Dictionary<Triangulation.Vertex, float> connectingPoints;

        public Point()
        {
            vertex = null;
            connectingPoints = null;
        }

        public Point(Triangulation.Vertex vertex)
        {
            this.vertex = vertex;
            connectingPoints = new Dictionary<Triangulation.Vertex, float>();
        }

        public bool Equals(Point otherPoint)
        {
            return vertex.x == otherPoint.vertex.x && vertex.y == otherPoint.vertex.y;
        }
    }

    public class Path
    {
        public Point a, b;

        public Path(Point a, Point b)
        {
            this.a = a;
            this.b = b;
        }

        public bool Equals(Path otherPath)
        {
            return a.Equals(otherPath.a) && b.Equals(otherPath.b) || a.Equals(otherPath.b) && b.Equals(otherPath.a);
        }
    }

    private List<Point> points;
    private List<Point> visitedPoints;
    public List<Path> pathsBuffer;
    public List<Path> resultingPath;


    public MST(List<Triangulation.Vertex> vertices, List<Triangulation.Edge> edges, float randomHallwayChance)
    {
        points = new List<Point>();
        visitedPoints = new List<Point>();
        pathsBuffer = new List<Path>();
        resultingPath = new List<Path>();
        // Need edges and vertices
        CreateGraph(vertices, edges);
        // Add the first point to the visited nodes
        visitedPoints.Add(points[0]);
        // Start calculating the DMT
        TraversePath();
        AddRandomPaths(randomHallwayChance);
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

    private void TraversePath()
    {
        // Keep doing this until all the points have been visited
        while (visitedPoints.Count != points.Count)
        {
            Point lowestCostPoint = new Point();
            Point parentPointToLowest = new Point();
            float lowestCost = Mathf.Infinity;

            foreach(Point visitedPoint in visitedPoints)
            {
                Point nextPoint;
                float nextPointCost;
                (nextPoint, nextPointCost) = FindLowestPathOfPoint(visitedPoint);

                if (lowestCost > nextPointCost)
                {
                    lowestCost = nextPointCost;
                    parentPointToLowest = visitedPoint;
                    lowestCostPoint = nextPoint;
                }
            }

            if (lowestCostPoint != null)
            {
                // Add the lowest costed point to the visited list
                visitedPoints.Add(lowestCostPoint);
                Path newPath = new Path(parentPointToLowest, lowestCostPoint);
                // Capture the path between the lowest cost and the point next to it
                resultingPath.Add(newPath);
                RemoveFromPathBuffer(newPath);
            }
        }
    }

    private void AddRandomPaths(float randomHallwayChance)
    {
        foreach(Path path in pathsBuffer)
        {
            bool addPath = Random.Range(0.0f, 1.0f) > 1 - randomHallwayChance;
            if (addPath)
            {
                resultingPath.Add(path);
            }
        }
    }

    private (Point, float) FindLowestPathOfPoint(Point point)
    {
        // Find the lowest costing path to take from a point
        Point nextPoint = new Point();
        float lowestCost = Mathf.Infinity;

        foreach(KeyValuePair<Triangulation.Vertex, float> connectingPoint in point.connectingPoints)
        {
            Point currentPoint = GetPoint(connectingPoint.Key, points);
            // Check if connecting point is not already on the visitied list
            if (visitedPoints.Contains(currentPoint))
            {
                continue;
            }

            // If the current path has a lower casting cost then its currently the lowest costing path
            if (lowestCost > connectingPoint.Value)
            {
                lowestCost = connectingPoint.Value;
                nextPoint = currentPoint;
            }
        }
        return (nextPoint, lowestCost);
    }

    private void FindAllEdgesConnected(Point point, List<Triangulation.Edge> edges)
    {        
        // Get all points that connected to the current point
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
            point.connectingPoints.Add(connectingPoint, distanceCost);
            AddPathToBuffer(point, new Point(connectingPoint));
        }
    }

    private Point GetPoint(Triangulation.Vertex vertex, List<Point> pointList)
    {
        foreach(Point point in pointList)
        {
            if (point.vertex == vertex)
            {
                return point;
            }
        }
        return null;
    }

    private void AddPathToBuffer(Point a, Point b)
    {
        foreach(Path path in pathsBuffer)
        {
            if (path.a.Equals(a) && path.b.Equals(b) ||
                path.a.Equals(b) && path.b.Equals(a))
            {
                return;
            }
        }
        pathsBuffer.Add(new Path(a, b));
    }

    private void RemoveFromPathBuffer(Path pathToRemove)
    {
        foreach(Path path in pathsBuffer)
        {
            if (path.Equals(pathToRemove))
            {
                pathsBuffer.Remove(path);
                return; 
            }
        }
    }
}