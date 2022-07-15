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
    [SerializeField] Sprite _mask;
    private ItemBoxOrPoach _item;
    void Start()
    {
    }

    void Update()
    {
        if (_ID == "") return;
        SetID(_ID, _item);
    }
    public void SetID(string id, ItemBoxOrPoach where)
    {
        if (!GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(id)) return;
        clear();
        _ID = id;
        var data = GameManager.Instance.MaterialDataList.Dictionary[id];
        switch (where)
        {
            case ItemBoxOrPoach.box:
                if (data.BoxHoldNumber <= 0)
                {
                    clear();
                    return;
                }
                break;
            case ItemBoxOrPoach.poach:
                if (data.PoachHoldNumber <= 0)
                {
                    clear();
                    return;
                }
                break;
            default:
                break;
        }

        _image.sprite = Resources.Load<Sprite>(data.IconName);
        _item = where;
        switch (_item)
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
        _count.text = "";
        _image.sprite = _mask;
    }




}
public enum ItemBoxOrPoach
{
    box,
    poach
}