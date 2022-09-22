using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using System;
public class UIItemView_new : UIBase
{
    [SerializeField] GameObject _itemViewPrefab;
    [SerializeField] GameObject _rightButtonPrefab;
    [SerializeField] GameObject _leftButtonPrefab;
    [SerializeField, Range(0.1f, 0.9f)] float _sideScaleSize;
    [SerializeField] float _padding;

    [SerializeField] GameObject _attackUpprefab;
    [SerializeField] GameObject _defenseUpprefab;
    [SerializeField] GameObject _hpUpprefab;

    private GameObject[] objects = new GameObject[5];
    private Vector3[] scale = new Vector3[5];
    private Vector2[] pos = new Vector2[5];
    private GameObject _rightButton;
    private GameObject _leftButton;

    GameObject _attackUpEffect;
    GameObject _defenseUpEffect;
    GameObject _hpUpEffect;

    private List<string> _itemIDList = new List<string>();
    private string _currentID;

    private Vector2 _iconSize;
    enum position
    {
        rightend,
        right,
        center,
        left,
        leftend
    }

    private void Start()
    {
        _currentState = new Village();
        _currentState.OnEnter(this, null);
        _iconSize = _itemViewPrefab.GetComponent<RectTransform>().sizeDelta;
        //左端ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
        Vector2 tmpPos = new Vector2(Screen.width / 2, -Screen.height / 2);
        tmpPos.y += _iconSize.y / 2 + _padding;
        tmpPos.x -= _padding + (_iconSize.x * _sideScaleSize) + _padding + _iconSize.x + _padding + (_iconSize.x * _sideScaleSize) + _padding + _padding;
        scale[(int)position.leftend] = new Vector3(0, 0, 1);
        pos[(int)position.leftend] = tmpPos;
        //左ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
        tmpPos = new Vector2(Screen.width / 2, -Screen.height / 2);
        tmpPos.y += _iconSize.y / 2 + _padding;
        tmpPos.x -= (_iconSize.x / 2 * _sideScaleSize) + _padding + _iconSize.x + _padding + (_iconSize.x * _sideScaleSize) + _padding + _padding;
        scale[(int)position.left] = new Vector3(_sideScaleSize, _sideScaleSize, 1);
        pos[(int)position.left] = tmpPos;
        //中心ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
        tmpPos = new Vector2(Screen.width / 2, -Screen.height / 2);
        tmpPos.y += _iconSize.y / 2 + _padding;
        tmpPos.x -= _iconSize.x / 2 + _padding + _iconSize.x * _sideScaleSize + _padding + _padding;
        scale[(int)position.center] = new Vector3(1, 1, 1);
        pos[(int)position.center] = tmpPos;
        //右ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
        tmpPos = new Vector2(Screen.width / 2, -Screen.height / 2);
        tmpPos.y += _iconSize.y / 2 + _padding;
        tmpPos.x -= (_iconSize.x / 2 * _sideScaleSize) + _padding + _padding;
        scale[(int)position.right] = new Vector3(_sideScaleSize, _sideScaleSize, 1);
        pos[(int)position.right] = tmpPos;
        //右端ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
        tmpPos = new Vector2(Screen.width / 2, -Screen.height / 2);
        tmpPos.y += _iconSize.y / 2 + _padding;
        tmpPos.x -= _padding;
        scale[(int)position.rightend] = new Vector3(0, 0, 1);
        pos[(int)position.rightend] = tmpPos;

    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void Update()
    {
        base.Update();
    }
    private class Village : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            owner.GetComponent<UIItemView_new>().AllDelete();
        }
        public override void OnSceneChenge(UIBase owner)
        {
            if (GameManager.Instance.Quest != null && GameManager.Instance.Quest.IsQuest)
            {
                var OWNER = owner.GetComponent<UIItemView_new>();
                OWNER.SetItemIDList();
                if (OWNER._itemIDList.Count != 0)
                {
                    OWNER._currentID = OWNER._itemIDList[0];
                }
                else
                {
                    OWNER._currentID = "";
                }
                owner.ChangeState<WaitItem>();
            }
        }
    }

    private class WaitItem : UIStateBase
    {
        UIItemView_new OWNER;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            OWNER = owner.GetComponent<UIItemView_new>();
            OWNER.CheckID();
            OWNER.SetIcon(position.center);
            OWNER.SetLeftButton();
        }
        public override void OnSelectItemStart(UIBase owner)
        {
            owner.ChangeState<SelectItem>();
        }
        public override void OnSceneChenge(UIBase owner)
        {
            if (GameManager.Instance.Quest != null && !GameManager.Instance.Quest.IsQuest)
            {
                owner.ChangeState<Village>();
            }
        }
    }
    private class SelectItem : UIStateBase
    {
        UIItemView_new OWNER;
        bool lockflg;
        int moveCount;
        float moveTime;
        float time;
        int index;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            time = 0;
            moveCount = 0;
            moveTime = 0.2f;
            lockflg = false;
            OWNER = owner.GetComponent<UIItemView_new>();
            OWNER.SetIcon(position.center);
            OWNER.SetIcon(position.right);
            OWNER.SetIcon(position.rightend);
            OWNER.SetIcon(position.left);
            OWNER.SetIcon(position.leftend);
            OWNER.SetLeftButton();
            OWNER.SetRightButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            if (OWNER._itemIDList.Count == 0) return;
            var inputVec = UISoundManager.Instance.InputItemView.ReadValue<float>();
            if (inputVec < 0)
            {
                if (lockflg) return;
                moveCount++;
                lockflg = true;
            }
            else if (inputVec > 0)
            {
                if (lockflg) return;
                moveCount--;
                lockflg = true;
            }
            else
            {
                lockflg = false;
            }

            Debug.Log(moveCount);
            //右に移動させる
            if (moveCount > 0)
            {
                Debug.Log("右に動くはず");
                time += Time.deltaTime;
                MoveIcon(position.leftend, position.left);
                MoveIcon(position.left, position.center);
                MoveIcon(position.center, position.right);
                MoveIcon(position.right, position.rightend);
                MoveIcon(position.rightend, position.leftend);
                if (Mathf.Clamp(time / moveTime, 0, 1.0f) >= 1.0f)
                {
                    time -= moveTime;
                    moveCount--;
                    GameObject tmp = OWNER.objects[(int)position.rightend];
                    OWNER.objects[(int)position.rightend] = OWNER.objects[(int)position.right];
                    OWNER.objects[(int)position.right] = OWNER.objects[(int)position.center];
                    OWNER.objects[(int)position.center] = OWNER.objects[(int)position.left];
                    OWNER.objects[(int)position.left] = OWNER.objects[(int)position.leftend];
                    OWNER.objects[(int)position.leftend] = tmp;
                    OWNER._currentID = OWNER._itemIDList[OWNER.GetIndex(1)];
                    OWNER.SetIcon(position.center);
                    OWNER.SetIcon(position.right);
                    OWNER.SetIcon(position.rightend);
                    OWNER.SetIcon(position.left);
                    OWNER.SetIcon(position.leftend);
                }
            }
            //左に移動させる
            else if (moveCount < 0)
            {
                Debug.Log("左に動くはず");
                time += Time.deltaTime;
                MoveIcon(position.rightend, position.right);
                MoveIcon(position.right, position.center);
                MoveIcon(position.center, position.left);
                MoveIcon(position.left, position.leftend);
                MoveIcon(position.leftend, position.rightend);
                if (Mathf.Clamp(time / moveTime, 0, 1.0f) >= 1.0f)
                {
                    time -= moveTime;
                    moveCount++;
                    GameObject tmp = OWNER.objects[(int)position.leftend];
                    OWNER.objects[(int)position.leftend] = OWNER.objects[(int)position.left];
                    OWNER.objects[(int)position.left] = OWNER.objects[(int)position.center];
                    OWNER.objects[(int)position.center] = OWNER.objects[(int)position.right];
                    OWNER.objects[(int)position.right] = OWNER.objects[(int)position.rightend];
                    OWNER.objects[(int)position.rightend] = tmp;
                    OWNER._currentID = OWNER._itemIDList[OWNER.GetIndex(-1)];
                    OWNER.SetIcon(position.center);
                    OWNER.SetIcon(position.right);
                    OWNER.SetIcon(position.rightend);
                    OWNER.SetIcon(position.left);
                    OWNER.SetIcon(position.leftend);
                }
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            int tmp = OWNER.GetIndex(index);
            if (index < 0)
            {
                var itemData = GameManager.Instance.ItemDataList.Dictionary[OWNER._itemIDList[tmp]];
                var image = OWNER.objects[(int)position.center].GetComponentsInChildren<Image>()[1];
                image.sprite = Resources.Load<Sprite>(itemData.baseData.IconName);
                OWNER._currentID = itemData.baseData.ID;
            }


            for (int i = 0; i < OWNER.objects.Length; i++)
            {
                if (OWNER.objects[i] == null) continue;
                Destroy(OWNER.objects[i]);
                OWNER.objects[i] = null;
            }

            OWNER.DeleteRightButton();
            OWNER.DeleteLeftButton();

        }
        public override void OnSelectItemEnd(UIBase owner)
        {
            owner.ChangeState<WaitItem>();
        }


        private void MoveIcon(position _from, position _to)
        {
            float t = Mathf.Clamp(time / moveTime, 0, 1);
            var p = OWNER.pos[(int)_to] - OWNER.pos[(int)_from];
            var s = OWNER.scale[(int)_to] - OWNER.scale[(int)_from];
            var rect = OWNER.objects[(int)_from].GetComponent<RectTransform>();
            rect.localScale = OWNER.scale[(int)_from] + s * t;
            rect.anchoredPosition = OWNER.pos[(int)_from] + p * t;
        }
    }
    private class UseItem : UIStateBase
    {
        UIItemView_new OWNER;
        private Run run = new Run();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            OWNER = owner.GetComponent<UIItemView_new>();
            if (prevState.GetType() == typeof(WaitItem))
            {

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
                                status.Attack = UISoundManager.Instance._player.StatusData.Attack;
                                GameManager.Instance.Player.Status = status;
                                GameManager.Instance.ItemDataList.Values[index] = data;
                                GameManager.Instance.ItemDataList.DesrializeDictionary();
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
                                GameManager.Instance.Player.Status.Defense = GameManager.Instance.Player.StatusData.Defense;
                                GameManager.Instance.ItemDataList.Values[index] = data;
                                GameManager.Instance.ItemDataList.DesrializeDictionary();
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
                GameManager.Instance.ItemDataList.Values[index] = data;
                GameManager.Instance.ItemDataList.DesrializeDictionary();
                OWNER.SetIcon(position.center);
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
                owner.ChangeState<Village>();
            }
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
        return !(this._currentState.GetType() == typeof(WaitItem));
    }

    /// <summary>
    /// 永続効果のアイテムの効果を削除
    /// </summary>
    public void ClearPermanentBuff()
    {
        var list = GameManager.Instance.ItemDataList;
        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
        {
            //永続効果でなければ戻る
            if (!item.Value.Permanent) continue;
            //使用していなかったら戻る
            if (!item.Value.Use) continue;
            int index = list.Keys.IndexOf(item.Key);
            var data = list.Values[index];
            data.Use = false;
            switch (data.ItemType)
            {
                case ItemType.AttackUp:
                    GameManager.Instance.Player.Status.Attack = GameManager.Instance.Player.StatusData.Attack;
                    break;
                case ItemType.DefenseUp:
                    GameManager.Instance.Player.Status.Defense = GameManager.Instance.Player.StatusData.Defense;
                    break;
                default:
                    break;
            }
            list.Values[index] = data;
        }
        GameManager.Instance.ItemDataList.DesrializeDictionary();
    }
    private void AllDelete()
    {
        _itemIDList.Clear();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == null) continue;
            Destroy(objects[i]);
            objects[i] = null;
        }

        DeleteRightButton();
        DeleteLeftButton();

        if (_attackUpEffect != null)
        {
            Destroy(_attackUpEffect);
            _attackUpEffect = null;
        }
        if (_defenseUpEffect != null)
        {
            Destroy(_defenseUpEffect);
            _defenseUpEffect = null;
        }
        if (_hpUpEffect != null)
        {
            Destroy(_hpUpEffect);
            _hpUpEffect = null;
        }
    }
    private void SetItemIDList()
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
    private void SetIcon(position _position)
    {
        RectTransform rect;
        Image image;
        ItemData itemData;
        Text text;
        int index = 0;
        GameObject icon = null;
        switch (_position)
        {
            case position.rightend:
                if (objects[(int)position.rightend] == null)
                {
                    objects[(int)position.rightend] = Instantiate(_itemViewPrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
                }
                index = GetIndex(-2);
                icon = objects[(int)position.rightend];
                rect = icon.GetComponent<RectTransform>();
                rect.localScale = scale[(int)position.rightend];
                rect.anchoredPosition = pos[(int)position.rightend];

                break;
            case position.right:
                if (objects[(int)position.right] == null)
                {
                    objects[(int)position.right] = Instantiate(_itemViewPrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
                }
                index = GetIndex(-1);
                icon = objects[(int)position.right];
                rect = icon.GetComponent<RectTransform>();
                rect.localScale = scale[(int)position.right];
                rect.anchoredPosition = pos[(int)position.right];

                break;
            case position.center:
                if (objects[(int)position.center] == null)
                {
                    objects[(int)position.center] = Instantiate(_itemViewPrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
                }
                index = GetIndex(0);
                icon = objects[(int)position.center];
                rect = icon.GetComponent<RectTransform>();
                rect.localScale = scale[(int)position.center];
                rect.anchoredPosition = pos[(int)position.center];

                break;
            case position.left:
                if (objects[(int)position.left] == null)
                {
                    objects[(int)position.left] = Instantiate(_itemViewPrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
                }
                index = GetIndex(1);
                icon = objects[(int)position.left];
                rect = icon.GetComponent<RectTransform>();
                rect.localScale = scale[(int)position.left];
                rect.anchoredPosition = pos[(int)position.left];

                break;
            case position.leftend:
                if (objects[(int)position.leftend] == null)
                {
                    objects[(int)position.leftend] = Instantiate(_itemViewPrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
                }
                index = GetIndex(2);
                icon = objects[(int)position.leftend];
                rect = icon.GetComponent<RectTransform>();
                rect.localScale = scale[(int)position.leftend];
                rect.anchoredPosition = pos[(int)position.leftend];

                break;
            default:
                break;
        }
        //画像設定ーーーーーーーーーーーーーーーーーーーーーーーーー
        image = icon.GetComponentsInChildren<Image>()[1];
        if (index < 0 || !GameManager.Instance.ItemDataList.Dictionary.ContainsKey(_itemIDList[index]))
        {
            image.sprite = Resources.Load<Sprite>("Icon/alpha");
            return;
        }
        itemData = GameManager.Instance.ItemDataList.Dictionary[_itemIDList[index]];
        image.sprite = Resources.Load<Sprite>(itemData.baseData.IconName);
        //文字設定ーーーーーーーーーーーーーーーーーーーーーーーーー
        text = icon.GetComponentInChildren<Text>();
        text.text = itemData.baseData.PoachHoldNumber.ToString();

    }
    private void DeleteIcon(position _position)
    {
        switch (_position)
        {
            case position.rightend:
                if (objects[(int)position.rightend] != null)
                {
                    Destroy(objects[(int)position.rightend]);
                    objects[(int)position.rightend] = null;
                }
                break;
            case position.right:
                if (objects[(int)position.right] != null)
                {
                    Destroy(objects[(int)position.right]);
                    objects[(int)position.right] = null;
                }
                break;
            case position.center:
                if (objects[(int)position.center] != null)
                {
                    Destroy(objects[(int)position.center]);
                    objects[(int)position.center] = null;
                }
                break;
            case position.left:
                if (objects[(int)position.left] != null)
                {
                    Destroy(objects[(int)position.left]);
                    objects[(int)position.left] = null;
                }
                break;
            case position.leftend:
                if (objects[(int)position.leftend] != null)
                {
                    Destroy(objects[(int)position.leftend]);
                    objects[(int)position.leftend] = null;
                }
                break;
            default:
                break;
        }
    }
    private void SetRightButton()
    {
        if (_rightButton != null) return;
        _rightButton = Instantiate(_rightButtonPrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
        RectTransform rect = _rightButton.GetComponent<RectTransform>();
        var tmp = pos[(int)position.center];
        tmp.x += _iconSize.x / 2;
        tmp.y -= _iconSize.y / 2;
        rect.anchoredPosition = tmp;

    }
    private void DeleteRightButton()
    {
        if (_rightButton == null) return;
        Destroy(_rightButton);
        _rightButton = null;
    }
    private void SetLeftButton()
    {
        if (_leftButton != null) return;
        _leftButton = Instantiate(_leftButtonPrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
        RectTransform rect = _leftButton.GetComponent<RectTransform>();
        var tmp = pos[(int)position.center];
        tmp.x -= _iconSize.x / 2;
        tmp.y -= _iconSize.y / 2;
        rect.anchoredPosition = tmp;
    }
    private void DeleteLeftButton()
    {
        if (_leftButton == null) return;
        Destroy(_leftButton);
        _leftButton = null;
    }

    private int GetIndex(int _next)
    {
        if (_itemIDList.Count <= 0)
        {
            Debug.Log("ない");
            return -1;
        }
        int index = _itemIDList.IndexOf(_currentID);
        index += _next;
        //リストのサイズ以上ある場合
        if (index >= _itemIDList.Count)
        {
            index = index % _itemIDList.Count;
        }
        else if (index < 0)
        {
            index = -index % _itemIDList.Count;
            if (index != 0)
            {
                index = _itemIDList.Count - index;
            }
        }
        return index;
    }
    private void CheckID()
    {
        //所持数が0以下ならリストから削除
        for (int i = 0; i < _itemIDList.Count; i++)
        {
            var data = GameManager.Instance.ItemDataList.Dictionary[_itemIDList[i]];
            if (data.baseData.PoachHoldNumber > 0) continue;
            _itemIDList.Remove(data.baseData.ID);
        }
        if (!_itemIDList.Contains(_currentID))
        {
            if (_itemIDList.Count > 0)
            {
                _currentID = _itemIDList[0];
            }
            else
            {
                _currentID = "";
            }
        }
    }
    public void ChangeNotQuestState()
    {
        ChangeState<Village>();
    }
}
