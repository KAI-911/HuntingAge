using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class WeponSaveData
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<WeaponData> values = new List<WeaponData>();
    [SerializeField] Dictionary<string, WeaponData> dictionary = new Dictionary<string, WeaponData>();

    public Dictionary<string, WeaponData> Dictionary { get => dictionary; set => dictionary = value; }
    public void SaveBefore()
    {
        keys.Clear();
        values.Clear();
        //dictionaryだと保存できないのでリスト型を二つ保存
        foreach (var item in dictionary)
        {
            keys.Add(item.Key);
            values.Add(item.Value);
        }
    }
    public void LoadAfter()
    {
        dictionary.Clear();
        //dictionaryだと保存できないのでリスト型二つからdictionaryを制作
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary.Add(keys[i], values[i]);
        }
    }
}
