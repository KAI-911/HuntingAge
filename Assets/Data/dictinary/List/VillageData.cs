using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class VillageData : MonoBehaviour
{
    public VillageSaveData _saveData;
    public int VillageLevel { get => _saveData.VillageLevel; set => _saveData.VillageLevel = value; }
    public int BlacksmithLevel { get => _saveData.BlacksmithLevel; set => _saveData.BlacksmithLevel = value; }
    public int KitchenLevel { get => _saveData.KitchenLevel; set => _saveData.KitchenLevel = value; }
    private void Awake()
    {
        _saveData = new VillageSaveData();
    }
}
