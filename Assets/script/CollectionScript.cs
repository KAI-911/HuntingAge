using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionScript : MonoBehaviour
{
    [SerializeField] ItemHolder _itemHolder;
    public ItemHolder ItemHolder { get => _itemHolder; }

    [SerializeField] string _ID;
    public string ID { get => _ID; }

    public string GetRandomItemID()
    {
        int sum = 0;
        var data = _itemHolder.Dictionary[_ID];

        foreach (var item in data.collectionDatas)
        {
            sum += item.Probability;
        }
        int c = Random.Range(0, sum);
        foreach (var item in data.collectionDatas)
        {
            c -= item.Probability;
            if (c < 0)
            {
                Debug.Log(item.ID);
                return item.ID;
            }
        }
        return "";
    }

}
