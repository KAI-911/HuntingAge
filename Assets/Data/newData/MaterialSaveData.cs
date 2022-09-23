using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class MaterialSaveData
{
    public Dictionary<string, MaterialData> dictionary = new Dictionary<string, MaterialData>();
    [SerializeField] List<string> _keys;
    [SerializeField] List<MaterialData> _value;
    public void SaveBefore()
    {
        _keys.Clear();
        _value.Clear();
        //dictionaryだと保存できないのでリスト型を二つ保存
        foreach (var item in dictionary)
        {
            _keys.Add(item.Key);
            _value.Add(item.Value);
        }
    }
    public void LoadAfter()
    {
        dictionary.Clear();
        //dictionaryだと保存できないのでリスト型二つからdictionaryを制作
        for (int i = 0; i < _keys.Count; i++)
        {
            dictionary.Add(_keys[i],_value[i]);
        }
    }
}
