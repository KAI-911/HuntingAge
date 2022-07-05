using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] ItemHolderObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<int> values = new List<int>();
    [SerializeField] Dictionary<string, int> dictionary = new Dictionary<string, int>();
    [SerializeField] int maxFrame;
    [SerializeField] ItemStack itemStack;
    public bool modifyValues;
    public Dictionary<string, int> Dictionary { get => dictionary; }
    public int MaxFrame { get => maxFrame; set => maxFrame = value; }

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

    private void Update()
    {
        //foreach (var item in dictionary)
        //{
        //    switch (itemStack)
        //    {
        //        case ItemStack.Box:
        //            var data1 = GameManager.Instance.ItemDataList.Dictionary[item.Key];
        //            if (data1.BoxHoldNumber <= 0) Erase(item.Key);
        //            break;
        //        case ItemStack.Poach:
        //            var data2 = GameManager.Instance.ItemDataList.Dictionary[item.Key];
        //            if (data2.PoachHoldNumber <= 0) Erase(item.Key);
        //            break;
        //        default:
        //            break;
        //    }
            
        //}
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

    public void Add(string id)
    {
        if (keys.Contains(id)) return;
        //’Ç‰Á‚Å‚«‚é”ÍˆÍ‚ðŽæ“¾
        List<int> vs = new List<int>();
        for (int i = 0; i < maxFrame; i++) vs.Add(i);
        //Šù‚É’Ç‰Á‚µ‚Ä‚¢‚é•”•ª‚ðœ‚­
        foreach (var item in dictionary)
        {
            if (vs.Contains(item.Value))
            {
                vs.Remove(item.Value);
            }
        }
        if (vs.Count == 0) return;
        //ˆê”ÔÅ‰‚Ì’l‚É’Ç‰Á
        keys.Add(id);
        values.Add(vs[0]);
        DesrializeDictionary();
    }
    public void Erase(string id)
    {
        if (!keys.Contains(id)) return;
        int index = keys.IndexOf(id);
        keys.RemoveAt(index);
        values.RemoveAt(index);
        DesrializeDictionary();
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
