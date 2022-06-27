using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(QuestDataList))]

public class QuestDataEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (((QuestDataList)target).modifyValues)
        {
            if (GUILayout.Button("Save Chenge"))
            {
                ((QuestDataList)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if (GUILayout.Button("PrintDictionary"))
        {
            ((QuestDataList)target).PrintDictionary();
        }
    }
}
