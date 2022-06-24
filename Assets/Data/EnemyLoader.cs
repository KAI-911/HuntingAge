using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoader : MonoBehaviour
{
    [SerializeField] bool set;
    [SerializeField] bool load;
    [SerializeField] bool setPos;
    [SerializeField] Scene scene;

    [SerializeField] string key;
    [SerializeField] EnemyData Data = new EnemyData();
    [SerializeField] GameObject[] list;
    private void Start()
    {
        set = false;
        load = false;
        setPos = false;
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
        if(setPos)
        {            
            foreach (var item in Data.EnemyPos)
            {
                if (item.scene != scene) continue;
                item.pos.Clear();
                foreach (var p in list)
                {
                    item.pos.Add(p.transform.position);
                }
                Debug.Log("セットしました");
                break;
            }

            setPos = false;
        }
    }
}
