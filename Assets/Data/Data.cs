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
    /// ���ʂ��i�����邩�ǂ����i���S����Ə�����j
    /// </summary>
    public bool Permanent;
    /// <summary>
    /// ���ʎ���
    /// </summary>
    public float Time;
    /// <summary>
    /// �ǂ̂悤�Ȍ��ʂȂ̂�
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
    /// Weapon100[��] Weapon200[��] Weapon300[�|]
    /// </summary>
    public string ID;
    public string Name;
    public int AttackPoint;
    public int WeaponType;
    //����ɕK�v�ȑf�ރf�[�^
    public Dictionary<string, int> CreateWeaponMaterial;
    //�����ɕK�v�ȑf�ރf�[�^
    public Dictionary<string, int> EnhancementWeaponMaterial;
    //�����ɕK�v�Ȓb��ꃌ�x��
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
    /// ���ꂼ��̖��O������
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
     /// 001�A�Ԃ�
     /// </summary>
    public int QuestLevel;
    public List<string> QuestDataID;
}
[Serializable]
public class QuestData
{
    /// <summary>
    /// Quest001 �A�Ԃ�
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
    /// �Ώۂ̓���
    /// </summary>
    TargetSubjugation,
    /// <summary>
    /// �̏W
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
        //���ɓo�^����Ă����炻�̂܂ܒǉ�
        if (_enemyCountList.ContainsKey(_enemyID))
        {
            _enemyCountList[_enemyID]++;
        }
        //�V�����o�^���Ēǉ�
        else
        {
            _enemyCountList.Add(_enemyID, 1);
        }
    }
}