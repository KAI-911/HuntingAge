using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDataList : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] QuestListObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<QuestData> values = new List<QuestData>();
    [SerializeField] Dictionary<string, QuestData> dictionary = new Dictionary<string, QuestData>();
    public bool modifyValues;
    public Dictionary<string, QuestData> Dictionary { get => dictionary; }


    private void Awake()
    {
        keys.Clear();
        values.Clear();
        for (int i = 0; i < Mathf.Min(DictionaryData.Keys.Count, DictionaryData.Values.Count); i++)
        {
            keys.Add(DictionaryData.Keys[i]);
            values.Add(DictionaryData.Values[i]);
            Dictionary.Add(DictionaryData.Keys[i], DictionaryData.Values[i]);
        }

    }
    public void OnBeforeSerialize()
    {
        if (!modifyValues)
        {
            keys.Clear();
            values.Clear();
            for (int i = 0; i < Mathf.Min(DictionaryData.Keys.Count, DictionaryData.Values.Count); i++)
            {
                keys.Add(DictionaryData.Keys[i]);
                values.Add(DictionaryData.Values[i]);
            }
        }
    }

    public void OnAfterDeserialize()
    {

    }
    public void DesrializeDictionary()
    {
        Debug.Log("DesrializeDictionary");
        dictionary.Clear();
        DictionaryData.Keys.Clear();
        DictionaryData.Values.Clear();
        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            DictionaryData.Keys.Add(keys[i]);
            DictionaryData.Values.Add(values[i]);
            Dictionary.Add(keys[i], values[i]);
        }
        modifyValues = false;
    }

    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        foreach (var item in Dictionary)
        {
            Debug.Log("Key: " + item.Key + " Value: " + item.Value);
        }
    }
}
[System.Serializable]
public struct QuestData
{
    /// <summary>
    /// Quest001 ˜A”Ô‚Å
    /// </summary>
    public string ID;
    public string Name;
    public ClearConditions Clear;
    public FailureConditions Failure;
    public Scene Field;
    public List<STRINGINT> TargetName;
    public List<STRINGINT> OtherName;
}

[System.Serializable]
public struct QuestHolderData
{
    public List<string> Quests;
}
[System.Serializable] 
public struct STRINGINT
{
    public string name;
    public int number;
}