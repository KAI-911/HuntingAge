using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionScript : MonoBehaviour
{
    [SerializeField] ItemHolder _itemHolder;
    [SerializeField] Vector3 _offest;
    private TargetChecker _targetChecker;
    private GameObject _image;
    public ItemHolder ItemHolder { get => _itemHolder; }

    [SerializeField] string _ID;
    public string ID { get => _ID; }
    private RunOnce _runOnce = new RunOnce();
    public void Awake()
    {
        _targetChecker = GetComponentInChildren<TargetChecker>();

    }
    public void OnDestroy()
    {
        if (_image != null)
        {
            Destroy(_image);
        }
    }
    public void Update()
    {
        if (_targetChecker.TriggerHit)
        {
            _runOnce.Run(
                () =>
                {
                    _image = Instantiate(_itemHolder.Dictionary[_ID]._imagePrefab);
                    _image.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
                    var text = _image.GetComponentInChildren<Text>();
                    text.text = GameManager.Instance.ItemDataList.Dictionary[_itemHolder.Dictionary[_ID].ID].Name;
                    var icon = _image.GetComponentsInChildren<Image>();
                    icon[1].sprite = _itemHolder.Dictionary[_ID].Icon;
                });
            var rext = _image.GetComponent<RectTransform>();
            rext.position = RectTransformUtility.WorldToScreenPoint(Camera.main, this.transform.position + _offest);
        }
        else
        {
            if (_runOnce.Flg == true)
            {
                Destroy(_image);
                _runOnce.Flg = false;
            }
        }
    }

    public string GetItemID()
    {
        return _itemHolder.Dictionary[_ID].ID;
    }
    public int GetNumber()
    {
        return _itemHolder.Dictionary[_ID].GetNum;
    }
}
