using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugVisualization : MonoBehaviour
{
    private GenerateDungeon generateDungeon;

    private bool showResultingPath, showPathsBuffer;

    private void Awake()
    {
        generateDungeon = GetComponent<GenerateDungeon>();
    }

    private void Update()
    {
        // if (generateDungeon.triangulation.allEdges != null)
        // {
        //     foreach(Triangulation.Edge edge in generateDungeon.triangulation.allEdges)
        //     {
        //         Vector3 pointA = new Vector3(edge.u.x, 0f, edge.u.y);
        //         Vector3 pointB = new Vector3(edge.v.x, 0f, edge.v.y);

        //         Debug.DrawLine(pointA, pointB, Color.gray);
        //     }
        // }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            showResultingPath = !showResultingPath;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            showPathsBuffer = !showPathsBuffer;
        }

        if (showResultingPath && generateDungeon.mst.resultingPath != null)
        {
            foreach(MST.Path path in generateDungeon.mst.resultingPath)
            {
                Vector3 pointA = new Vector3(path.a.vertex.x, 0f, path.a.vertex.y);
                Vector3 pointB = new Vector3(path.b.vertex.x, 0f, path.b.vertex.y);

                Debug.DrawLine(pointA, pointB, Color.green);
            }
        }

        if (showPathsBuffer && generateDungeon.mst.pathsBuffer != null)
        {
            foreach(MST.Path path in generateDungeon.mst.pathsBuffer)
            {
                Vector3 pointA = new Vector3(path.a.vertex.x, 0f, path.a.vertex.y);
                Vector3 pointB = new Vector3(path.b.vertex.x, 0f, path.b.vertex.y);

                Debug.DrawLine(pointA, pointB, Color.gray);
            }
        }
    }
}