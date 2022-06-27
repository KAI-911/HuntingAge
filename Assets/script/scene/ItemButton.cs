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
        var data = SaveData.GetClass(id, new MaterialData());
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(data.IconName);
    }
    [ContextMenu("Init")]
    void Start()
    {
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/white");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
