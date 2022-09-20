using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageData : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] VillageListObject DictionaryData;
    [SerializeField] int villageLevel;
    [SerializeField] int blacksmithLevel;
    [SerializeField] int kitchenLevel;

    public bool modifyValues;

    public int VillageLevel { get => villageLevel; set => villageLevel = value; }
    public int BlacksmithLevel { get => blacksmithLevel; set => blacksmithLevel = value; }
    public int KitchenLevel { get => kitchenLevel; set => kitchenLevel = value; }

    private void Awake()
    {
        villageLevel = DictionaryData.VillageLevel;
        blacksmithLevel = DictionaryData.BlacksmithLevel;
        kitchenLevel = DictionaryData.KitchenLevel;
    }
    public void OnBeforeSerialize()
    {
        if (!modifyValues)
        {
            villageLevel = DictionaryData.VillageLevel;
            blacksmithLevel = DictionaryData.BlacksmithLevel;
            kitchenLevel = DictionaryData.KitchenLevel;
        }
    }

    public void OnAfterDeserialize()
    {

    }
    public void DesrializeDictionary()
    {
        Debug.Log("DesrializeDictionary");
        DictionaryData.VillageLevel = villageLevel;
        DictionaryData.BlacksmithLevel = blacksmithLevel;
        DictionaryData.KitchenLevel = kitchenLevel;
        modifyValues = false;
    }

    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        Debug.Log(villageLevel);
        Debug.Log(blacksmithLevel);
        Debug.Log(kitchenLevel);
    }
}
