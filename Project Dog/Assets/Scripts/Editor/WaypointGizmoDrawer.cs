using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaypointGizmoDrawer
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    static void DrawGizmoForMyScript(Waypoint waypoint, GizmoType gizmoType)
    {
        Vector3 position = waypoint.transform.position;

        Vector3 nextPosition = waypoint.next.transform.position;

        Debug.DrawLine(position, nextPosition, Color.black);

        UnityEditor.Handles.BeginGUI();

        var restoreColor = GUI.color;

        GUI.color = Color.black;

        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(position);

        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
        {
            GUI.color = restoreColor;
            UnityEditor.Handles.EndGUI();
            return;
        }
        string text = waypoint.waitTime.ToString() + "s";

        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text)) * 2;
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        GUI.color = restoreColor;
        UnityEditor.Handles.EndGUI();
    }
}
