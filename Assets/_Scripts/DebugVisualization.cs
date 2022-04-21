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

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !startedShowingEdges)
        {
            if (generateDungeon.triangulation.result.Count != 0)
            {
                StartCoroutine(StartShowingEdges());
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
        foreach (Triangulation.Edge edge in generateDungeon.triangulation.edges)
        {
            latestEdgeToDraw = edge;
            yield return new WaitForSeconds(0.5f);
            edgesToDraw.Add(latestEdgeToDraw);
        }
    }
}