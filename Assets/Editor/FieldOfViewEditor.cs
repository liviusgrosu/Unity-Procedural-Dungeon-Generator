using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;

        // Draw the far sight radius circle
        Handles.color = Color.yellow;
        Handles.DrawWireArc(fov.transform.position,
                            Vector3.up,
                            fov.transform.forward,
                            360f,
                            fov.Radius);

    }
}