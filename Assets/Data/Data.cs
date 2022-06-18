using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MaterialData
{
    public int ID;
    public string Name;
    public string IconName;
}

[Serializable]
public class ItemData
{
    public int ID;
    public string Name;
    /// <summary>
    /// Œø‰Ê‚ª‰i‘±‚·‚é‚©‚Ç‚¤‚©i€–S‚·‚é‚ÆÁ‚¦‚éj
    /// </summary>
    public bool Permanent;
    /// <summary>
    /// Œø‰ÊŠÔ
    /// </summary>
    public float Time;
    /// <summary>
    /// ‚Ç‚Ì‚æ‚¤‚ÈŒø‰Ê‚È‚Ì‚©
    /// </summary>
    public ItemType ItemType;
    public string IconName;
}

public enum ItemType
{
    HpRecovery,
    AttackUp,
    DefenseUp
}

[Serializable]
public class EnemyData
{
    /// <summary>
    /// ‚»‚ê‚¼‚ê‚Ì–¼‘O‚ğ‘‚­
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
     /// 1 ˜A”Ô‚Å
     /// </summary>
    public int QuestLevel;
    public List<string> QuestDataID;
}
[Serializable]
public class QuestData
{
    /// <summary>
    /// Quest000 ˜A”Ô‚Å
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
    /// ‘ÎÛ‚Ì“¢”°
    /// </summary>
    TargetSubjugation,
    /// <summary>
    /// ÌW
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
        //Šù‚É“o˜^‚³‚ê‚Ä‚¢‚½‚ç‚»‚Ì‚Ü‚Ü’Ç‰Á
        if (_enemyCountList.ContainsKey(_enemyID))
        {
            _enemyCountList[_enemyID]++;
        }
        //V‚µ‚­“o˜^‚µ‚Ä’Ç‰Á
        else
        {
            _enemyCountList.Add(_enemyID, 1);
        }
    }
}