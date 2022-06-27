using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDataList : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] MaterialDataObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<MaterialData> values = new List<MaterialData>();
    [SerializeField] Dictionary<string, MaterialData> dictionary = new Dictionary<string, MaterialData>();
    public bool modifyValues;

    /// <summary>
    /// 所持できる種類の最大量(枠数)
    /// </summary>
    public int MaxHolding;

    /// <summary>
    /// ボックスかポーチのどっちで保存するか
    /// </summary>
    public ItemStack ItemStack;

    public Dictionary<string, MaterialData> Dictionary { get => dictionary;}



    private void Awake()
    {
        for (int i = 0; i < Mathf.Min(DictionaryData.Keys.Count, DictionaryData.Values.Count); i++)
        {
            Dictionary.Add(DictionaryData.Keys[i], DictionaryData.Values[i]);
        }

    }
    public void OnBeforeSerialize()
    {
        if (!modifyValues)
        {
            keys.Clear();
            values.Clear();
            for (int i = 0; i < Mathf.Min(DictionaryData.Keys.Count,DictionaryData.Values.Count); i++)
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
        dictionary = new Dictionary<string, MaterialData>();
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
