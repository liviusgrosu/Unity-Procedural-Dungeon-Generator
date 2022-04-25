using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugVisualization : MonoBehaviour
{
    private GenerateDungeon generateDungeon;

    private bool showResultingPath, showPathsBuffer, showAStar;

    private void Awake()
    {
        generateDungeon = GetComponent<GenerateDungeon>();
        showResultingPath = false;
        showPathsBuffer = false;
        showAStar = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            showPathsBuffer = !showPathsBuffer;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            showResultingPath = !showResultingPath;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            showAStar = !showAStar;
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

        if (showResultingPath && generateDungeon.mst.resultingPath != null)
        {
            foreach(MST.Path path in generateDungeon.mst.resultingPath)
            {
                Vector3 pointA = new Vector3(path.a.vertex.x, 0f, path.a.vertex.y);
                Vector3 pointB = new Vector3(path.b.vertex.x, 0f, path.b.vertex.y);

                Debug.DrawLine(pointA, pointB, Color.green);
            }
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (showAStar && generateDungeon.aStar.totalPaths != null)
        {
            foreach(List<AStar.Node> currentPath in generateDungeon.aStar.totalPaths)
            {
                foreach(AStar.Node node in currentPath)
                {
                    if (generateDungeon.CheckHallwayOverlapsRoom(node.pos))
                    {
                        continue;
                    }
                    Vector3 pos = new Vector3(node.pos.x, 0, node.pos.y);
                    Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                }
            }
        }
    }
}