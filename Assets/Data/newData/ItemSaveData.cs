using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class ItemSaveData
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<ItemData> values = new List<ItemData>();
    [SerializeField] Dictionary<string, ItemData> dictionary = new Dictionary<string, ItemData>();

    public Dictionary<string, ItemData> Dictionary { get => dictionary; set => dictionary = value; }
    public void SaveBefore()
    {
        keys.Clear();
        values.Clear();
        //dictionary���ƕۑ��ł��Ȃ��̂Ń��X�g�^���ۑ�
        foreach (var item in dictionary)
        {
            keys.Add(item.Key);
            values.Add(item.Value);
        }
    }
    public void LoadAfter()
    {
        dictionary.Clear();
        //dictionary���ƕۑ��ł��Ȃ��̂Ń��X�g�^�����dictionary�𐧍�
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary.Add(keys[i], values[i]);
        }
    }

}
