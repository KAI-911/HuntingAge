using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MaterialDataList))]
public class ItemDataEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(((MaterialDataList)target).modifyValues)
        {
            if(GUILayout.Button("Save Chenge"))
            {
                ((MaterialDataList)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if(GUILayout.Button("PrintDictionary"))
        {
            ((MaterialDataList)target).PrintDictionary();
        }
    }
}
