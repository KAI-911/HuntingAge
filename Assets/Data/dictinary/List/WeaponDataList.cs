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
    public List<string> Keys { get => keys; set => keys = value; }
    public List<WeaponData> Values { get => values; set => values = value; }
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
    public int Production(string _ID, bool _confirmation)
    {
        int index = keys.FindIndex(n => n.StartsWith(_ID));
        //Debug.Log(index);
        var data = values[index];

        int count = data.ProductionNeedMaterialLst.Count;
        string[] needID; needID = new string[count];
        int[] needRequiredCount; needRequiredCount = new int[count];

        for (int i = 0; i < count; i++)
        {
            needID[i] = data.ProductionNeedMaterialLst[i].materialID;
            needRequiredCount[i] = data.ProductionNeedMaterialLst[i].requiredCount;

            var _material = GameManager.Instance.MaterialDataList;
            if (!(_material.Dictionary.ContainsKey(needID[i]))) return 0;
            int _num = _material.Dictionary[needID[i]].BoxHoldNumber + _material.Dictionary[needID[i]].PoachHoldNumber;
            if (_num < needRequiredCount[i]) return 0;
        }

        if (!_confirmation) return 1;
        if (data.BoxPossession) return 1;
        ItemsConsumption(_ID, true);

        data.BoxPossession = true;
        values[index] = data;
        DesrializeDictionary();
        return 2;
    }

    public int Enhancement(string _ID, bool _confirmation)
    {
        Debug.Log("DataListmadekiteTukuretayo");
        int index = keys.FindIndex(n => n.StartsWith(_ID));
        var data = values[index];
        int enhIndex = keys.FindIndex(n => n.StartsWith(data.EnhancementID));
        var enhdata = values[enhIndex];
        if (enhdata.BoxPossession) return 1;

        Debug.Log(data.ID);
        Debug.Log(enhdata.ID);

        for (int i = 0; i < enhdata.ProductionNeedMaterialLst.Count; i++)
        {
            string needID = enhdata.EnhancementNeedMaterialLst[i].materialID;
            int needRequiredCount = enhdata.EnhancementNeedMaterialLst[i].requiredCount;

            var _material = GameManager.Instance.MaterialDataList;
            if (!(_material.Dictionary.ContainsKey(needID))) return 0;
            int _num = _material.Dictionary[needID].BoxHoldNumber + _material.Dictionary[needID].PoachHoldNumber;
            if (_num < needRequiredCount) return 0;
        }


        if (!_confirmation) return 1;
        Debug.Log("kakuninnmadesitayo");

        ItemsConsumption(_ID, false);

        Debug.Log("syouhimadesitayo");
        enhdata.BoxPossession = true;
        values[enhIndex] = enhdata;
        data.BoxPossession = false;
        values[index] = data;
        Debug.Log("dataList" + values[index].BoxPossession + "kakuninndayo");
        DesrializeDictionary();

        return 2;
    }

    //�A�C�e���f�[�^��������̂͂�������
    public void ItemsConsumption(string _ID, bool _production)
    {
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



        for (int i = 0; i < listCount; i++)
        {
            int count = i;
            var _material = GameManager.Instance.MaterialDataList;
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
        GameManager.Instance.MaterialDataList.DesrializeDictionary();
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
    /// �������ID�F�����悪�Ȃ��ꍇempty
    /// </summary>
    public string EnhancementID;

    /// <summary>
    /// �\������閼�O
    /// </summary>
    public string Name;

    /// <summary>
    /// ����v���n�u�̃p�X
    /// </summary>
    public string weaponPath;

    /// <summary>
    /// �A�C�R���̃p�X
    /// </summary>
    public string IconPass;

    ///// <summary>
    ///// �A�C�R���̖��O
    ///// </summary>
    //public string IconName;

    /// <summary>
    /// �A�C�e���{�b�N�X�łǂ̘g�ɕۑ�����Ă��邩
    /// </summary>
    public int BoxUINumber;

    /// <summary>
    /// �b��ꃌ�x��������ō�邱�Ƃ��ł��邩
    /// </summary>
    public int CreatableLevel;

    /// <summary>
    /// �{�b�N�X�ɏ������Ă��邩
    /// </summary>
    public bool BoxPossession;

    /// <summary>
    /// �U����
    /// </summary>
    public int AttackPoint;

    /// <summary>
    /// �����
    /// </summary>
    public WeaponType WeaponType;

    /// <summary>
    /// �����ɕK�v�ȑf��
    /// </summary>
    public List<NeedMaterial> ProductionNeedMaterialLst;

    /// <summary>
    /// �����ɕK�v�ȑf��
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
