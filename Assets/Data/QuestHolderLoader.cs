using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHolderLoader : MonoBehaviour
{
    [SerializeField] bool set;
    [SerializeField] bool load;

    [SerializeField] string key;
    [SerializeField] QuestHolder Data = new QuestHolder();

    private void Start()
    {
        set = false;
        load = false;

    }
    // Update is called once per frame
    void Update()
    {
        if (set)
        {
            SaveData.SetClass(key, Data);
            Debug.Log("セットしました");
            set = false;
        }

        if (load)
        {
            Data = SaveData.GetClass(key, Data);
            Debug.Log("ロードしました");
            load = false;
        }
    }

}
