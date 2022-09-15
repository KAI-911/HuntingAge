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
    /// �{�b�N�X����|�[�`�Ɏw�萔�̃A�C�e���𑗂�
    /// �{�b�N�X�Ɏw�萔�Ȃ��ꍇ�̓{�b�N�X�ɂ��邾���n��
    /// �|�[�`�Ɏ󂯎��e�ʂ�����Ȃ��ꍇ�͎󂯎��邾���n��
    /// </summary>
    /// <param name="ID">�ړ�������A�C�e����ID</param>
    /// <param name="move">�{�b�N�X���炢���ړ������邩</param>
    /// <param name="uinum">�|�[�`�Ɉړ�������A�C�e�����Ȃ��ꍇ�Ɏg�p����UI�̏ꏊ</param>
    public void BoxToPoach(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int eraseNumber = move;
        //�{�b�N�X�ɃA�C�e��������Ȃ�
        if (move > data.baseData.BoxHoldNumber) eraseNumber = data.baseData.BoxHoldNumber;
        //�|�[�`�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber) eraseNumber = data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.baseData.PoachHoldNumber <= 0) data.baseData.PoachUINumber = uinum;

        data.baseData.BoxHoldNumber -= eraseNumber;
        data.baseData.PoachHoldNumber += eraseNumber;
        values[index] = data;
        DesrializeDictionary();
    }

    /// <summary>
    /// �|�[�`����{�b�N�X�Ɏw�萔�̃A�C�e���𑗂�
    /// �|�[�`�Ɏw�萔�Ȃ��ꍇ�̓|�[�`�ɂ��邾���n��
    /// �{�b�N�X�Ɏ󂯎��e�ʂ�����Ȃ��ꍇ�͎󂯎��邾���n��
    /// </summary>
    /// <param name="ID">�ړ�������A�C�e����ID</param>
    /// <param name="move">�{�b�N�X���炢���ړ������邩</param>
    /// <param name="uinum">�{�b�N�X�Ɉړ�������A�C�e�����Ȃ��ꍇ�Ɏg�p����UI�̏ꏊ</param>
    public void PoachToBox(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int eraseNumber = move;
        //�|�[�`�ɃA�C�e��������Ȃ�
        if (move > data.baseData.PoachHoldNumber) eraseNumber = data.baseData.PoachHoldNumber;
        //�{�b�N�X�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber) eraseNumber = data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.baseData.BoxHoldNumber <= 0) data.baseData.BoxUINumber = uinum;

        data.baseData.PoachHoldNumber -= eraseNumber;
        data.baseData.BoxHoldNumber += eraseNumber;
        values[index] = data;
        DesrializeDictionary();
    }

    /// <summary>
    /// �|�[�`�Ɏw�萔�̃A�C�e���𑗂�
    /// </summary>
    /// <param name="ID">�ړ�������A�C�e����ID</param>
    /// <param name="move">�|�[�`���炢���ړ������邩</param>
    /// <param name="uinum">�|�[�`�Ɉړ�������A�C�e�����Ȃ��ꍇ�Ɏg�p����UI�̏ꏊ</param>
    /// <returns>
    /// ��{�I�ɐ��̒l��Ԃ�
    /// -1 �G���[:�L�[��������Ȃ�����
    /// </returns>
    public int GetToPoach(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return -1;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int addNumber = move;
        //�{�b�N�X�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber) addNumber = data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.baseData.PoachHoldNumber <= 0) data.baseData.PoachUINumber = uinum;

        data.baseData.PoachHoldNumber += addNumber;
        values[index] = data;
        DesrializeDictionary();
        return addNumber;
    }
    /// <summary>
    /// �{�b�N�X�Ɏw�萔�̃A�C�e���𑗂� 
    /// ��{�I�ɐ��̒l��Ԃ�
    /// -1 �G���[:�L�[��������Ȃ�����
    /// </summary>
    /// <param name="ID">�ړ�������A�C�e����ID</param>
    /// <param name="move">�����ړ������邩</param>
    /// <param name="uinum">�{�b�N�X�Ɉړ�������A�C�e�����Ȃ��ꍇ�Ɏg�p����UI�̏ꏊ</param>
    /// <returns></returns>
    public int GetToBox(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return -1;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int addNumber = move;
        //�|�[�`�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber) addNumber = data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.baseData.BoxHoldNumber <= 0) data.baseData.BoxUINumber = uinum;

        data.baseData.BoxHoldNumber += addNumber;
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

    public void ItemsConsumption(string _ID, int _num, bool _toPouch)
    {
        int index = keys.FindIndex(n => n.StartsWith(_ID));
        //Debug.Log(index);
        var data = values[index];
        int listCount;

        listCount = data.NeedMaterialLst.Count;

        List<string> needID = new List<string>();
        List<int> needRequired = new List<int>();

        for (int i = 0; i < listCount; i++)
        {
            int count = i;
            needID.Add(data.NeedMaterialLst[count].materialID);
            needRequired.Add(data.NeedMaterialLst[count].requiredCount);
            needRequired[i] *= _num;
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
public struct ItemData
{
    public MaterialData baseData;
    /// <summary>
    /// �g�p���̃t���O
    /// </summary>
    public bool Use;
    /// <summary>
    /// ���ʂ��i�����邩�ǂ����i���S����Ə�����j
    /// </summary>
    public bool Permanent;
    /// <summary>
    /// ���ʎ���(�b)
    /// </summary>
    public float Time;
    /// <summary>
    /// �㏸����l
    /// </summary>
    public int UpValue;
    /// <summary>
    /// �ǂ̂悤�Ȍ��ʂȂ̂�
    /// </summary>
    public ItemType ItemType;
    /// <summary>
    /// �~�[���x��������ō�邱�Ƃ��ł��邩
    /// </summary>
    public int CreatableLevel;
    /// <summary>
    /// ���Y�ɕK�v�ȑf��
    /// </summary>
    public List<ItemNeedMaterial> NeedMaterialLst;

    //���ʂ̐�����
    public string FlavorText;
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