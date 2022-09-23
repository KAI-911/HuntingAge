using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
[Serializable]
public class InitData : MonoBehaviour
{
    [SerializeField] VillageSaveData _villageSaveData_Init;
    [SerializeField] SettingSaveData _settingSaveData_Init;
    [SerializeField] PlayerSaveData _playerSaveData_Init;
    [SerializeField] MaterialSaveData _materialSaveData_Init;
    [SerializeField] ItemSaveData _itemSaveData_Init;
    [SerializeField] QuestSaveData _questSaveData;
    [SerializeField] WeponSaveData _weponSaveData;
    public void InitSaveDataLoad()
    {
        //ファイルがない場合は制作して初期データを読み込む
        if (!File.Exists(GameManager.Instance.VillageDataPath))
        {
            JsonDataManager.Save(_villageSaveData_Init, GameManager.Instance.VillageDataPath);
        }

        if (!File.Exists(GameManager.Instance.SettingDataPath))
        {
            JsonDataManager.Save(_settingSaveData_Init, GameManager.Instance.SettingDataPath);
        }

        if (!File.Exists(GameManager.Instance.PlayerDataPath))
        {
            JsonDataManager.Save(_playerSaveData_Init, GameManager.Instance.PlayerDataPath);
        }

        if (!File.Exists(GameManager.Instance.MaterialDataPath))
        {
            JsonDataManager.Save(_materialSaveData_Init, GameManager.Instance.MaterialDataPath);
        }

        if (!File.Exists(GameManager.Instance.ItemDataPath))
        {
            JsonDataManager.Save(_itemSaveData_Init, GameManager.Instance.ItemDataPath);
        }

        if (!File.Exists(GameManager.Instance.QuestDataPath))
        {
            JsonDataManager.Save(_questSaveData, GameManager.Instance.QuestDataPath);
        }

        if (!File.Exists(GameManager.Instance.WeponDataPath))
        {
            JsonDataManager.Save(_weponSaveData, GameManager.Instance.WeponDataPath);
        }
    }
}
