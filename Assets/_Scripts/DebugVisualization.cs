using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugVisualization : MonoBehaviour
{
    private GenerateDungeon generateDungeon;
    private bool startedShowingEdges;
    private List<Triangulation.Edge> edgesToDraw;
    private Triangulation.Edge latestEdgeToDraw;

    private void Awake()
    {
        generateDungeon = GetComponent<GenerateDungeon>();
        edgesToDraw = new List<Triangulation.Edge>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !startedShowingEdges)
        {
            if (generateDungeon.triangulation.finishedTriangulation.Count != 0)
            {
                startedShowingEdges = true;
                StartCoroutine(StartShowingEdges());
            }
        }

        if (generateDungeon.mst.lowestCostPath != null)
        {
            foreach(MST.Path path in generateDungeon.mst.lowestCostPath)
            {
                Vector3 pointA = new Vector3(path.a.vertex.x, 0f, path.a.vertex.y);
                Vector3 pointB = new Vector3(path.b.vertex.x, 0f, path.b.vertex.y);

                Debug.DrawLine(pointA, pointB, Color.green);
            }
        }

        if (latestEdgeToDraw != null)
        {
            foreach(Triangulation.Edge edge in edgesToDraw)
            {
                Vector3 pointA = new Vector3(edge.u.x, 0f, edge.u.y);
                Vector3 pointB = new Vector3(edge.v.x, 0f, edge.v.y);

                Debug.DrawLine(pointA, pointB, Color.green);
            }

            Vector3 pointD = new Vector3(latestEdgeToDraw.u.x, 0f, latestEdgeToDraw.u.y);
            Vector3 pointE = new Vector3(latestEdgeToDraw.v.x, 0f, latestEdgeToDraw.v.y);
            Debug.DrawLine(pointD, pointE, Color.red);
        }
    }

    IEnumerator StartShowingEdges()
    {
        foreach (Triangulation.Edge edge in generateDungeon.triangulation.allEdges)
        {
            latestEdgeToDraw = edge;
            yield return new WaitForSeconds(0.25f);
            edgesToDraw.Add(latestEdgeToDraw);
        }
    }
}