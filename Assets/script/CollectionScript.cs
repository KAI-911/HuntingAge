using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionScript : PopImage
{
    [SerializeField] ItemHolder _itemHolder;
    [SerializeField] int _getItemNumber;
    public ItemHolder ItemHolder { get => _itemHolder; }

    [SerializeField] string _ID;
    public string ID { get => _ID; }

    public override void CreateImage()
    {
        _image = Instantiate(_itemHolder.Dictionary[_ID]._imagePrefab);
        _image.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        var text = _image.GetComponentInChildren<Text>();
        text.text = GameManager.Instance.MaterialDataList.Dictionary[_itemHolder.Dictionary[_ID].ID].Name;
        var icon = _image.GetComponentsInChildren<Image>();
        icon[1].sprite = _itemHolder.Dictionary[_ID].Icon;
        var rext = _image.GetComponent<RectTransform>();
        rext.anchoredPosition = new Vector2(0, rext.sizeDelta.y);
    }

    public string GetItemID()
    {
        return _itemHolder.Dictionary[_ID].ID;
    }
    public int GetNumber()
    {
        return _getItemNumber;
    }
}
