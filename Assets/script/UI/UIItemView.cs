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
    private GameObject _rightButton;
    private GameObject _leftButton;

    private float _sideLength;
    private List<string> _itemIDList = new List<string>();
    private string _currentID;
    enum position
    {
        center,
        right,
        left
    }
    [SerializeField] GameObject _attackUpprefab;
    [SerializeField] GameObject _defenseUpprefab;
    [SerializeField] GameObject _hpUpprefab;

    GameObject _attackUpEffect;
    GameObject _defenseUpEffect;
    GameObject _hpUpEffect;
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
        UIDelete();
        ChangeState<NotQuest>();
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
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.GetComponent<UIItemView>().SetItemID();
            if (owner.GetComponent<UIItemView>()._itemIDList.Count != 0)
            {
                owner.GetComponent<UIItemView>()._currentID = owner.GetComponent<UIItemView>()._itemIDList[0];
            }
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
            OWNER.CreateCenterUI();
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
        //public override void OnPushBoxButton(UIBase owner)
        //{
        //    if (GameManager.Instance.Player.CollectionScript == null)
        //    {
        //        owner.ChangeState<UseItem>();
        //    }
        //}
        public override void OnSceneChenge(UIBase owner)
        {
            if (!GameManager.Instance.Quest.IsQuest)
            {
                var OWNER = owner.GetComponent<UIItemView>();
                OWNER.DeleteCenterUI();
                OWNER.DeleteRightUI();
                OWNER.DeleteLeftUI();
                Destroy(OWNER._attackUpEffect);
                Destroy(OWNER._defenseUpEffect);
                Destroy(OWNER._hpUpEffect);
                owner.ChangeState<NotQuest>();

            }
        }
    }
    private class UseItem : UIStateBase
    {
        private Run run = new Run();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            if (prevState.GetType() == typeof(WaitItem))
            {
                var OWNER = owner.GetComponent<UIItemView>();

                var data = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[OWNER._currentID];
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
                        OWNER._hpUpEffect = Instantiate(OWNER._hpUpprefab, UISoundManager.Instance._player.transform) as GameObject;
                        Destroy(OWNER._hpUpEffect, 3);
                        break;
                    case ItemType.AttackUp:
                        if (data.Use) return;
                        if (!data.Permanent)
                        {
                            _ = run.WaitForAsync(data.Time, () =>
                            {
                                if (!data.Use) return;
                                data.Use = false;
                                status.Attack = GameManager.Instance.StatusData.PlayerSaveData.Attack;
                                GameManager.Instance.Player.Status = status;
                                GameManager.Instance.ItemDataList._itemSaveData.Dictionary[OWNER._currentID] = data;
                                if (OWNER._attackUpEffect != null) Destroy(OWNER._attackUpEffect);
                            });
                        }
                        data.Use = true;
                        data.baseData.PoachHoldNumber--;
                        status.Attack += (int)data.UpValue;
                        OWNER._attackUpEffect = Instantiate(OWNER._attackUpprefab, UISoundManager.Instance._player.transform) as GameObject;
                        break;
                    case ItemType.DefenseUp:
                        if (data.Use) return;
                        if (!data.Permanent)
                        {
                            _ = run.WaitForAsync(data.Time, () =>
                            {
                                if (!data.Use) return;
                                data.Use = false;
                                GameManager.Instance.Player.Status.Defense = GameManager.Instance.StatusData.PlayerSaveData.Defense;
                                GameManager.Instance.ItemDataList._itemSaveData.Dictionary[OWNER._currentID] = data;
                                if (OWNER._defenseUpEffect != null) Destroy(OWNER._defenseUpEffect);
                            });
                        }
                        data.Use = true;
                        data.baseData.PoachHoldNumber--;
                        status.Defense += (int)data.UpValue;
                        OWNER._defenseUpEffect = Instantiate(OWNER._defenseUpprefab, UISoundManager.Instance._player.transform) as GameObject;

                        break;
                    default:
                        break;
                }
                GameManager.Instance.Player.Status = status;
                GameManager.Instance.ItemDataList._itemSaveData.Dictionary[OWNER._currentID] = data;
                OWNER.SetCenterImage();
            }

        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ChangeState<WaitItem>();
        }
        public override void OnSceneChenge(UIBase owner)
        {
            if (!GameManager.Instance.Quest.IsQuest)
            {
                var OWNER = owner.GetComponent<UIItemView>();
                OWNER.DeleteCenterUI();
                OWNER.DeleteRightUI();
                OWNER.DeleteLeftUI();
                owner.ChangeState<NotQuest>();
            }
        }
    }
    private class SelectItem : UIStateBase
    {
        bool lockflg;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var OWNER = owner.GetComponent<UIItemView>();

            OWNER.CreateRightUI();
            OWNER.SetRightImage();

            OWNER.CreateLeftUI();
            OWNER.SetLeftImage();
            OWNER.CreateLightButton();
            OWNER.CreateRightButton();
            lockflg = false;
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

            var OWNER = owner.GetComponent<UIItemView>();
            var inputVec = UISoundManager.Instance.InputItemView.ReadValue<float>();
            if (inputVec > 0)
            {
                if (lockflg) return;
                if (OWNER._itemIDList.Count == 0) return;
                LeftToRighet(OWNER);
                lockflg = true;
            }
            else if (inputVec < 0)
            {
                if (lockflg) return;
                if (OWNER._itemIDList.Count == 0) return;
                RighetToLeft(OWNER);
                lockflg = true;
            }
            else
            {
                lockflg = false;
            }

        }

        public override void OnSceneChenge(UIBase owner)
        {
            if (!GameManager.Instance.Quest.IsQuest)
            {
                var OWNER = owner.GetComponent<UIItemView>();
                OWNER.DeleteCenterUI();
                OWNER.DeleteRightUI();
                OWNER.DeleteLeftUI();
                owner.ChangeState<NotQuest>();
            }
        }

        private void LeftToRighet(UIItemView OWNER)
        {
            int index = OWNER._itemIDList.IndexOf(OWNER._currentID);
            index++;
            if (index >= OWNER._itemIDList.Count) index = 0;
            OWNER._currentID = OWNER._itemIDList[index];
            OWNER.SetCenterImage();
            OWNER.SetRightImage();
            OWNER.SetLeftImage();
        }
        private void RighetToLeft(UIItemView OWNER)
        {
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

    public void UIDelete()
    {
        DeleteCenterUI();
        DeleteRightUI();
        DeleteLeftUI();
        Destroy(_attackUpEffect);
        Destroy(_defenseUpEffect);
        Destroy(_hpUpEffect);
        ChangeState<NotQuest>();
    }
    public void CreateLightButton()
    {
        if (_rightButton != null) return;
        if (objects[(int)position.center] == null) return;
        Debug.Log("Light");
        _rightButton = Instantiate(_maruButtonPrefab);
        _rightButton.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        var rect = _rightButton.GetComponent<RectTransform>();
        var centerRect = objects[(int)position.center].GetComponent<RectTransform>();
        var pos = centerRect.anchoredPosition;
        pos.x -= rect.sizeDelta.x / 2;
        pos.y -= centerRect.sizeDelta.y - rect.sizeDelta.y / 2;
        rect.anchoredPosition = pos;
    }
    public void CreateRightButton()
    {
        if (_leftButton != null) return;
        if (objects[(int)position.center] == null) return;
        Debug.Log("Right");
        _leftButton = Instantiate(_shikakuButtonPrefab);
        _leftButton.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        var rect = _leftButton.GetComponent<RectTransform>();
        var centerRect = objects[(int)position.center].GetComponent<RectTransform>();
        var pos = centerRect.anchoredPosition;
        pos.x += centerRect.sizeDelta.x - rect.sizeDelta.x / 2;
        pos.y -= centerRect.sizeDelta.y - rect.sizeDelta.y / 2;
        rect.anchoredPosition = pos;
    }
    public void DeleteLightButton()
    {
        if (_rightButton == null) return;
        Destroy(_rightButton);
        _rightButton = null;
    }
    public void DeleteRightButton()
    {
        if (_leftButton == null) return;
        Destroy(_leftButton);
        _leftButton = null;
    }

    public void CreateCenterUI()
    {
        if (objects[(int)position.center] != null) return;
        var rectSize = _itemViewPrefab.GetComponent<RectTransform>().sizeDelta;
        _sideLength = rectSize.x * _sideScaleSize;
        var rect = new Vector2(Screen.width / 2 - rectSize.x - _sideLength - (_padding * 2), -Screen.height / 2 + rectSize.y + _padding);
        objects[(int)position.center] = Instantiate(_itemViewPrefab);
        objects[(int)position.center].transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        objects[(int)position.center].GetComponent<RectTransform>().anchoredPosition = rect;
    }
    /// <summary>
    /// 中心のUIが無ければ表示しない
    /// </summary>
    /// <returns></returns>
    public bool CreateRightUI()
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
    public bool CreateLeftUI()
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
    //UIの削除
    public void DeleteCenterUI()
    {
        if (objects[(int)position.center] == null) return;
        Destroy(objects[(int)position.center]);
        objects[(int)position.center] = null;

    }
    //UIの削除
    public void DeleteRightUI()
    {
        if (objects[(int)position.right] == null) return;
        Destroy(objects[(int)position.right]);
        objects[(int)position.right] = null;
    }
    //UIの削除
    public void DeleteLeftUI()
    {
        if (objects[(int)position.left] == null) return;
        Destroy(objects[(int)position.left]);
        objects[(int)position.left] = null;

    }
    //持っているアイテムのリスト
    public void SetItemID()
    {
        var data = GameManager.Instance.ItemDataList._itemSaveData.Dictionary;
        _itemIDList.Clear();
        foreach (var item in data)
        {
            if (item.Value.baseData.PoachHoldNumber <= 0) continue;
            _itemIDList.Add(item.Key);
        }
        _itemIDList.Sort((a, b) => GameManager.Instance.ItemDataList._itemSaveData.Dictionary[a].baseData.PoachUINumber - GameManager.Instance.ItemDataList._itemSaveData.Dictionary[b].baseData.PoachUINumber);
    }
    //アイコンの設定
    public void SetRightImage()
    {
        if (_itemIDList.Count == 0) return;
        if (objects[(int)position.right] == null) return;
        //_currentIDの一つ次のアイテムを表示
        int index = _itemIDList.IndexOf(_currentID);
        index++;
        //_currentIDが先頭の場合一番先頭のアイテムを表示
        if (index >= _itemIDList.Count) index = 0;
        var images = objects[(int)position.right].GetComponentsInChildren<Image>();
        var iconname = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[_itemIDList[index]].baseData.IconName;
        images[1].sprite = Resources.Load<Sprite>(iconname);
        var text = objects[(int)position.center].GetComponentInChildren<Text>();
        text.text = "";

    }
    //アイコンの設定
    public void SetLeftImage()
    {
        if (_itemIDList.Count == 0) return;
        if (objects[(int)position.left] == null) return;
        //_currentIDの一つ手前のアイテムを表示
        int index = _itemIDList.IndexOf(_currentID);
        index--;
        //_currentIDが先頭の場合一番後ろのアイテムを表示
        if (index < 0) index = _itemIDList.Count - 1;

        var images = objects[(int)position.left].GetComponentsInChildren<Image>();
        var iconname = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[_itemIDList[index]].baseData.IconName;
        images[1].sprite = Resources.Load<Sprite>(iconname);
        var text = objects[(int)position.center].GetComponentInChildren<Text>();
        text.text = "";

    }
    //アイコンの設定
    public void SetCenterImage()
    {
        if (!_itemIDList.Contains(_currentID)) return;
        if (objects[(int)position.center] == null) return;
        //既にリスト内に登録されている、オブジェクトがある
        var data = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[_currentID].baseData;
        var images = objects[(int)position.center].GetComponentsInChildren<Image>();
        var text = objects[(int)position.center].GetComponentInChildren<Text>();
        //もっていない場合は次のアイテムを表示させる
        if (data.PoachHoldNumber <= 0)
        {
            //一つももっていない場合は削除
            int index = _itemIDList.IndexOf(_currentID);
            _itemIDList.Remove(_currentID);

            //全てのアイテムを使っていたら
            if (_itemIDList.Count == 0)
            {
                images[1].sprite = Resources.Load<Sprite>("Icon/alpha");
                text.text = "";
            }
            //後ろのアイテムがある場合はそのアイテムを表示
            else if (_itemIDList.Count > index)
            {
                _currentID = _itemIDList[index];
                var nextData = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[_currentID].baseData;
                images[1].sprite = Resources.Load<Sprite>(nextData.IconName);
                text.text = nextData.PoachHoldNumber.ToString();
            }
            //もし、リストの範囲外になっていたら_itemIDListの先頭アイテムを表示
            else if (_itemIDList.Count <= index)
            {
                _currentID = _itemIDList[0];
                var nextData = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[_currentID].baseData;
                images[1].sprite = Resources.Load<Sprite>(nextData.IconName);
                text.text = nextData.PoachHoldNumber.ToString();
            }
        }
        else
        {
            images[1].sprite = Resources.Load<Sprite>(data.IconName);
            text.text = data.PoachHoldNumber.ToString();
        }



        //if (_itemIDList.Count == 0) return;
        //if (objects[(int)position.center] == null) return;

        //var images = objects[(int)position.center].GetComponentsInChildren<Image>();
        //var data = GameManager.Instance.ItemDataList.Dictionary[_currentID].baseData;
        //if (data.PoachHoldNumber == 0)
        //{
        //    images[1].sprite = Resources.Load<Sprite>("Icon/alpha");
        //}
        //else
        //{
        //    images[1].sprite = Resources.Load<Sprite>(data.IconName);
        //}
        //var text = objects[(int)position.center].GetComponentInChildren<Text>();
        //text.text = data.PoachHoldNumber.ToString();
    }
    /// <summary>
    /// 永続効果のアイテムの効果を削除
    /// </summary>
    public void ClearPermanentBuff()
    {
        var list = GameManager.Instance.ItemDataList._itemSaveData.Dictionary;
        foreach (var item in list)
        {
            //永続効果でなければ戻る
            if (!item.Value.Permanent) continue;
            //使用していなかったら戻る
            if (!item.Value.Use) continue;
            var data = list[item.Key];
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
            list[item.Key] = data;
        }
    }
    public void ItemUseEnd()
    {
        if (this._currentState.GetType() == typeof(WaitItem))
        {
            ChangeState<UseItem>();
        }
    }
    public bool IsSelect()
    {
        if (this._currentState.GetType() == typeof(WaitItem)) return false;
        return true;
    }
}
