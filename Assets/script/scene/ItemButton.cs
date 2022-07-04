using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] string _ID;
    public string ID { get => _ID; }
    [SerializeField] Image _image;
    [SerializeField] Text _count;
    void Start()
    {
    }

    void Update()
    {

    }
    public void SetID(string id, ItemBoxOrPoach where)
    {
        _ID = id;
        var data = GameManager.Instance.ItemDataList.Dictionary[id];
        _image.sprite = Resources.Load<Sprite>(data.IconName);
        switch (where)
        {
            case ItemBoxOrPoach.box:
                _count.text = data.BoxHoldNumber.ToString();
                break;
            case ItemBoxOrPoach.poach:
                _count.text = data.PoachHoldNumber.ToString();
                break;
            default:
                break;
        }
        
    }
    public void clear()
    {
        _ID = "";
        _image.sprite = GetComponent<Sprite>();
    }

   


}
 public enum ItemBoxOrPoach
    {
        box,
        poach
    }