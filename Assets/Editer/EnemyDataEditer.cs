using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyDataList))]

public class EnemyDataEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (((EnemyDataList)target).modifyValues)
        {
            if (GUILayout.Button("Save Chenge"))
            {
                ((EnemyDataList)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if (GUILayout.Button("PrintDictionary"))
        {
            ((EnemyDataList)target).PrintDictionary();
        }
    }

}
