using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ItemDataList))]

public class ItemDataEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (((ItemDataList)target).modifyValues)
        {
            if (GUILayout.Button("Save Chenge"))
            {
                ((ItemDataList)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if (GUILayout.Button("PrintDictionary"))
        {
            ((ItemDataList)target).PrintDictionary();
        }
    }
}
