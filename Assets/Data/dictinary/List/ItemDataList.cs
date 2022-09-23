using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataList : MonoBehaviour
{
    public ItemSaveData _itemSaveData;


    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        foreach (var item in _itemSaveData. Dictionary)
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
        if (!_itemSaveData.Dictionary.ContainsKey(ID)) return;
        var data = _itemSaveData.Dictionary[ID];

        int eraseNumber = move;
        //�{�b�N�X�ɃA�C�e��������Ȃ�
        if (move > data.baseData.BoxHoldNumber) eraseNumber = data.baseData.BoxHoldNumber;
        //�|�[�`�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber) eraseNumber = data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.baseData.PoachHoldNumber <= 0) data.baseData.PoachUINumber = uinum;

        data.baseData.BoxHoldNumber -= eraseNumber;
        data.baseData.PoachHoldNumber += eraseNumber;
        _itemSaveData.Dictionary[ID] = data;
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
        if (!_itemSaveData.Dictionary.ContainsKey(ID)) return;
        var data = _itemSaveData.Dictionary[ID];

        int eraseNumber = move;
        //�|�[�`�ɃA�C�e��������Ȃ�
        if (move > data.baseData.PoachHoldNumber) eraseNumber = data.baseData.PoachHoldNumber;
        //�{�b�N�X�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber) eraseNumber = data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.baseData.BoxHoldNumber <= 0) data.baseData.BoxUINumber = uinum;

        data.baseData.PoachHoldNumber -= eraseNumber;
        data.baseData.BoxHoldNumber += eraseNumber;
        _itemSaveData.Dictionary[ID] = data;
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
        if (!_itemSaveData.Dictionary.ContainsKey(ID)) return-1;
        var data = _itemSaveData.Dictionary[ID];


        int addNumber = move;
        //�{�b�N�X�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber) addNumber = data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.baseData.PoachHoldNumber <= 0) data.baseData.PoachUINumber = uinum;

        data.baseData.PoachHoldNumber += addNumber;
        _itemSaveData.Dictionary[ID] = data;
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
        if (!_itemSaveData.Dictionary.ContainsKey(ID)) return -1;
        var data = _itemSaveData.Dictionary[ID];

        int addNumber = move;
        //�|�[�`�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber) addNumber = data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.baseData.BoxHoldNumber <= 0) data.baseData.BoxUINumber = uinum;

        data.baseData.BoxHoldNumber += addNumber;
        _itemSaveData.Dictionary[ID] = data;
        return addNumber;
    }


    public bool ChackItem(string _ID, int _num)
    {
        var _material = _itemSaveData.Dictionary;
        if (!(_material.ContainsKey(_ID))) return false;
        int num = _material[_ID].baseData.BoxHoldNumber + _material[_ID].baseData.PoachHoldNumber;
        if (num < _num) return false;

        return true;
    }

    public int CreateItem(string _ID, int _num, bool _toPouch)
    {
        var data = _itemSaveData.Dictionary[_ID];

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

            //var _material = GameManager.Instance.ItemDataList._itemSaveData;
            //int num = _material.Dictionary[needID[i]].baseData.BoxHoldNumber + _material.Dictionary[needID[i]].baseData.PoachHoldNumber;
        }


        for (int i = 0; i < listCount; i++)
        {
            int count = i;
            var _material = GameManager.Instance.ItemDataList._itemSaveData.Dictionary;
            var tmp = _material[needID[count]];
            if (tmp.baseData.PoachHoldNumber < needRequired[count])
            {
                needRequired[count] -= tmp.baseData.PoachHoldNumber;
                var data1 = tmp;
                data1.baseData.PoachHoldNumber = 0;
                data1.baseData.BoxHoldNumber -= needRequired[count];
                tmp = data1;
            }
            else
            {
                var data1 = tmp;
                data1.baseData.PoachHoldNumber -= needRequired[count];
                tmp = data1;
            }
            _material[needID[count]] = tmp;
        }

        if (_toPouch) 
        {
            var data1 = data; 
            data1.baseData.PoachHoldNumber = _num;
            _itemSaveData.Dictionary[_ID] = data1; 
        }
        else 
        {
            var data1 = data;
            data1.baseData.BoxHoldNumber = _num;
            _itemSaveData.Dictionary[_ID] = data1;
        }
        return 1;
    }

    public void ItemsConsumption(string _ID, int _num, bool _toPouch)
    {
        var data = _itemSaveData.Dictionary[_ID];
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
            var tmpdata = _material._materialSaveData.dictionary[needID[count]];
            if (tmpdata.PoachHoldNumber < needRequired[count])
            {
                needRequired[count] -= tmpdata.PoachHoldNumber;
                var data1 = tmpdata;
                data1.PoachHoldNumber = 0;
                data1.BoxHoldNumber -= needRequired[count];
                tmpdata = data1;
            }
            else
            {
                var data1 = tmpdata;
                data1.PoachHoldNumber -= needRequired[count];
                tmpdata = data1;
            }
            _material._materialSaveData.dictionary[needID[count]] = tmpdata;
        }

        int _UINum = 0;

        if (_toPouch)
        {
            if (data.baseData.PoachHoldNumber == 0)
            { //���g�p�̘g�����邩�m�F----------------------------------------------
                List<int> vs = new List<int>();
                var table = UISoundManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]._tableSize;
                //�S�Ă̘g���m�F
                for (int i = 0; i < table.x * table.y; i++) vs.Add(i);
                Debug.Log(vs.Count);
                foreach (var item in vs)
                {
                    Debug.Log(item);
                }
                //�g�p���Ă���g���폜���Ă���
                foreach (var item in GameManager.Instance.ItemDataList._itemSaveData.Dictionary.Values)
                {
                    if (item.baseData.PoachHoldNumber <= 0) continue;
                    if (vs.Contains(item.baseData.PoachUINumber)) vs.Remove(item.baseData.PoachUINumber);
                }
                foreach (var item in GameManager.Instance.MaterialDataList._materialSaveData.dictionary.Values)
                {
                    if (item.PoachHoldNumber <= 0) continue;
                    if (vs.Contains(item.PoachUINumber)) vs.Remove(item.PoachUINumber);
                }
                Debug.Log(vs.Count);
                foreach (var item in vs)
                {
                    Debug.Log(item);
                }
                if (vs.Count > 0) _UINum = vs[0];
            }
        }
        else
        {
            if (data.baseData.BoxHoldNumber == 0)
            { //���g�p�̘g�����邩�m�F----------------------------------------------
                List<int> vs = new List<int>();
                var table = UISoundManager.Instance.UIPresetData.Dictionary["IB_ItemSelect"]._tableSize;
                //�S�Ă̘g���m�F
                for (int i = 0; i < table.x * table.y; i++) vs.Add(i);
                //�g�p���Ă���g���폜���Ă���
                foreach (var item in GameManager.Instance.ItemDataList._itemSaveData.Dictionary.Values)
                {
                    if (item.baseData.BoxHoldNumber <= 0) continue;
                    if (vs.Contains(item.baseData.BoxUINumber)) vs.Remove(item.baseData.BoxUINumber);
                }
                foreach (var item in GameManager.Instance.MaterialDataList._materialSaveData.dictionary.Values)
                {
                    if (item.BoxHoldNumber <= 0) continue;
                    if (vs.Contains(item.BoxUINumber)) vs.Remove(item.BoxUINumber);
                }
                if (vs.Count > 0) _UINum = vs[0];
            }
        }



        if (_toPouch) GameManager.Instance.ItemDataList.GetToPoach(_ID, _num, _UINum);
        else GameManager.Instance.ItemDataList.GetToBox(_ID, _num, _UINum);

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