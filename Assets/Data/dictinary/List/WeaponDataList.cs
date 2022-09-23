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

    //アイテムデータをいじるのはここから
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
    /// 武器プレハブのパス
    /// </summary>
    public string weaponPath;

    /// <summary>
    /// アイコンのパス
    /// </summary>
    public string IconPass;

    ///// <summary>
    ///// アイコンの名前
    ///// </summary>
    //public string IconName;

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
    public int AttackPoint;

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
