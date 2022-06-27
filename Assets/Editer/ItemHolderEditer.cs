using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ItemHolder))]
public class ItemHolderEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(((ItemHolder)target).modifyValues)
        {
            if(GUILayout.Button("Save Chenge"))
            {
                ((ItemHolder)target).DesrializeDictionary();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);
        if(GUILayout.Button("PrintDictionary"))
        {
            ((ItemHolder)target).PrintDictionary();
        }
    }
}
