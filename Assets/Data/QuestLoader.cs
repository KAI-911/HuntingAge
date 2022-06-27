using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLoader : MonoBehaviour
{
    [SerializeField] bool set;
    [SerializeField] bool load;

    [SerializeField] string key;
    [SerializeField] QuestData Data = new QuestData();

    private void Start()
    {
        set = false;
        load = false;
    }
    // Update is called once per frame
    //void Update()
    //{
    //    if (set)
    //    {
    //        SaveData.SetClass(key, Data);
    //        Debug.Log("セットしました");
    //        set = false;
    //    }

    //    if (load)
    //    {
    //        Data = SaveData.GetClass(key, Data);
    //        Debug.Log("ロードしました");
    //        load = false;
    //    }
    //}
}
