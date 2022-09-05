using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionScript : PopImage
{
    [SerializeField] ItemHolder _itemHolder;
    [SerializeField] int _getItemNumber;
    [SerializeField] int _collectableTimes = 3;
    int _keepCollectableTimes;
    [SerializeField] float _repopTime = 20;
    float _repopTimeCount;
    public ItemHolder ItemHolder { get => _itemHolder; }

    [SerializeField] string _ID;
    public string ID { get => _ID; }
    public int CollectableTimes { get => _collectableTimes; set => _collectableTimes = value; }

    public void Start()
    {
        _repopTimeCount = _repopTime;
        _keepCollectableTimes = _collectableTimes;
    }
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
    public override void Update()
    {
        base.Update();
        //全て採取した後のリポップ処理
        if(_collectableTimes<=0)
        {
            _repopTimeCount -= Time.deltaTime;
            if(_repopTimeCount<=0)
            {
                _collectableTimes = _keepCollectableTimes;
                _repopTimeCount = _repopTime;
            }
        }
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
