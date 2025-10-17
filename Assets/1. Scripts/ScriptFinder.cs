using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScriptFinder : EditorWindow
{
    MonoScript script;

    [MenuItem("Tools/Script Finder")]
    public static void ShowWindow()
    {
        GetWindow<ScriptFinder>("Script Finder");
    }

    private void OnGUI()
    {
        script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false, GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Find in Scene"))
        {
            if (script == null) return;

            var type = script.GetClass();
            var objs = FindObjectsOfType(type);
            foreach (var obj in objs)
            {
                Debug.Log($"Found in: {((MonoBehaviour)obj).gameObject.name}", ((MonoBehaviour)obj).gameObject);
            }
        }
    }
}
