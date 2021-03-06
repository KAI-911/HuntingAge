using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using System;
public class UIItemView : UIBase
{
    [SerializeField] GameObject _itemViewPrefab;
    [SerializeField] GameObject _maruButtonPrefab;
    [SerializeField] GameObject _shikakuButtonPrefab;
    [SerializeField, Range(0.1f, 1.0f)] float _sideScaleSize;
    [SerializeField] float _padding;
    private GameObject[] objects = new GameObject[3];
    private GameObject _maruButton;
    private GameObject _shikakuButton;

    private float _sideLength;
    private List<string> _itemIDList = new List<string>();
    private string _currentID;
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
    public void ChangeNotQuestState()
    {
        GetComponent<UIBase>().ChangeState<NotQuest>();
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
            var OWNER = owner.GetComponent<UIItemView>();
            OWNER.SetCenterUI();
            OWNER.SetCenterImage();
            OWNER.CreateLightButton();
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            var OWNER = owner.GetComponent<UIItemView>();
            OWNER.DeleteLightButton();
        }
        public override void OnSelectItemStart(UIBase owner)
        {
            owner.ChangeState<SelectItem>();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.GetComponent<UIItemView>().SetItemID();
        }
        public override void OnPushBoxButton(UIBase owner)
        {
            owner.ChangeState<UseItem>();
        }
        public override void OnSceneChenge(UIBase owner)
        {
            if (!GameManager.Instance.Quest.IsQuest)
            {
                owner.ChangeState<NotQuest>();
            }
        }
    }
    private class UseItem : UIStateBase
    {
        private Run run = new Run();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var OWNER = owner.GetComponent<UIItemView>();

            int index = GameManager.Instance.ItemDataList.Keys.IndexOf(OWNER._currentID);
            var data = GameManager.Instance.ItemDataList.Values[index];
            Status status = GameManager.Instance.Player.Status;
            switch (data.ItemType)
            {
                case ItemType.HpRecovery:
                    status.HP += (int)data.UpValue;
                    data.baseData.PoachHoldNumber--;
                    if (status.HP > status.MaxHP)
                    {
                        status.HP = status.MaxHP;
                    }
                    break;
                case ItemType.AttackUp:
                    if (data.Use) return;
                    if (!data.Permanent)
                    {
                        _ = run.WaitForAsync(data.Time, () =>
                        {
                            if (!data.Use) return;
                            data.Use = false;
                            status.Attack -= (int)data.UpValue;
                            GameManager.Instance.Player.Status = status;
                            GameManager.Instance.ItemDataList.Values[index] = data;
                            GameManager.Instance.ItemDataList.DesrializeDictionary();
                        });
                    }
                    data.Use = true;
                    data.baseData.PoachHoldNumber--;
                    status.Attack += (int)data.UpValue;
                    break;
                case ItemType.DefenseUp:
                    if (data.Use) return;
                    if (!data.Permanent)
                    {
                        _ = run.WaitForAsync(data.Time, () =>
                        {
                            if (!data.Use) return;
                            data.Use = false;
                            status.Defense -= (int)data.UpValue;
                            GameManager.Instance.Player.Status = status;
                            GameManager.Instance.ItemDataList.Values[index] = data;
                            GameManager.Instance.ItemDataList.DesrializeDictionary();
                        });
                    }
                    data.Use = true;
                    data.baseData.PoachHoldNumber--;
                    status.Defense += (int)data.UpValue;

                    break;
                default:
                    break;
            }
            GameManager.Instance.Player.Status = status;
            GameManager.Instance.ItemDataList.Values[index] = data;
            GameManager.Instance.ItemDataList.DesrializeDictionary();
            OWNER.SetCenterImage();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ChangeState<WaitItem>();
        }
        public override void OnSceneChenge(UIBase owner)
        {
            if (!GameManager.Instance.Quest.IsQuest)
            {
                owner.ChangeState<NotQuest>();
            }
        }

    }
    private class SelectItem : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var OWNER = owner.GetComponent<UIItemView>();

            OWNER.SetRightUI();
            OWNER.SetRightImage();

            OWNER.SetLeftUI();
            OWNER.SetLeftImage();
            OWNER.CreateLightButton();
            OWNER.CreateRightButton();

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            var OWNER = owner.GetComponent<UIItemView>();
            OWNER.DeleteLightButton();
            OWNER.DeleteRightButton();
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
        public override void OnSceneChenge(UIBase owner)
        {
            if (!GameManager.Instance.Quest.IsQuest)
            {
                owner.ChangeState<NotQuest>();
            }
        }

    }

    public void CreateLightButton()
    {
        if (_maruButton != null) return;
        if (objects[(int)position.center] == null) return;
        Debug.Log("Light");
        _maruButton = Instantiate(_maruButtonPrefab);
        _maruButton.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        var rect = _maruButton.GetComponent<RectTransform>();
        var centerRect = objects[(int)position.center].GetComponent<RectTransform>();
        var pos = centerRect.anchoredPosition;
        pos.x -= rect.sizeDelta.x / 2;
        pos.y -= centerRect.sizeDelta.y - rect.sizeDelta.y / 2;
        rect.anchoredPosition = pos;
    }
    public void CreateRightButton()
    {
        if (_shikakuButton != null) return;
        if (objects[(int)position.center] == null) return;
        Debug.Log("Right");
        _shikakuButton = Instantiate(_shikakuButtonPrefab);
        _shikakuButton.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        var rect = _shikakuButton.GetComponent<RectTransform>();
        var centerRect = objects[(int)position.center].GetComponent<RectTransform>();
        var pos = centerRect.anchoredPosition;
        pos.x += centerRect.sizeDelta.x - rect.sizeDelta.x / 2;
        pos.y -= centerRect.sizeDelta.y - rect.sizeDelta.y / 2;
        rect.anchoredPosition = pos;
    }
    public void DeleteLightButton()
    {
        if (_maruButton == null) return;
        Destroy(_maruButton);
        _maruButton = null;
    }
    public void DeleteRightButton()
    {
        if (_shikakuButton == null) return;
        Destroy(_shikakuButton);
        _shikakuButton = null;
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
    /// ???S??UI???????????\????????
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
    /// ???S??UI???????????\????????
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
    //UI??????
    public void DeleteCenterUI()
    {
        if (objects[(int)position.center] == null) return;
        Destroy(objects[(int)position.center]);
        objects[(int)position.center] = null;

    }
    //UI??????
    public void DeleteRightUI()
    {
        if (objects[(int)position.right] == null) return;
        Destroy(objects[(int)position.right]);
        objects[(int)position.right] = null;
    }
    //UI??????
    public void DeleteLeftUI()
    {
        if (objects[(int)position.left] == null) return;
        Destroy(objects[(int)position.left]);
        objects[(int)position.left] = null;

    }
    //???????????A?C?e???????X?g
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
    //?A?C?R????????
    public void SetRightImage()
    {
        if (_itemIDList.Count == 0) return;
        if (objects[(int)position.right] == null) return;
        int index = _itemIDList.IndexOf(_currentID);
        index++;
        if (index >= _itemIDList.Count) index = 0;
        var images = objects[(int)position.right].GetComponentsInChildren<Image>();
        var iconname = GameManager.Instance.ItemDataList.Dictionary[_itemIDList[index]].baseData.IconName;
        images[1].sprite = Resources.Load<Sprite>(iconname);
    }
    //?A?C?R????????
    public void SetLeftImage()
    {
        if (_itemIDList.Count == 0) return;
        if (objects[(int)position.left] == null) return;

        int index = _itemIDList.IndexOf(_currentID);
        index--;
        if (index < 0) index = _itemIDList.Count - 1;
        var images = objects[(int)position.left].GetComponentsInChildren<Image>();
        var iconname = GameManager.Instance.ItemDataList.Dictionary[_itemIDList[index]].baseData.IconName;
        images[1].sprite = Resources.Load<Sprite>(iconname);
    }
    //?A?C?R????????
    public void SetCenterImage()
    {
        if (_itemIDList.Count == 0) return;
        if (objects[(int)position.center] == null) return;

        var images = objects[(int)position.center].GetComponentsInChildren<Image>();
        var data = GameManager.Instance.ItemDataList.Dictionary[_currentID].baseData;
        if (data.PoachHoldNumber == 0)
        {
            images[1].sprite = Resources.Load<Sprite>("Icon/alpha");
        }
        else
        {
            images[1].sprite = Resources.Load<Sprite>(data.IconName);
        }
    }
    /// <summary>
    /// ?i?????????A?C?e??????????????
    /// </summary>
    public void ClearPermanentBuff()
    {
        var list = GameManager.Instance.ItemDataList;
        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
        {
            //?i????????????????????
            if (!item.Value.Permanent) continue;
            //?g?p????????????????????
            if (!item.Value.Use) continue;
            int index = list.Keys.IndexOf(item.Key);
            var data = list.Values[index];
            data.Use = false;
            switch (data.ItemType)
            {
                case ItemType.AttackUp:
                    GameManager.Instance.Player.Status.Attack -= data.UpValue;
                    break;
                case ItemType.DefenseUp:
                    GameManager.Instance.Player.Status.Defense -= data.UpValue;
                    break;
                default:
                    break;
            }
            list.Values[index] = data;
        }
        GameManager.Instance.ItemDataList.DesrializeDictionary();
    }
}
