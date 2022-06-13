using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class DataManager : MonoBehaviour
{
    //public List<MaterialData> _materialdata;
    //public List<ItemData> _itemData;
    public EnemyData _enemyData;
    public bool set;
    public bool load;
    //string Key = "MaterialData";
    public string Key = "key";
    void Start()
    {
        //path = Application.persistentDataPath + "/itemDatafile.json";
        set = false;
        load = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (set)
        {
            //SaveData.SetList<MaterialData>(Key, _materialdata);
            //SaveData.Save();

            SaveData.SetClass<EnemyData>(Key, _enemyData);
            Debug.Log("set");
            set = false;
        }
        if (load)
        {
            //_materialdata = SaveData.GetList<MaterialData>(Key, new List<MaterialData>());
            _enemyData = SaveData.GetClass<EnemyData>(Key, new EnemyData());
            load = false;
        }
    }
}
