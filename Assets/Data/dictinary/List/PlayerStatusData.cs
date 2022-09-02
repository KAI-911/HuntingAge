using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusData : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] PlayerStatusObject DictionaryData;
    [SerializeField] int _maxHP;
    [SerializeField] int _maxSP;
    [SerializeField] int _attack;
    [SerializeField] int _defense;
    public bool modifyValues;

    public int MaxHP { get => _maxHP; }
    public int MaxSP { get => _maxSP; }
    public int Attack { get => _attack;}
    public int Defense { get => _defense;  }

    private void Awake()
    {
        _maxHP = DictionaryData.MaxHP;
        _maxSP = DictionaryData.MaxSP;
        _attack = DictionaryData.Attack;
        _defense = DictionaryData.Defense;
    }
    public void OnBeforeSerialize()
    {
        if (!modifyValues)
        {
            _maxHP = DictionaryData.MaxHP;
            _maxSP = DictionaryData.MaxSP;
            _attack = DictionaryData.Attack;
            _defense = DictionaryData.Defense;
        }
    }

    public void OnAfterDeserialize()
    {

    }
    public void DesrializeDictionary()
    {
        Debug.Log("DesrializeDictionary");
        DictionaryData.MaxHP = _maxHP;
        DictionaryData.MaxSP = _maxSP;
        DictionaryData.Attack = _attack;
        DictionaryData.Defense = _defense;

        modifyValues = false;
    }

    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        Debug.Log(_maxHP);
        Debug.Log(_maxSP);
        Debug.Log(_attack);
        Debug.Log(_defense);
    }
}
