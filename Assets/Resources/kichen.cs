using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class kichen : UIBase
{
    //プレイヤーが近くまで来たか判断
    [SerializeField] TargetChecker _kichenChecker;
    //確認用
    [SerializeField] Confirmation _confirmation;
    //武器データリスト
    [SerializeField] ItemDataList _itemDataList;
    //強化する武器のID
    [SerializeField] string _cleateItemID;
    //ボックスかポーチどっちに送るか
    [SerializeField] bool _toPouch;

    enum IconType
    {
        TypeSelect,
        Confirmation
    }

    private int productionItem;
    private enum ItemType
    {
        Axe = 1,
        Spear,
        Bow
    }
    void Start()
    {
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }

    [Serializable]
    public class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("Blacksmith_close_OnEnter");
            UIManager.Instance._player.IsAction = true;
        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log("Blacksmith_close_OnProceed");
            //近くに来ている && 決定ボタンを押している && キャンバスがactiveでない
            if (owner.GetComponent<kichen>()._kichenChecker.TriggerHit && UIManager.Instance._player.IsAction)
            {
                owner.ChangeState<BoxorPouch>();
            }
        }
    }

    public class BoxorPouch : UIStateBase
    {
        private bool ConfirmationSelect;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            ConfirmationSelect = true;
            var icon = owner.ItemIconList[(int)IconType.TypeSelect].IconData;
            icon._textData.text = "アイテムをどこに送りますか？";
            owner.ItemIconList[(int)IconType.TypeSelect].SetIcondata(icon);
            var table = owner.ItemIconList[(int)IconType.TypeSelect];
            var iconData = table.IconData;
            iconData._tableSize = new Vector2(2, 1);
            table.SetIcondata(iconData);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();

            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "ボックスへ";
            var button0 = list[0].GetComponent<Button>();
            button0.onClick.AddListener(() => { owner.GetComponent<kichen>()._toPouch = false; owner.ChangeState<TypeSelectMode>(); });


            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "ポーチへ";
            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() => { owner.GetComponent<kichen>()._toPouch = true; owner.ChangeState<TypeSelectMode>(); });
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<Close>();
        }
    }

    public class TypeSelectMode : UIStateBase
    {
        private GameObject count;
        private int min, max, now;
        private bool lockflg;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log(owner.GetComponent<kichen>()._toPouch);
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("作るアイテム");
            Debug.Log("e!");
            //モード選択画面
            var Item = owner.GetComponent<kichen>()._itemDataList.Dictionary;
            Debug.Log("a!");
            List<ItemData> _createItem = new List<ItemData>();
            Debug.Log("o!");
            foreach (var item in Item)
            {
                if (item.Value.CreatableLevel < GameManager.Instance.VillageData.KitchenLevel) _createItem.Add(item.Value);
            }
            Debug.Log(_createItem[0].Name);
            var table = owner.ItemIconList[(int)IconType.TypeSelect];
            var iconData = table.IconData;
            iconData._tableSize = new Vector2(_createItem.Count, 1);
            table.SetIcondata(iconData);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();

            for (int i = 0; i < _createItem.Count; i++)
            {
                int num = i;
                var buttonText = list[num].GetComponentInChildren<Text>();
                buttonText.text = _createItem[num].Name;
                var button = list[num].GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    Debug.Log("osaretayo");
                    owner.GetComponent<kichen>()._cleateItemID = _createItem[num].ID; lockflg = false;
                    count = Instantiate(Resources.Load("UI/Count"), Vector3.zero, Quaternion.identity) as GameObject;
                    var rect = count.GetComponent<RectTransform>();
                    Vector2 buttonSize = new Vector2();
                    ItemIconData data = new ItemIconData();

                    buttonSize = owner.ItemIconList[(int)IconType.TypeSelect].IconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;
                    data = owner.ItemIconList[(int)IconType.TypeSelect].IconData;

                    int w = Mathf.Abs((int)(buttonSize.x + data._padding));
                    var pos = new Vector2(list[num].GetComponent<RectTransform>().anchoredPosition.x + w, list[num].GetComponent<RectTransform>().anchoredPosition.y);
                    rect.anchoredPosition = pos;
                    count.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);

                    now = 1;
                    min = 1;
                    max = 9999;

                    string id = owner.GetComponent<kichen>()._cleateItemID;
                    var _Item = GameManager.Instance.ItemDataList;
                    var _material = GameManager.Instance.MaterialDataList;
                    var counter = _Item.Dictionary[id].NeedMaterialLst;
                    for (int i = 0; i < counter.Count; i++)
                    {
                        var tmp = counter[i].requiredCount;
                        var materialNum = _material.Dictionary[id].BoxHoldNumber + _material.Dictionary[id].PoachHoldNumber;
                        if ((materialNum / tmp) < max)
                        {
                            max = materialNum / tmp;
                        }
                    }
                });
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            var vec = UIManager.Instance.InputSelection.ReadValue<Vector2>();
            if (vec.sqrMagnitude > 0)
            {
                if (lockflg == false)
                {
                    if (vec.y > 0) now++;
                    else now--;
                    now = Mathf.Clamp(now, min, max);
                    lockflg = true;

                    count.GetComponentInChildren<Text>().text = now.ToString();
                }
            }
            else
            {
                lockflg = false;
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
            if (!owner.ItemIconList[(int)IconType.TypeSelect].CheckCurrentNunberItem()) return;
            var list = owner.ItemIconList[(int)IconType.TypeSelect];
            var data = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
            //UIの位置を設定
            int UINumber = owner.ItemIconList[(int)IconType.TypeSelect].FirstNotSetNumber();
            GameManager.Instance.ItemDataList.PoachToBox(data.ID, now, UINumber);

            owner.GetComponent<UIItemBox>().UISet();

            owner.ChangeState<CleateItem>();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<Close>();
        }
    }

    public class CleateItem : UIStateBase
    {
        private bool ConfirmationSelect;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            ConfirmationSelect = true;
            var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
            icon._textData.text = "素材を消費してアイテムを作りますか？";
            owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
            var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();

            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "はい";
            var button0 = list[0].GetComponent<Button>();
            switch (GameManager.Instance.WeaponDataList.Enhancement(owner.GetComponent<kichen>()._cleateItemID))

            {
                case 0:
                    button0.onClick.AddListener(() =>
                    {
                        ConfirmationSelect = true;
                        var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                        icon._textData.text = "すでに所持しています";
                        icon._tableSize = new Vector2(1, 1);
                        owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                        var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                        var buttonText = list[0].GetComponentInChildren<Text>();
                        buttonText.text = "OK";
                        var button = list[0].GetComponent<Button>();
                        button.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
                    });
                    break;
                case 1:
                    button0.onClick.AddListener(() =>
                    {
                        ConfirmationSelect = true;
                        var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                        icon._textData.text = "製造完了";
                        icon._tableSize = new Vector2(1, 1);
                        owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                        var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                        var buttonText = list[0].GetComponentInChildren<Text>();
                        buttonText.text = "OK";
                        var button = list[0].GetComponent<Button>();
                        button.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
                    });
                    break;
                case 2:
                    button0.onClick.AddListener(() =>
                    {
                        ConfirmationSelect = true;
                        var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                        icon._textData.text = "素材が足りません";
                        icon._tableSize = new Vector2(1, 1);
                        owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                        var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                        var buttonText = list[0].GetComponentInChildren<Text>();
                        buttonText.text = "OK";
                        var button = list[0].GetComponent<Button>();
                        button.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
                    });
                    break;
                default:
                    break;
            }

            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "いいえ";
            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
        }
        public override void OnUpdate(UIBase owner)
        {
            if (ConfirmationSelect)
            {
                owner.ItemIconList[(int)IconType.Confirmation].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            }
            else
            {
                owner.ItemIconList[(int)IconType.TypeSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<TypeSelectMode>();
        }
    }


}

