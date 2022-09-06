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
        //SetID(_ID, _item);
    }
    public void SetID(string id, ItemBoxOrPoach where)
    {
        if (GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(id))
        {
            clear();
            _ID = id;
            var data = GameManager.Instance.MaterialDataList.Dictionary[id];
            if(where== ItemBoxOrPoach.box&& data.BoxHoldNumber <= 0)
            {
                clear();
                return;
            }
            else if (where == ItemBoxOrPoach.poach && data.PoachHoldNumber <= 0)
            {
                clear();
                return;
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
        else if (GameManager.Instance.ItemDataList.Dictionary.ContainsKey(id))
        {
            clear();
            _ID = id;
            var data = GameManager.Instance.ItemDataList.Dictionary[id];
            if (where == ItemBoxOrPoach.box && data.baseData.BoxHoldNumber <= 0)
            {
                clear();
                return;
            }
            else if (where == ItemBoxOrPoach.poach && data.baseData.PoachHoldNumber <= 0)
            {
                clear();
                return;
            }
            _image.sprite = Resources.Load<Sprite>(data.baseData.IconName);
            _item = where;
            switch (_item)
            {
                case ItemBoxOrPoach.box:
                    _count.text = data.baseData.BoxHoldNumber.ToString();
                    break;
                case ItemBoxOrPoach.poach:
                    _count.text = data.baseData.PoachHoldNumber.ToString();
                    break;
                default:
                    break;
            }
        }

    }
    public void SetWeaponID(string id)
    {
        if (!GameManager.Instance.WeaponDataList.Dictionary.ContainsKey(id)) return;
        clear();
        _ID = id;
        var data = GameManager.Instance.WeaponDataList.Dictionary[id];
        _image.sprite = Resources.Load<Sprite>(data.IconPass);
        if (UISoundManager.Instance._player.WeaponID == data.ID)
        {
            _count.text = "E";
        }
    }

    public void SetData(string _id,string _text, Sprite _sprite)
    {
        _ID = _id;
        _count.text = _text;
        _image.sprite = _sprite;
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