using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(WeaponDataList))]
public class WeaponDataEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (((WeaponDataList)target).modifyValues)
        {
            if (GUILayout.Button("Save Chenge"))
            {
                ((WeaponDataList)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if (GUILayout.Button("PrintDictionary"))
        {
            ((WeaponDataList)target).PrintDictionary();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if (GUILayout.Button("add"))
        {
            ((WeaponDataList)target).add("weapon100");
        }
    }
}