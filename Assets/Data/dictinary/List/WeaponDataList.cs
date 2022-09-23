using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class WeaponDataList : MonoBehaviour
{
    public WeponSaveData _weponSaveData;

    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        foreach (var weapon in _weponSaveData.Dictionary)
        {
            Debug.Log("Key: " + weapon.Key + " Value: " + weapon.Value);
        }
    }
    [ContextMenu("Production")]
    public bool Production(string _ID, bool _confirmation)
    {
        var data = _weponSaveData.Dictionary[_ID];

        if (data.BoxPossession) return false;
        ItemsConsumption(_ID, true);

        data.BoxPossession = true;
        _weponSaveData.Dictionary[_ID] = data;
        return true;
    }

    public bool Enhancement(string _ID, bool _confirmation)
    {
        var data = _weponSaveData.Dictionary[_ID];
        var enhdata = _weponSaveData.Dictionary[data.EnhancementID];

        if (enhdata.BoxPossession) return false;

        ItemsConsumption(_ID, false);

        enhdata.BoxPossession = true;
        _weponSaveData.Dictionary[data.EnhancementID] = enhdata;
        data.BoxPossession = false;
        _weponSaveData.Dictionary[_ID] = data;

        GameManager.Instance.Player.ChangeWepon(enhdata.ID);


        return true;
    }

    //�A�C�e���f�[�^��������̂͂�������
    public void ItemsConsumption(string _ID, bool _production)
    {
        var data = _weponSaveData.Dictionary[_ID];
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
            var tmp = _material._materialSaveData.dictionary[needID[count]];//.Keys.IndexOf(needID[count]);
            if (tmp.PoachHoldNumber < needRequired[count])
            {
                needRequired[count] -= tmp.PoachHoldNumber;
                var data1 = tmp;
                data1.PoachHoldNumber = 0;
                data1.BoxHoldNumber -= needRequired[count];
                tmp = data1;
            }
            else
            {
                var data1 = tmp;
                data1.PoachHoldNumber -= needRequired[count];
                tmp = data1;
            }
            _material._materialSaveData.dictionary[needID[count]] = tmp;
        }
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
