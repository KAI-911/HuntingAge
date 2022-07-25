using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
public class UIItemView : UIBase
{
    [SerializeField] GameObject _itemViewPrefab;
    [SerializeField, Range(0.1f, 1.0f)] float _sideScaleSize;
    [SerializeField] float _padding;
    [SerializeField] private GameObject[] objects = new GameObject[3];
    private float _sideLength;
    [SerializeField] private List<string> _itemIDList;
    [SerializeField] private int[] intList = new int[3];
    [SerializeField] private string _currentID;
    enum position
    {
        center,
        right,
        left
    }

    private void Start()
    {
        _currentState = new NotQuest();
        _currentState.OnEnter(this, null);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        DeleteCenterUI();
        DeleteRightUI();
        DeleteLeftUI();
    }

    private class NotQuest : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            owner.GetComponent<UIItemView>().DeleteCenterUI();
            owner.GetComponent<UIItemView>().DeleteRightUI();
            owner.GetComponent<UIItemView>().DeleteLeftUI();
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log(GetType().Name);
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.GetComponent<UIItemView>().SetItemID();
            owner.GetComponent<UIItemView>()._currentID = owner.GetComponent<UIItemView>()._itemIDList[0];
        }
        public override void OnSceneChenge(UIBase owner)
        {
            if (GameManager.Instance.Quest != null && GameManager.Instance.Quest.IsQuest) owner.ChangeState<WaitItem>();
        }
    }
    private class WaitItem : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            owner.GetComponent<UIItemView>().SetCenterUI();
            owner.GetComponent<UIItemView>().SetCenterImage();
        }
        public override void OnSelectItemStart(UIBase owner)
        {
            owner.ChangeState<SelectItem>();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.GetComponent<UIItemView>().SetItemID();
            Debug.Log(GetType().Name);
        }
        public override void OnSceneChenge(UIBase owner)
        {
            if (!GameManager.Instance.Quest.IsQuest) owner.ChangeState<NotQuest>();
        }

    }
    private class UseItem : UIStateBase
    {

    }
    private class SelectItem : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            owner.GetComponent<UIItemView>().SetRightUI();
            owner.GetComponent<UIItemView>().SetRightImage();

            owner.GetComponent<UIItemView>().SetLeftUI();
            owner.GetComponent<UIItemView>().SetLeftImage();

        }
        public override void OnSelectItemEnd(UIBase owner)
        {
            owner.GetComponent<UIItemView>().DeleteLeftUI();
            owner.GetComponent<UIItemView>().DeleteRightUI();
            owner.ChangeState<WaitItem>();
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log(GetType().Name);
        }
        public override void OnProceed(UIBase owner)
        {
            var OWNER = owner.GetComponent<UIItemView>();
            if (OWNER._itemIDList.Count == 0) return;
            int index = OWNER._itemIDList.IndexOf(OWNER._currentID);
            index++;
            if (index >= OWNER._itemIDList.Count) index = 0;
            OWNER._currentID = OWNER._itemIDList[index];
            OWNER.SetCenterImage();
            OWNER.SetRightImage();
            OWNER.SetLeftImage();

        }
        public override void OnPushBoxButton(UIBase owner)
        {
            var OWNER = owner.GetComponent<UIItemView>();
            if (OWNER._itemIDList.Count == 0) return;
            int index = OWNER._itemIDList.IndexOf(OWNER._currentID);
            index--;
            if (index < 0) index = OWNER._itemIDList.Count - 1;
            OWNER._currentID = OWNER._itemIDList[index];
            OWNER.SetCenterImage();
            OWNER.SetRightImage();
            OWNER.SetLeftImage();
        }
    }

    public void SetCenterUI()
    {
        if (objects[(int)position.center] != null) return;
        var rectSize = _itemViewPrefab.GetComponent<RectTransform>().sizeDelta;
        _sideLength = rectSize.x * _sideScaleSize;
        var rect = new Vector2(SCR.Width / 2 - rectSize.x - _sideLength - (_padding * 2), -SCR.Height / 2 + rectSize.y + _padding);
        objects[(int)position.center] = Instantiate(_itemViewPrefab);
        objects[(int)position.center].transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        objects[(int)position.center].GetComponent<RectTransform>().anchoredPosition = rect;
    }
    /// <summary>
    /// 中心のUIが無ければ表示しない
    /// </summary>
    /// <returns></returns>
    public bool SetRightUI()
    {
        if (objects[(int)position.center] == null) return false;
        var centerRect = objects[(int)position.center].GetComponent<RectTransform>();
        var rectSize = centerRect.sizeDelta.y * _sideScaleSize;
        var rect = new Vector2(centerRect.anchoredPosition.x + centerRect.sizeDelta.x + _padding, centerRect.anchoredPosition.y - centerRect.sizeDelta.y / 2 + rectSize / 2);
        objects[(int)position.right] = Instantiate(_itemViewPrefab);
        objects[(int)position.right].transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        objects[(int)position.right].GetComponent<RectTransform>().anchoredPosition = rect;
        objects[(int)position.right].GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, rectSize);
        return true;
    }
    /// <summary>
    /// 中心のUIが無ければ表示しない
    /// </summary>
    /// <returns></returns>
    public bool SetLeftUI()
    {
        if (objects[(int)position.center] == null) return false;
        var centerRect = objects[(int)position.center].GetComponent<RectTransform>();
        var rectSize = centerRect.sizeDelta.y * _sideScaleSize;
        var rect = new Vector2(centerRect.anchoredPosition.x - rectSize - _padding, centerRect.anchoredPosition.y - centerRect.sizeDelta.y / 2 + rectSize / 2);
        objects[(int)position.left] = Instantiate(_itemViewPrefab);
        objects[(int)position.left].transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        objects[(int)position.left].GetComponent<RectTransform>().anchoredPosition = rect;
        objects[(int)position.left].GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, rectSize);
        return true;
    }

    public void DeleteCenterUI()
    {
        if (objects[(int)position.center] == null) return;
        Destroy(objects[(int)position.center]);
    }
    public void DeleteRightUI()
    {
        if (objects[(int)position.right] == null) return;
        Destroy(objects[(int)position.right]);
    }
    public void DeleteLeftUI()
    {
        if (objects[(int)position.left] == null) return;
        Destroy(objects[(int)position.left]);
    }

    public void SetItemID()
    {
        var data = GameManager.Instance.ItemDataList.Dictionary;
        _itemIDList.Clear();
        foreach (var item in data)
        {
            if (item.Value.baseData.PoachHoldNumber <= 0) continue;
            _itemIDList.Add(item.Key);
        }
        _itemIDList.Sort((a, b) => GameManager.Instance.ItemDataList.Dictionary[a].baseData.PoachUINumber - GameManager.Instance.ItemDataList.Dictionary[b].baseData.PoachUINumber);
    }

    public void SetRightImage()
    {
        if (_itemIDList.Count == 0) return;
        int index = _itemIDList.IndexOf(_currentID);
        index++;
        if (index >= _itemIDList.Count) index = 0;
        var images = objects[(int)position.right].GetComponentsInChildren<Image>();
        var iconname = GameManager.Instance.ItemDataList.Dictionary[_itemIDList[index]].baseData.IconName;
        images[1].sprite = Resources.Load<Sprite>(iconname);
        intList[(int)position.right] = index;
    }
    public void SetLeftImage()
    {
        if (_itemIDList.Count == 0) return;
        int index = _itemIDList.IndexOf(_currentID);
        index--;
        if (index < 0) index = _itemIDList.Count - 1;
        var images = objects[(int)position.left].GetComponentsInChildren<Image>();
        var iconname = GameManager.Instance.ItemDataList.Dictionary[_itemIDList[index]].baseData.IconName;
        images[1].sprite = Resources.Load<Sprite>(iconname);
        intList[(int)position.left] = index;
    }
    public void SetCenterImage()
    {
        if (_itemIDList.Count == 0) return;
        var images = objects[(int)position.center].GetComponentsInChildren<Image>();
        var iconname = GameManager.Instance.ItemDataList.Dictionary[_currentID].baseData.IconName;
        images[1].sprite = Resources.Load<Sprite>(iconname);
        intList[(int)position.center] = _itemIDList.IndexOf(_currentID);
    }

}
