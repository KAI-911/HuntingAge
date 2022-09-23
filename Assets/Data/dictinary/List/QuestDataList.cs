using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class QuestDataList : MonoBehaviour
{
    public QuestSaveData _saveData;

    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        foreach (var item in _saveData.Dictionary)
        {
            Debug.Log("Key: " + item.Key + " Value: " + item.Value);
        }
    }
}
[Serializable]
public struct QuestData
{
    /// <summary>
    /// Quest001 ˜A”Ô‚Å
    /// </summary>
    public string ID;
    public string Name;
    public bool ClearedFlg;
    public bool KeyQuestFlg;
    public ClearConditions Clear;
    public FailureConditions Failure;
    public Scene Field;
    public List<STRINGINT> TargetName;
    public List<STRINGINT> OtherName;
    public List<QuestRewardData> QuestRewardDatas;
}

[Serializable]
public struct QuestHolderData
{
    public List<string> Quests;
}
[Serializable] 
public struct STRINGINT
{
    public string name;
    public int number;
}
[Serializable]
public struct QuestRewardData
{
    public string name;
    public int number;
    public int probability;
}

