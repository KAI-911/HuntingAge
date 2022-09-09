using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPresetDataList : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] UIPresetObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<ItemIconData> values = new List<ItemIconData>();
    [SerializeField] Dictionary<string, ItemIconData> dictionary = new Dictionary<string, ItemIconData>();
    public bool modifyValues;
    public Dictionary<string, ItemIconData> Dictionary { get => dictionary; }


    private void Awake()
    {
        keys.Clear();
        values.Clear();
        Dictionary.Clear();
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
public struct ItemIconData
{
    public Vector2 _leftTopPos;
    public float _padding;
    public Vector2 _tableSize;
    public bool _canBeChanged;
    public bool _buttonBack;
    public bool _textFlg;
    public GameObject _buttonPrefab;
    public GameObject _buttonBackPrefab;
    public GameObject _textPrefab;
    public TEXT _textData;
}
