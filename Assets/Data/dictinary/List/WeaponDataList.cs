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
        //Debug.Log(index);
        var data = values[index];
        if (data.BoxPossession) return 0;

        int count = data.ProductionNeedMaterialLst.Count;
        string[] needID; needID = new string[count];
        int[] needRequiredCount; needRequiredCount = new int[count];

        for (int i = 0; i < count; i++)
        {
            needID[i] = data.ProductionNeedMaterialLst[i].materialID;
            needRequiredCount[i] = data.ProductionNeedMaterialLst[i].requiredCount;

            var _material = GameManager.Instance.ItemDataList;
            if (!(_material.Dictionary.ContainsKey(needID[i]))) return 2;
            int _num = _material.Dictionary[needID[i]].BoxHoldNumber + _material.Dictionary[needID[i]].PoachHoldNumber;
            if (_num < needRequiredCount[i]) return 2;
        }

        ItemsConsumption(_ID, true);

        data.BoxPossession = true;
        values[index] = data;
        DesrializeDictionary();
        return 1;
    }

    public int Enhancement(string _ID)
    {
        Debug.Log("DataListmadekiteTukuretayo");
        int index = keys.FindIndex(n => n.StartsWith(_ID));
        var data = values[index + 1];
        if (data.BoxPossession) return 0;

        for (int i = 0; i < data.ProductionNeedMaterialLst.Count; i++)
        {
            string needID = data.EnhancementNeedMaterialLst[i].materialID;
            int needRequiredCount = data.EnhancementNeedMaterialLst[i].requiredCount;

            var _material = GameManager.Instance.ItemDataList;
            if (!(_material.Dictionary.ContainsKey(needID))) return 2;
            int _num = _material.Dictionary[needID].BoxHoldNumber + _material.Dictionary[needID].PoachHoldNumber;
            if (_num < needRequiredCount) return 2;
        }


        Debug.Log("kakuninnmadesitayo");

        ItemsConsumption(_ID, false);

        Debug.Log("syouhimadesitayo");
        data.BoxPossession = true;
        values[index + 1] = data;
        var data1 = values[index];
        data1.BoxPossession = false;
        values[index] = data1;
        DesrializeDictionary();

        return 1;
    }

    public void ItemsConsumption(string _ID, bool _production)
    {


        Debug.Log("kannsuuyobaretayo");
        int index = keys.FindIndex(n => n.StartsWith(_ID));
        //Debug.Log(index);
        var data = values[index];
        int listCount;

        if (_production) listCount = data.ProductionNeedMaterialLst.Count;
        else listCount = data.EnhancementNeedMaterialLst.Count;

        List<string> needID = new List<string>();
        List<int> needRequired = new List<int>();

        if (_production)
        {
            for (int i = 0; i < listCount; i++)
            {
                needID.Add(data.ProductionNeedMaterialLst[i].materialID);
                needRequired.Add(data.ProductionNeedMaterialLst[i].requiredCount);
            }
        }
        else
        {
            for (int i = 0; i < listCount; i++)
            {
                int count = i;
                needID.Add(data.EnhancementNeedMaterialLst[count].materialID);
                needRequired.Add(data.EnhancementNeedMaterialLst[count].requiredCount);
            }
        }
        Debug.Log("syouhisetteimadesitayo");



        for (int i = 0; i < listCount; i++)
        {
            int count = i;
            var _material = GameManager.Instance.ItemDataList;
            int tmp = _material.Keys.IndexOf(needID[count]);
            if (_material.Values[tmp].PoachHoldNumber < needRequired[count])
            {
                needRequired[count] -= _material.Values[tmp].PoachHoldNumber;
                var data1 = _material.Values[tmp];
                data1.PoachHoldNumber = 0;
                _material.Values[tmp] = data1;

                data1.BoxHoldNumber -= needRequired[count];
                _material.Values[tmp] = data1;
            }
            else
            {
                var data1 = _material.Values[tmp];
                data1.PoachHoldNumber -= needRequired[count];
                _material.Values[tmp] = data1;
            }
        }
        GameManager.Instance.ItemDataList.DesrializeDictionary();
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
    /// 強化先のID：強化先がない場合empty
    /// </summary>
    public string EnhancementID;

    /// <summary>
    /// 表示される名前
    /// </summary>
    public string Name;

    /// <summary>
    /// アイコンのパス
    /// </summary>
    public string IconPass;

    /// <summary>
    /// アイコンの名前
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

    /// <summary>
    /// 製造に必要な素材
    /// </summary>
    public List<NeedMaterial> ProductionNeedMaterialLst;

    /// <summary>
    /// 強化に必要な素材
    /// </summary>
    public List<NeedMaterial> EnhancementNeedMaterialLst;
}

public enum WeaponType
{
    Axe = 0,
    Spear,
    Bow
}

[System.Serializable]
public struct NeedMaterial
{
    public string materialID;
    public int requiredCount;
}
