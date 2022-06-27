using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDataList : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] EnemyListObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<EnemyData> values = new List<EnemyData>();
    [SerializeField] Dictionary<string, EnemyData> dictionary = new Dictionary<string, EnemyData>();
    public bool modifyValues;
    public Dictionary<string, EnemyData> Dictionary { get => dictionary; }


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
public class EnemyData
{
    /// <summary>
    /// ÇªÇÍÇºÇÍÇÃñºëOÇèëÇ≠
    /// </summary>
    public string ID;
    public string DisplayName;
    public string InstanceName;
    public int DeathCount;
    public List<Position> EnemyPos;
    public Position EnemyPosition(Scene scene)
    {
        foreach (var item in EnemyPos)
        {
            if (item.scene != scene) continue;
            return item;
        }
        return null;
    }
}
