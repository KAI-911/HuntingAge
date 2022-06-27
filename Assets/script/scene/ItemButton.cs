using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] string _ID;
    public string ID { get => _ID; }
    public void SetID(string id)
    {
        _ID = id;
        var data = GameManager.Instance.ItemDataList.Dictionary[id];
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(data.IconName);
        Debug.Log(data.IconName);
    }
    public void clear()
    {
        _ID = "";
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/white");
    }


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
