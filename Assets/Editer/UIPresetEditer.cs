using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(UIPresetDataList))]

public class UIPresetEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (((UIPresetDataList)target).modifyValues)
        {
            if (GUILayout.Button("Save Chenge"))
            {
                ((UIPresetDataList)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if (GUILayout.Button("PrintDictionary"))
        {
            ((UIPresetDataList)target).PrintDictionary();
        }
    }
}
