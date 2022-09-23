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
    /// ボックスからポーチに指定数のアイテムを送る
    /// ボックスに指定数ない場合はボックスにあるだけ渡す
    /// ポーチに受け取る容量が足りない場合は受け取れるだけ渡す
    /// </summary>
    /// <param name="ID">移動させるアイテムのID</param>
    /// <param name="move">ボックスからいくつ移動させるか</param>
    /// <param name="uinum">ポーチに移動させるアイテムがない場合に使用するUIの場所</param>
    public void BoxToPoach(string ID, int move, int uinum)
    {
        if (!_itemSaveData.Dictionary.ContainsKey(ID)) return;
        var data = _itemSaveData.Dictionary[ID];

        int eraseNumber = move;
        //ボックスにアイテムが足りない
        if (move > data.baseData.BoxHoldNumber) eraseNumber = data.baseData.BoxHoldNumber;
        //ポーチに受け取る容量がない
        if (move > data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber) eraseNumber = data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber;

        //UIの位置を設定
        if (data.baseData.PoachHoldNumber <= 0) data.baseData.PoachUINumber = uinum;

        data.baseData.BoxHoldNumber -= eraseNumber;
        data.baseData.PoachHoldNumber += eraseNumber;
        _itemSaveData.Dictionary[ID] = data;
    }

    /// <summary>
    /// ポーチからボックスに指定数のアイテムを送る
    /// ポーチに指定数ない場合はポーチにあるだけ渡す
    /// ボックスに受け取る容量が足りない場合は受け取れるだけ渡す
    /// </summary>
    /// <param name="ID">移動させるアイテムのID</param>
    /// <param name="move">ボックスからいくつ移動させるか</param>
    /// <param name="uinum">ボックスに移動させるアイテムがない場合に使用するUIの場所</param>
    public void PoachToBox(string ID, int move, int uinum)
    {
        if (!_itemSaveData.Dictionary.ContainsKey(ID)) return;
        var data = _itemSaveData.Dictionary[ID];

        int eraseNumber = move;
        //ポーチにアイテムが足りない
        if (move > data.baseData.PoachHoldNumber) eraseNumber = data.baseData.PoachHoldNumber;
        //ボックスに受け取る容量がない
        if (move > data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber) eraseNumber = data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber;

        //UIの位置を設定
        if (data.baseData.BoxHoldNumber <= 0) data.baseData.BoxUINumber = uinum;

        data.baseData.PoachHoldNumber -= eraseNumber;
        data.baseData.BoxHoldNumber += eraseNumber;
        _itemSaveData.Dictionary[ID] = data;
    }

    /// <summary>
    /// ポーチに指定数のアイテムを送る
    /// </summary>
    /// <param name="ID">移動させるアイテムのID</param>
    /// <param name="move">ポーチからいくつ移動させるか</param>
    /// <param name="uinum">ポーチに移動させるアイテムがない場合に使用するUIの場所</param>
    /// <returns>
    /// 基本的に正の値を返す
    /// -1 エラー:キーが見つからなかった
    /// </returns>
    public int GetToPoach(string ID, int move, int uinum)
    {
        if (!_itemSaveData.Dictionary.ContainsKey(ID)) return-1;
        var data = _itemSaveData.Dictionary[ID];


        int addNumber = move;
        //ボックスに受け取る容量がない
        if (move > data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber) addNumber = data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber;

        //UIの位置を設定
        if (data.baseData.PoachHoldNumber <= 0) data.baseData.PoachUINumber = uinum;

        data.baseData.PoachHoldNumber += addNumber;
        _itemSaveData.Dictionary[ID] = data;
        return addNumber;
    }
    /// <summary>
    /// ボックスに指定数のアイテムを送る 
    /// 基本的に正の値を返す
    /// -1 エラー:キーが見つからなかった
    /// </summary>
    /// <param name="ID">移動させるアイテムのID</param>
    /// <param name="move">いくつ移動させるか</param>
    /// <param name="uinum">ボックスに移動させるアイテムがない場合に使用するUIの場所</param>
    /// <returns></returns>
    public int GetToBox(string ID, int move, int uinum)
    {
        if (!_itemSaveData.Dictionary.ContainsKey(ID)) return -1;
        var data = _itemSaveData.Dictionary[ID];

        int addNumber = move;
        //ポーチに受け取る容量がない
        if (move > data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber) addNumber = data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber;

        //UIの位置を設定
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
            { //未使用の枠があるか確認----------------------------------------------
                List<int> vs = new List<int>();
                var table = UISoundManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]._tableSize;
                //全ての枠を確認
                for (int i = 0; i < table.x * table.y; i++) vs.Add(i);
                Debug.Log(vs.Count);
                foreach (var item in vs)
                {
                    Debug.Log(item);
                }
                //使用している枠を削除していく
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
            { //未使用の枠があるか確認----------------------------------------------
                List<int> vs = new List<int>();
                var table = UISoundManager.Instance.UIPresetData.Dictionary["IB_ItemSelect"]._tableSize;
                //全ての枠を確認
                for (int i = 0; i < table.x * table.y; i++) vs.Add(i);
                //使用している枠を削除していく
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
    /// 使用中のフラグ
    /// </summary>
    public bool Use;
    /// <summary>
    /// 効果が永続するかどうか（死亡すると消える）
    /// </summary>
    public bool Permanent;
    /// <summary>
    /// 効果時間(秒)
    /// </summary>
    public float Time;
    /// <summary>
    /// 上昇する値
    /// </summary>
    public int UpValue;
    /// <summary>
    /// どのような効果なのか
    /// </summary>
    public ItemType ItemType;
    /// <summary>
    /// 厨房レベルいくらで作ることができるか
    /// </summary>
    public int CreatableLevel;
    /// <summary>
    /// 生産に必要な素材
    /// </summary>
    public List<ItemNeedMaterial> NeedMaterialLst;

    //効果の説明文
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