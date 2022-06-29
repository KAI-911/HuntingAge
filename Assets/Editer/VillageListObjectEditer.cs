using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(VillageData))]

public class VillageListObjectEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (((VillageData)target).modifyValues)
        {
            if (GUILayout.Button("Save Chenge"))
            {
                ((VillageData)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if (GUILayout.Button("PrintDictionary"))
        {
            ((VillageData)target).PrintDictionary();
        }
    }

}
