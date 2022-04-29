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
        showResultingPath = false;
        showPathsBuffer = false;
    }

    private void Update()
    {
        // Toggle the MST visualization
        if (Input.GetKeyDown(KeyCode.T))
        {
            showResultingPath = !showResultingPath;
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

        // Toggle the compliment-MST visualization
        if (Input.GetKeyDown(KeyCode.Y))
        {
            showPathsBuffer = !showPathsBuffer;
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