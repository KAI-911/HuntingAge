using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] string _ID;
    public string ID { get => _ID; }
    [SerializeField] Image _image;

    void Start()
    {
        
    }

    void Update()
    {

    }
    public void SetID(string id)
    {
        _ID = id;
        var data = GameManager.Instance.ItemDataList.Dictionary[id];
        _image.sprite = Resources.Load<Sprite>(data.IconName);
    }
    public void clear()
    {
        _ID = "";
        _image.sprite = GetComponent<Sprite>();
    }



}
