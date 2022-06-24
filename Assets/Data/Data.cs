using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MaterialData
{
    public string ID;
    public string Name;
    public string IconName;
}

[Serializable]
public class ItemData
{
    public string ID;
    public string Name;
    /// <summary>
    /// 効果が永続するかどうか（死亡すると消える）
    /// </summary>
    public bool Permanent;
    /// <summary>
    /// 効果時間
    /// </summary>
    public float Time;
    /// <summary>
    /// どのような効果なのか
    /// </summary>
    public ItemType ItemType;
    public string IconName;
}

public enum ItemType
{
    HpRecovery=1,
    AttackUp,
    DefenseUp
}

[Serializable]
public class WeaponData
{
    /// <summary>
    /// Weapon100[斧] Weapon200[槍] Weapon300[弓]
    /// </summary>
    public string ID;
    public string Name;
    public int AttackPoint;
    public int WeaponType;
    //制作に必要な素材データ
    public Dictionary<string, int> CreateWeaponMaterial;
    //強化に必要な素材データ
    public Dictionary<string, int> EnhancementWeaponMaterial;
    //強化に必要な鍛冶場レベル
    public int RequiredBlackmithLevel;
}

public enum WeaponType
{
    Axe=1,
    Spear,
    Bow
}
[Serializable]
public class EnemyData
{
    /// <summary>
    /// それぞれの名前を書く
    /// </summary>
    public string ID;
    public string DisplayName;
    public string InstanceName;
    public int DeathCount;
    public List<Position> EnemyPos;
    public Position EnemyPosition(Scene scene)
    {
        foreach (var item in EnemyPos)
        {
            if (item.scene != scene) continue;
            return item;
        }
        return null;
    }
}

[Serializable]
public class Position
{
    public Scene scene;
    public List<Vector3> pos;
}
[Serializable]
public class QuestHolder
{    /// <summary>
     /// 001連番で
     /// </summary>
    public int QuestLevel;
    public List<string> QuestDataID;
}
[Serializable]
public class QuestData
{
    /// <summary>
    /// Quest001 連番で
    /// </summary>
    public string ID;
    public string Name;
    public ClearConditions Clear;
    public FailureConditions Failure;
    public Scene Field;
    public List<string> TargetName;
    public List<string> OtherName;
}



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
    Dictionary<string, int> _enemyCountList=new Dictionary<string, int>();
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