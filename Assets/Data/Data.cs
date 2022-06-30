using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;


//[Serializable]
//public class WeaponData
//{
//    /// <summary>
//    /// Weapon100[斧] Weapon200[槍] Weapon300[弓]
//    /// </summary>
//    public string ID;
//    public string Name;
//    public int AttackPoint;
//    public int WeaponType;
//    //制作に必要な素材データ
//    public Dictionary<string, int> CreateWeaponMaterial;
//    //強化に必要な素材データ
//    public Dictionary<string, int> EnhancementWeaponMaterial;
//    //強化に必要な鍛冶場レベル
//    public int RequiredBlackmithLevel;
//}

//public enum WeaponType
//{
//    Axe = 1,
//    Spear,
//    Bow
//}


[Serializable]
public class Position
{
    public Scene scene;
    public List<Vector3> pos;
}
//[Serializable]
//public class QuestHolder
//{    /// <summary>
//     /// 001連番で
//     /// </summary>
//    public int QuestLevel;
//    public List<string> QuestDataID;
//}



[Serializable]
public enum ClearConditions
{
    /// <summary>
    /// 対象の討伐
    /// </summary>
    TargetSubjugation,
    /// <summary>
    /// 採集
    /// </summary>
    Gathering
}
[Serializable]
public enum FailureConditions
{
    OneDown,
    TwoDown,
    ThreeDown,
    FourDown,
    FiveDown
}

[Serializable]
public class VillageData
{
    public int VillageLevel;
    public int BlacksmithLevel;
    public int KitchenLevel;
}

public class EnemyCount
{
    Dictionary<string, int> _enemyCountList = new Dictionary<string, int>();
    public Dictionary<string, int> EnemyCountList { get => _enemyCountList; }
    public void EnemyDataSet()
    {
        foreach (var item in _enemyCountList)
        {
            EnemyData data = SaveData.GetClass(item.Key, new EnemyData());
            data.DeathCount += item.Value;
            SaveData.SetClass(item.Key, data);
        }
    }
    public void Add(string _enemyID)
    {
        //既に登録されていたらそのまま追加
        if (_enemyCountList.ContainsKey(_enemyID))
        {
            _enemyCountList[_enemyID]++;
        }
        //新しく登録して追加
        else
        {
            _enemyCountList.Add(_enemyID, 1);
        }
    }
}

//[Serializable]
//public class ItemHolder
//{
//    /// <summary>
//    /// string 所持している種類
//    /// int    個数
//    /// </summary>
//    public Dictionary<string, int> ItemList =new Dictionary<string, int>();
//    /// <summary>
//    /// 所持できる種類の最大量(枠数)
//    /// </summary>
//    public int MaxHolding;

//    /// <summary>
//    /// ボックスかポーチのどっちで保存するか
//    /// </summary>
//    public ItemStack ItemStack;

//    /// <summary>
//    /// -1 = 枠が全て埋まっている
//    /// -2 = セーブデータがない
//    /// -3 = スタック上限
//    /// </summary>
//    /// <param name="AddID"></param>
//    /// <param name="AddNumber"></param>
//    /// <returns></returns>
//    int ItemAdd(string AddID, int AddNumber)
//    {
//        //枠が全て埋まっている
//        if (MaxHolding <= ItemList.Count) return -1;
//        //AddIDがセーブデータから見つからない
//        if (!SaveData.ContainsKey(AddID)) return -2;
//        var data = SaveData.GetClass(AddID, new MaterialData());
//        if (ItemList.ContainsKey(AddID))
//        {
//            int stack=0;
//            switch (ItemStack)
//            {
//                case ItemStack.Box:
//                    stack = data.BoxStackNumber;
//                    break;
//                case ItemStack.Poach:
//                    stack = data.PoachStackNumber;
//                    break;
//                default:
//                    stack = data.PoachStackNumber;
//                    break;
//            }
//            //スタック上限の場合
//            if (stack <= ItemList[AddID]) return -3;

//            if (stack <= ItemList[AddID] + AddNumber)
//            {
//                AddNumber = (ItemList[AddID] + AddNumber) - stack;
//            }

//            //既に一つ以上持っている && スタック上限未満
//            ItemList[AddID] += AddNumber;
//        }
//        else
//        {
//            //まだ一つももっていない && 枠が空いている
//            ItemList.Add(AddID, 1);
//        }
//        return AddNumber;
//    }

//    /// <summary>
//    /// -1 = EraseIDが見つからない
//    /// -2 = ItemListにEraseIDがない
//    /// </summary>
//    /// <param name="EraseID"></param>
//    /// <param name="EraseNumber"></param>
//    /// <returns></returns>
//    int ItemErase(string EraseID, int EraseNumber)
//    {
//        if (!SaveData.ContainsKey(EraseID)) return -1;//エラー:データがない
//        var data = SaveData.GetClass(EraseID, new MaterialData());
//        if (ItemList.ContainsKey(EraseID))
//        {
//            int num = ItemList[EraseID];
//            if (num >= EraseNumber)
//            {
//                //数値を減らす
//                ItemList[EraseID] -= EraseNumber;
//            }
//            else
//            {
//                //減らす数より保持している数が少なかった場合
//                //減らす数を保持している数にし、保持数を0にする
//                EraseNumber = num;
//                ItemList[EraseID] = 0;
//            }
//        }
//        else
//        {
//            return -2;//エラー:リストにない
//        }
//        //保持数を減らした結果0になっていたらリストから削除
//        if (ItemList[EraseID] == 0)
//        {
//            ItemList.Remove(EraseID);
//        }
//        return EraseNumber;
//    }
//}

public enum ItemStack
{
    Box,
    Poach
}

