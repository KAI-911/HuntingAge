using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataList : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] ItemListObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<ItemData> values = new List<ItemData>();
    [SerializeField] Dictionary<string, ItemData> dictionary = new Dictionary<string, ItemData>();
    public bool modifyValues;
    public Dictionary<string, ItemData> Dictionary { get => dictionary; }
    public List<string> Keys { get => keys; set => keys = value; }
    public List<ItemData> Values { get => values; set => values = value; }

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
        foreach (var item in Dictionary)
        {
            Debug.Log("Key: " + item.Key + " Value: " + item.Value);
        }
    }

    /// <summary>
    /// ?{?b?N?X?????|?[?`???w???????A?C?e????????
    /// ?{?b?N?X???w???????????????{?b?N?X???????????n??
    /// ?|?[?`???????????e?????????????????????????????????n??
    /// </summary>
    /// <param name="ID">???????????A?C?e????ID</param>
    /// <param name="move">?{?b?N?X??????????????????????</param>
    /// <param name="uinum">?|?[?`?????????????A?C?e???????????????g?p????UI??????</param>
    public void BoxToPoach(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int eraseNumber = move;
        //?{?b?N?X???A?C?e????????????
        if (move > data.baseData.BoxHoldNumber) eraseNumber = data.baseData.BoxHoldNumber;
        //?|?[?`???????????e????????
        if (move > data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber) eraseNumber = data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber;

        //UI?????u??????
        if (data.baseData.PoachHoldNumber <= 0) data.baseData.PoachUINumber = uinum;

        data.baseData.BoxHoldNumber -= eraseNumber;
        data.baseData.PoachHoldNumber += eraseNumber;
        values[index] = data;
        DesrializeDictionary();
    }

    /// <summary>
    /// ?|?[?`?????{?b?N?X???w???????A?C?e????????
    /// ?|?[?`???w???????????????|?[?`???????????n??
    /// ?{?b?N?X???????????e?????????????????????????????????n??
    /// </summary>
    /// <param name="ID">???????????A?C?e????ID</param>
    /// <param name="move">?{?b?N?X??????????????????????</param>
    /// <param name="uinum">?{?b?N?X?????????????A?C?e???????????????g?p????UI??????</param>
    public void PoachToBox(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int eraseNumber = move;
        //?|?[?`???A?C?e????????????
        if (move > data.baseData.PoachHoldNumber) eraseNumber = data.baseData.PoachHoldNumber;
        //?{?b?N?X???????????e????????
        if (move > data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber) eraseNumber = data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber;

        //UI?????u??????
        if (data.baseData.BoxHoldNumber <= 0) data.baseData.BoxUINumber = uinum;

        data.baseData.PoachHoldNumber -= eraseNumber;
        data.baseData.BoxHoldNumber += eraseNumber;
        values[index] = data;
        DesrializeDictionary();
    }

    /// <summary>
    /// ?|?[?`???w???????A?C?e????????
    /// </summary>
    /// <param name="ID">???????????A?C?e????ID</param>
    /// <param name="move">?|?[?`??????????????????????</param>
    /// <param name="uinum">?|?[?`?????????????A?C?e???????????????g?p????UI??????</param>
    /// <returns>
    /// ???{?I???????l??????
    /// -1 ?G???[:?L?[??????????????????
    /// </returns>
    public int GetToPoach(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return -1;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int addNumber = move;
        //?{?b?N?X???????????e????????
        if (move > data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber) addNumber = data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber;

        //UI?????u??????
        if (data.baseData.PoachHoldNumber <= 0) data.baseData.PoachUINumber = uinum;

        data.baseData.PoachHoldNumber += addNumber;
        values[index] = data;
        DesrializeDictionary();
        return addNumber;
    }

    public bool ChackItem(string _ID, int _num)
    {
        var _material = GameManager.Instance.ItemDataList;
        if (!(_material.Dictionary.ContainsKey(_ID))) return false;
        int num = _material.Dictionary[_ID].baseData.BoxHoldNumber + _material.Dictionary[_ID].baseData.PoachHoldNumber;
        if (num < _num) return false;

        return true;
    }

    public int CreateItem(string _ID, int _num, bool _toPouch)
    {
        int index = keys.FindIndex(n => n.StartsWith(_ID));
        //Debug.Log(index);
        var data = values[index];

        int listCount = data.NeedMaterialLst.Count;
        List<string> needID = new List<string>();
        List<int> needRequiredCount = new List<int>();
        List<int> needRequired = new List<int>();

        for (int i = 0; i < listCount; i++)
        {
            needID.Add(data.NeedMaterialLst[i].materialID);
            needRequiredCount.Add(data.NeedMaterialLst[i].requiredCount);
            needRequired.Add(data.NeedMaterialLst[i].requiredCount);
            needRequired[i] *= _num;

            var _material = GameManager.Instance.ItemDataList;
            int num = _material.Dictionary[needID[i]].baseData.BoxHoldNumber + _material.Dictionary[needID[i]].baseData.PoachHoldNumber;
        }


        for (int i = 0; i < listCount; i++)
        {
            int count = i;
            var _material = GameManager.Instance.ItemDataList;
            int tmp = _material.Keys.IndexOf(needID[count]);
            if (_material.Values[tmp].baseData.PoachHoldNumber < needRequired[count])
            {
                needRequired[count] -= _material.Values[tmp].baseData.PoachHoldNumber;
                var data1 = _material.Values[tmp];
                data1.baseData.PoachHoldNumber = 0;
                _material.Values[tmp] = data1;

                data1.baseData.BoxHoldNumber -= needRequired[count];
                _material.Values[tmp] = data1;
            }
            else
            {
                var data1 = _material.Values[tmp];
                data1.baseData.PoachHoldNumber -= needRequired[count];
                _material.Values[tmp] = data1;
            }
        }
        GameManager.Instance.ItemDataList.DesrializeDictionary();

        if (_toPouch) { var data1 = values[index]; data1.baseData.PoachHoldNumber = _num; values[index] = data1; }
        else { var data1 = values[index]; data1.baseData.BoxHoldNumber = _num; values[index] = data1; }
        DesrializeDictionary();
        return 1;
    }
}

[System.Serializable]
public struct ItemData
{
    public MaterialData baseData;
    /// <summary>
    /// ?g?p?????t???O
    /// </summary>
    public bool Use;
    /// <summary>
    /// ???????i???????????????i???S?????????????j
    /// </summary>
    public bool Permanent;
    /// <summary>
    /// ????????(?b)
    /// </summary>
    public float Time;
    /// <summary>
    /// ?????????l
    /// </summary>
    public int UpValue;
    /// <summary>
    /// ????????????????????
    /// </summary>
    public ItemType ItemType;
    /// <summary>
    /// ?~?[???x????????????????????????????
    /// </summary>
    public int CreatableLevel;
    /// <summary>
    /// ???Y???K?v???f??
    /// </summary>
    public List<ItemNeedMaterial> NeedMaterialLst;
}

public enum ItemType
{
    HpRecovery,
    AttackUp,
    DefenseUp
}

[System.Serializable]
public struct ItemNeedMaterial
{
    public string materialID;
    public int requiredCount;
}