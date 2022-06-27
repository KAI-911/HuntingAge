using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(QuestHolder))]

public class QuestHolderEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (((QuestHolder)target).modifyValues)
        {
            if (GUILayout.Button("Save Chenge"))
            {
                ((QuestHolder)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if (GUILayout.Button("PrintDictionary"))
        {
            ((QuestHolder)target).PrintDictionary();
        }
    }

}
