using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DoorTriggerInteraction))]
class LabelHandle : Editor
{
    private static GUIStyle labelStyle;

    private void OnEnable()
    {
        labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white;
        labelStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnSceneGUI()
    {
        DoorTriggerInteraction door = (DoorTriggerInteraction)target;

        Handles.BeginGUI();
        Handles.Label(
            door.transform.position + new Vector3(0f, 4f, 0f),
            door.CurrentDoorPosition.ToString(),
            labelStyle
        );
        Handles.EndGUI();
    }
}
