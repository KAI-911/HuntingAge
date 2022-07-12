using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] ItemHolderObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<CollectionDataLsit> values = new List<CollectionDataLsit>();
    [SerializeField] Dictionary<string, CollectionDataLsit> dictionary = new Dictionary<string, CollectionDataLsit>();
    public bool modifyValues;
    public Dictionary<string, CollectionDataLsit> Dictionary { get => dictionary; }

    private void Awake()
    {
        for (int i = 0; i < Mathf.Min(DictionaryData.Keys.Count, DictionaryData.Values.Count); i++)
        {
            Dictionary.Add(DictionaryData.Keys[i], DictionaryData.Values[i]);
        }
    }
    private void Start()
    {
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
        foreach (var item in Dictionary)
        {
            Debug.Log("Key: " + item.Key + " Value: " + item.Value);
        }
    }
}
