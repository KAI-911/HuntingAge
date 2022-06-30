using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponDataList : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] WeaponListObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<WeaponData> values = new List<WeaponData>();
    [SerializeField] Dictionary<string, WeaponData> dictionary = new Dictionary<string, WeaponData>();
    public bool modifyValues;
    public Dictionary<string, WeaponData> Dictionary { get => dictionary; }
    // Start is called before the first frame update
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
        foreach (var weapon in Dictionary)
        {
            Debug.Log("Key: " + weapon.Key + " Value: " + weapon.Value);
        }
    }

    [ContextMenu("add")]
    public void add(string _ID)
    {

        int index = keys.FindIndex(n => n.StartsWith(_ID));
        Debug.Log(index);
        var data = values[index];
        data.BoxPossession = true;
        values[index] = data;
        DesrializeDictionary();
    }
}


[System.Serializable]
public struct WeaponData
{
    /// <summary>
    /// ����킪X00�A�f�ނ�00X
    /// </summary>
    public string ID;

    /// <summary>
    /// �\������閼�O
    /// </summary>
    public string Name;

    /// <summary>
    /// �A�C�R���̃p�X
    /// </summary>
    public string IconName;

    /// <summary>
    /// �A�C�e���{�b�N�X�łǂ̘g�ɕۑ�����Ă��邩
    /// </summary>
    public int BoxUINumber;

    /// <summary>
    /// �{�b�N�X�ɏ������Ă��邩
    /// </summary>
    public bool BoxPossession;

    /// <summary>
    /// �U����
    /// </summary>
    public float AttackPoint;

    /// <summary>
    /// �����
    /// </summary>
    public WeaponType WeaponType;
}

public enum WeaponType
{
    Axe = 0,
    Spear,
    Bow
}