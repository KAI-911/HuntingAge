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