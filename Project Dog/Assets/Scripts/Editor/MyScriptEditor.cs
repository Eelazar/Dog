using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Assistant))]
public class MyScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as Assistant;


        EditorGUILayout.LabelField("Sound Clips", EditorStyles.boldLabel);
        myScript.notification = (AudioClip)EditorGUILayout.ObjectField("Notification: ", myScript.notification, typeof(AudioClip), true);
        myScript.newNotification = (AudioClip)EditorGUILayout.ObjectField("New Notification: ", myScript.newNotification, typeof(AudioClip), true);

        EditorGUILayout.LabelField("Assistant Pause Durations", EditorStyles.boldLabel);
        myScript.letterPause = EditorGUILayout.FloatField("Letter: ", myScript.letterPause);
        myScript.shortPause = EditorGUILayout.FloatField("Short: ", myScript.shortPause);
        myScript.normalPause = EditorGUILayout.FloatField("Normal: ", myScript.normalPause);
        myScript.longPause = EditorGUILayout.FloatField("Long: ", myScript.longPause);

        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        myScript.assistant_Window = (GameObject)EditorGUILayout.ObjectField("Assistant Window: ", myScript.assistant_Window, typeof(GameObject), true);
        myScript.assistant_Text_Red = (TMP_Text)EditorGUILayout.ObjectField("Red TextField: ", myScript.assistant_Text_Red, typeof(TMP_Text), true);
        myScript.assistant_Text_Blue = (TMP_Text)EditorGUILayout.ObjectField("Blue TextField: ", myScript.assistant_Text_Blue, typeof(TMP_Text), true);
        myScript.assistant_Text_White = (TMP_Text)EditorGUILayout.ObjectField("White TextField: ", myScript.assistant_Text_White, typeof(TMP_Text), true);

        EditorGUILayout.LabelField("Animation Stuff", EditorStyles.boldLabel);
        myScript.animCurve = EditorGUILayout.CurveField("Assistant AnimCurve: ", myScript.animCurve);
        myScript.animDuration = EditorGUILayout.FloatField("Animation duration: ", myScript.animDuration);

        EditorGUILayout.LabelField("Docked Mode", EditorStyles.boldLabel);
        myScript.docked = GUILayout.Toggle(myScript.docked, "Docked");
        if (myScript.docked)
        {
            myScript.assistant_Icon = (GameObject)EditorGUILayout.ObjectField("Assistant Icon: ", myScript.assistant_Icon, typeof(GameObject), true);
            myScript.log_Panel = (GameObject)EditorGUILayout.ObjectField("Log Panel: ", myScript.log_Panel, typeof(GameObject), true);
            myScript.logEntry_Prefab = (GameObject)EditorGUILayout.ObjectField("Neptune-Entry Prefab: ", myScript.logEntry_Prefab, typeof(GameObject), true);
            myScript.logEntryAmount = EditorGUILayout.IntField("Log Entry Amount: ", myScript.logEntryAmount);
            myScript.textAnimPause = EditorGUILayout.FloatField("Log Char Pause: ", myScript.textAnimPause);
        }
    }
}