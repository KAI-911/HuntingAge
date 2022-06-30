using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;


//[Serializable]
//public class WeaponData
//{
//    /// <summary>
//    /// Weapon100[��] Weapon200[��] Weapon300[�|]
//    /// </summary>
//    public string ID;
//    public string Name;
//    public int AttackPoint;
//    public int WeaponType;
//    //����ɕK�v�ȑf�ރf�[�^
//    public Dictionary<string, int> CreateWeaponMaterial;
//    //�����ɕK�v�ȑf�ރf�[�^
//    public Dictionary<string, int> EnhancementWeaponMaterial;
//    //�����ɕK�v�Ȓb��ꃌ�x��
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
//     /// 001�A�Ԃ�
//     /// </summary>
//    public int QuestLevel;
//    public List<string> QuestDataID;
//}



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

//[Serializable]
//public class ItemHolder
//{
//    /// <summary>
//    /// string �������Ă�����
//    /// int    ��
//    /// </summary>
//    public Dictionary<string, int> ItemList =new Dictionary<string, int>();
//    /// <summary>
//    /// �����ł����ނ̍ő��(�g��)
//    /// </summary>
//    public int MaxHolding;

//    /// <summary>
//    /// �{�b�N�X���|�[�`�̂ǂ����ŕۑ����邩
//    /// </summary>
//    public ItemStack ItemStack;

//    /// <summary>
//    /// -1 = �g���S�Ė��܂��Ă���
//    /// -2 = �Z�[�u�f�[�^���Ȃ�
//    /// -3 = �X�^�b�N���
//    /// </summary>
//    /// <param name="AddID"></param>
//    /// <param name="AddNumber"></param>
//    /// <returns></returns>
//    int ItemAdd(string AddID, int AddNumber)
//    {
//        //�g���S�Ė��܂��Ă���
//        if (MaxHolding <= ItemList.Count) return -1;
//        //AddID���Z�[�u�f�[�^���猩����Ȃ�
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
//            //�X�^�b�N����̏ꍇ
//            if (stack <= ItemList[AddID]) return -3;

//            if (stack <= ItemList[AddID] + AddNumber)
//            {
//                AddNumber = (ItemList[AddID] + AddNumber) - stack;
//            }

//            //���Ɉ�ȏ㎝���Ă��� && �X�^�b�N�������
//            ItemList[AddID] += AddNumber;
//        }
//        else
//        {
//            //�܂���������Ă��Ȃ� && �g���󂢂Ă���
//            ItemList.Add(AddID, 1);
//        }
//        return AddNumber;
//    }

//    /// <summary>
//    /// -1 = EraseID��������Ȃ�
//    /// -2 = ItemList��EraseID���Ȃ�
//    /// </summary>
//    /// <param name="EraseID"></param>
//    /// <param name="EraseNumber"></param>
//    /// <returns></returns>
//    int ItemErase(string EraseID, int EraseNumber)
//    {
//        if (!SaveData.ContainsKey(EraseID)) return -1;//�G���[:�f�[�^���Ȃ�
//        var data = SaveData.GetClass(EraseID, new MaterialData());
//        if (ItemList.ContainsKey(EraseID))
//        {
//            int num = ItemList[EraseID];
//            if (num >= EraseNumber)
//            {
//                //���l�����炷
//                ItemList[EraseID] -= EraseNumber;
//            }
//            else
//            {
//                //���炷�����ێ����Ă��鐔�����Ȃ������ꍇ
//                //���炷����ێ����Ă��鐔�ɂ��A�ێ�����0�ɂ���
//                EraseNumber = num;
//                ItemList[EraseID] = 0;
//            }
//        }
//        else
//        {
//            return -2;//�G���[:���X�g�ɂȂ�
//        }
//        //�ێ��������炵������0�ɂȂ��Ă����烊�X�g����폜
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

