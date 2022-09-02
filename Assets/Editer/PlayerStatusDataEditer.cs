using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PlayerStatusData))]

public class PlayerStatusDataEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (((PlayerStatusData)target).modifyValues)
        {
            if (GUILayout.Button("Save Chenge"))
            {
                ((PlayerStatusData)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if (GUILayout.Button("PrintDictionary"))
        {
            ((PlayerStatusData)target).PrintDictionary();
        }
    }
}
