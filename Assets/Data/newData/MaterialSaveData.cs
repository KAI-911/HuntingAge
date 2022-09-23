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
        //dictionary���ƕۑ��ł��Ȃ��̂Ń��X�g�^���ۑ�
        foreach (var item in dictionary)
        {
            _keys.Add(item.Key);
            _value.Add(item.Value);
        }
    }
    public void LoadAfter()
    {
        dictionary.Clear();
        //dictionary���ƕۑ��ł��Ȃ��̂Ń��X�g�^�����dictionary�𐧍�
        for (int i = 0; i < _keys.Count; i++)
        {
            dictionary.Add(_keys[i],_value[i]);
        }
    }
}
