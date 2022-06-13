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