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
    //強化する武器のID
    [SerializeField] string _cleateItemID;
    //ボックスかポーチどっちに送るか
    [SerializeField] bool _toPouch;
    //何個作るか
    int cleateNum;
    private GameObject countObject;

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
        private int min, max, now;
        private bool lockflg;
        private bool _check;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            if (prevState.GetType() == typeof(CleateItem))
            {
                owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            }
            _check = false;
            Debug.Log(owner.GetComponent<kichen>()._toPouch);
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("作るアイテム");
            //モード選択画面
            var Item = GameManager.Instance.ItemDataList.Dictionary;
            List<ItemData> _createItem = new List<ItemData>();
            foreach (var item in Item)
            {
                if (item.Value.CreatableLevel < GameManager.Instance.VillageData.KitchenLevel)
                {
                    for (int i = 0; i < item.Value.NeedMaterialLst.Count; i++)
                    {
                        if (!GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(item.Value.NeedMaterialLst[i].materialID)) continue;
                    }
                    _createItem.Add(item.Value);
                }
            }
            Debug.Log(_createItem[0].baseData.Name);
            var table = owner.ItemIconList[(int)IconType.TypeSelect];
            var iconData = table.IconData;
            iconData._tableSize = new Vector2(_createItem.Count, 1);
            table.SetIcondata(iconData);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();

            for (int i = 0; i < _createItem.Count; i++)
            {
<<<<<<< HEAD:Assets/Resources/kichen.cs
                int num = i;
                var buttonText = list[num].GetComponentInChildren<Text>();
                buttonText.text = _createItem[num].baseData.Name;
                var button = list[num].GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    Debug.Log("osaretayo");
                    owner.GetComponent<kichen>()._cleateItemID = _createItem[num].baseData.ID; lockflg = false;
                    count = Instantiate(Resources.Load("UI/Count"), Vector3.zero, Quaternion.identity) as GameObject;
                    var rect = count.GetComponent<RectTransform>();
=======
                int numi = i;
                var buttonText = list[numi].GetComponentInChildren<Text>();
                buttonText.text = _createItem[numi].baseData.Name;
                var button = list[numi].GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    _check = true;
                    owner.GetComponent<kichen>()._cleateItemID = _createItem[numi].baseData.ID; lockflg = false;
                    owner.GetComponent<kichen>().countObject = Instantiate(Resources.Load("UI/Count"), Vector3.zero, Quaternion.identity) as GameObject;
                    var rect = owner.GetComponent<kichen>().countObject.GetComponent<RectTransform>();
>>>>>>> cf2e1680e54e9e2490cd8414ed3f94cd8c6d6625:Assets/Scenes/Base/kitchen/kichen.cs
                    Vector2 buttonSize = new Vector2();
                    ItemIconData data = new ItemIconData();

                    buttonSize = owner.ItemIconList[(int)IconType.TypeSelect].IconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;
                    data = owner.ItemIconList[(int)IconType.TypeSelect].IconData;

                    buttonSize = owner.ItemIconList[(int)IconType.TypeSelect].IconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;
                    data = owner.ItemIconList[(int)IconType.TypeSelect].IconData;
                    var num = owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber;
                    int w = Mathf.Abs((num % (int)data._tableSize.y) * (int)(buttonSize.x + data._padding)) + (int)(buttonSize.x + data._padding);
                    int h = Mathf.Abs((num / (int)data._tableSize.y) * (int)(buttonSize.y + data._padding));
                    var pos = new Vector2(data._leftTopPos.x + w, data._leftTopPos.y - h);
                    rect.anchoredPosition = pos;
                    owner.GetComponent<kichen>().countObject.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);

                    now = 0;
                    min = 0;
                    max = 9999;

                    string id = owner.GetComponent<kichen>()._cleateItemID;
                    var _Item = GameManager.Instance.ItemDataList;
                    var _material = GameManager.Instance.MaterialDataList;
                    var counter = _Item.Dictionary[id].NeedMaterialLst;
                    for (int i = 0; i < counter.Count; i++)
                    {
                        string _needMaterialID = _Item.Dictionary[id].NeedMaterialLst[i].materialID;
                        var tmp = counter[i].requiredCount;
                        var materialNum = _material.Dictionary[_needMaterialID].BoxHoldNumber + _material.Dictionary[_needMaterialID].PoachHoldNumber;
                        if ((materialNum / tmp) < max)
                        {
                            max = materialNum / tmp;
                        }
                    }
                    Debug.Log(max);
                });
            }
        }
        public override void OnUpdate(UIBase owner)
        {

            if (!_check)
            {
                owner.ItemIconList[(int)IconType.TypeSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            }
            else
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

                        owner.GetComponent<kichen>().countObject.GetComponentInChildren<Text>().text = now.ToString();
                    }
                }
                else
                {
                    lockflg = false;
                }
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            Destroy(owner.GetComponent<kichen>().countObject);
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();

        }
        public override void OnProceed(UIBase owner)
        {
<<<<<<< HEAD:Assets/Resources/kichen.cs
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
            if (!owner.ItemIconList[(int)IconType.TypeSelect].CheckCurrentNunberItem()) return;
            var list = owner.ItemIconList[(int)IconType.TypeSelect];
            var data = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
            //UIの位置を設定
            int UINumber = owner.ItemIconList[(int)IconType.TypeSelect].FirstNotSetNumber();
            GameManager.Instance.ItemDataList.PoachToBox(data.baseData.ID, now, UINumber);

            owner.GetComponent<UIItemBox>().UISet();

            owner.ChangeState<CleateItem>();
=======
            if (!_check)
            {
                Debug.Log("futuu");
                owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
            }
            else
            {
                owner.GetComponent<kichen>().cleateNum = now;
                owner.ChangeState<CleateItem>();
            }
>>>>>>> cf2e1680e54e9e2490cd8414ed3f94cd8c6d6625:Assets/Scenes/Base/kitchen/kichen.cs
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
        private ItemIcon confimationIcon;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            confimationIcon = owner.ItemIconList[(int)IconType.Confirmation];
            Debug.Log("cleateItemnikitayo");
            ConfirmationSelect = false;
            var icon = confimationIcon.IconData;
            icon._textData.text = "素材を消費してアイテムを作りますか？";
            confimationIcon.SetIcondata(icon);
            var list = confimationIcon.CreateButton();

            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "はい";
            var button0 = list[0].GetComponent<Button>();
            button0.onClick.AddListener(() =>
            {
                GameManager.Instance.WeaponDataList.Enhancement(owner.GetComponent<kichen>()._cleateItemID);
                ConfirmationSelect = true;
                var icon = confimationIcon.IconData;
                icon._textData.text = "調理完了";
                icon._tableSize = new Vector2(1, 1);
                confimationIcon.SetIcondata(icon);
                var list = confimationIcon.CreateButton();
                var buttonText = list[0].GetComponentInChildren<Text>();
                buttonText.text = "OK";
                var button = list[0].GetComponent<Button>();
                button.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
            });

            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "いいえ";
            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
        }
        public override void OnUpdate(UIBase owner)
        {
            confimationIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            Destroy(owner.GetComponent<kichen>().countObject);
            confimationIcon.DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            confimationIcon.Buttons[confimationIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<TypeSelectMode>();
        }
    }


}

