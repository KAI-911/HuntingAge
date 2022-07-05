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

    [ContextMenu("Production")]
    public int Production(string _ID)
    {
        int index = keys.FindIndex(n => n.StartsWith(_ID));
        Debug.Log(index);
        var data = values[index];
        if (data.BoxPossession)return 0;
        data.BoxPossession = true;
        values[index] = data;
        DesrializeDictionary();
        return 1;
    }

    public int Enhancement(string _ID)
    {
        return 1;
    }
}


[System.Serializable]
public struct WeaponData
{
    /// <summary>
    /// 武器種がX00、素材が00X
    /// </summary>
    public string ID;

    /// <summary>
    /// 表示される名前
    /// </summary>
    public string Name;

    /// <summary>
    /// アイコンのパス
    /// </summary>
    public string IconName;

    /// <summary>
    /// アイテムボックスでどの枠に保存されているか
    /// </summary>
    public int BoxUINumber;

    /// <summary>
    /// 鍛冶場レベルいくらで作ることができるか
    /// </summary>
    public int CreatableLevel;

    /// <summary>
    /// ボックスに所持しているか
    /// </summary>
    public bool BoxPossession;

    /// <summary>
    /// 攻撃力
    /// </summary>
    public float AttackPoint;

    /// <summary>
    /// 武器種
    /// </summary>
    public WeaponType WeaponType;
}

public enum WeaponType
{
    Axe = 0,
    Spear,
    Bow
}
