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
    public int ID;
    public string InstanceName;
    public List<EnemyPos> EnemyPos;
}

[Serializable]
public class EnemyPos
{
    public Scene _scene;
    public List<Vector3> _position;
}

[Serializable]
public class QuestData
{
    public int QuestID;
    public ClearConditions Clear;
    public FailureConditions Failure;
    public Scene Field;
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




