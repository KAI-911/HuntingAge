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
    HpRecovery,
    AttackUp,
    DefenseUp
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
public class QuestData
{
    /// <summary>
    /// Quest000 連番で
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




