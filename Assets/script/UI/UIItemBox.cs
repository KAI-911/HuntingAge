using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemBox : UIBase
{
    [SerializeField] ItemListObject _dataList;
    [SerializeField] TargetChecker _targetChecker;
    CurrentUI current;

    private void Start()
    {
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.BoxItemSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IB_ItemSelect"]);
        ItemIconList[(int)IconType.PoachItemSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]);
        ItemIconList[(int)IconType.SubMenuSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IB_SubMenu"]);

        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }


    private class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UIManager.Instance._player.IsAction = true;
        }
        public override void OnUpdate(UIBase owner)
        {
        }
        public override void OnProceed(UIBase owner)
        {
            if (!UIManager.Instance._player.IsAction) return;
            if (owner.gameObject.GetComponent<UIItemBox>()._targetChecker.TriggerHit)
            {
                UIManager.Instance._player.IsAction = false;
                owner.ChangeState<FirstSlect>();
            }
        }
    }
    /// <summary>
    /// アイテムの整理か武器の整理か
    /// </summary>
    private class FirstSlect : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();
            var button0 = list[0].GetComponent<Button>();
            button0.onClick.AddListener(() => owner.ChangeState<ItemSlect>());
            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "アイテム選択";
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
            owner.ChangeState<Close>();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            Debug.Log("UIStateBase_FirstSlect");
        }
    }
    private class ItemSlect : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            if (prevState.GetType() == typeof(FirstSlect))
            {
                owner.GetComponent<UIItemBox>().current = CurrentUI.box;
                owner.ItemIconList[(int)IconType.BoxItemSelect].CreateButton();
                owner.ItemIconList[(int)IconType.PoachItemSelect].CreateButton();
                owner.GetComponent<UIItemBox>().UISet();
            }

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            if (nextState.GetType() == typeof(FirstSlect))
            {
                owner.ItemIconList[(int)IconType.BoxItemSelect].DeleteButton();
                owner.ItemIconList[(int)IconType.PoachItemSelect].DeleteButton();
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("select");
            //どっちのリストを見ているか
            if (UIManager.Instance.InputCurrentChange.ReadValue<Vector2>().sqrMagnitude > 0)
            {
                if (UIManager.Instance.InputCurrentChange.ReadValue<Vector2>().x > 0) owner.GetComponent<UIItemBox>().current = CurrentUI.box;
                else owner.GetComponent<UIItemBox>().current = CurrentUI.poach;
            }
            switch (owner.GetComponent<UIItemBox>().current)
            {
                case CurrentUI.box:
                    owner.ItemIconList[(int)IconType.BoxItemSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
                    break;
                case CurrentUI.poach:
                    owner.ItemIconList[(int)IconType.PoachItemSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
                    break;
            }



        }
        public override void OnProceed(UIBase owner)
        {
            //アイテムの入れ替えpoach(box)からbox(poach)へ最大量送る
            if (owner.GetComponent<UIItemBox>().current == CurrentUI.box)
            {
                if (!owner.ItemIconList[(int)IconType.BoxItemSelect].CheckCurrentNunberItem()) return;
                var list = owner.ItemIconList[(int)IconType.BoxItemSelect];
                var data = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
                int needNumber = data.PoachStackNumber - data.PoachHoldNumber;
                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.PoachItemSelect].FirstNotSetNumber();
                if (UINumber != -1) GameManager.Instance.ItemDataList.BoxToPoach(data.ID, needNumber, UINumber);

                owner.GetComponent<UIItemBox>().UISet();
            }
            else
            {
                if (!owner.ItemIconList[(int)IconType.PoachItemSelect].CheckCurrentNunberItem()) return;
                var list = owner.ItemIconList[(int)IconType.PoachItemSelect];
                var data = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
                int needNumber = data.BoxStackNumber - data.BoxHoldNumber;
                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.BoxItemSelect].FirstNotSetNumber();
                if (UINumber != -1) GameManager.Instance.ItemDataList.PoachToBox(data.ID, needNumber, UINumber);

                owner.GetComponent<UIItemBox>().UISet();

            }
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<FirstSlect>();
        }
        public override void OnSubMenu(UIBase owner)
        {
            switch (owner.GetComponent<UIItemBox>().current)
            {
                case CurrentUI.box:
                    var boxlist = owner.GetComponent<UIItemBox>().ItemIconList[(int)IconType.BoxItemSelect];
                    if (boxlist.Buttons[boxlist.CurrentNunber].GetComponent<ItemButton>().ID == "") return;
                    break;
                case CurrentUI.poach:
                    var poachlist = owner.GetComponent<UIItemBox>().ItemIconList[(int)IconType.PoachItemSelect];
                    if (poachlist.Buttons[poachlist.CurrentNunber].GetComponent<ItemButton>().ID == "") return;
                    break;
                default:
                    return;
            }
            owner.ChangeState<SubMune>();
        }

    }

    private class SubMune : UIStateBase
    {
        ItemIcon _itemIcon = new ItemIcon();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            _itemIcon = owner.ItemIconList[(int)IconType.SubMenuSelect];
            var muneData = _itemIcon.IconData;
            Vector2 buttonSize = new Vector2();
            ItemIconData data = new ItemIconData();

            switch (owner.GetComponent<UIItemBox>().current)
            {
                case CurrentUI.box:
                    buttonSize = owner.ItemIconList[(int)IconType.BoxItemSelect].IconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;
                    data = owner.ItemIconList[(int)IconType.BoxItemSelect].IconData;
                    break;
                case CurrentUI.poach:
                    buttonSize = owner.ItemIconList[(int)IconType.PoachItemSelect].IconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;
                    data = owner.ItemIconList[(int)IconType.PoachItemSelect].IconData;
                    break;
            }
            var num = owner.ItemIconList[(int)IconType.BoxItemSelect].CurrentNunber;
            int w = Mathf.Abs((num % (int)data._tableSize.y) * (int)(buttonSize.x + data._padding)) + (int)(buttonSize.x + data._padding);
            int h = Mathf.Abs((num / (int)data._tableSize.y) * (int)(buttonSize.y + data._padding));
            muneData._leftTopPos = new Vector2(data._leftTopPos.x + w, data._leftTopPos.y - h);
            _itemIcon.SetIcondata(muneData);
            var buttonList = _itemIcon.CreateButton();

            var button0 = buttonList[0].GetComponent<Button>();
            button0.onClick.AddListener(() => owner.ChangeState<NumberSelection>());
            var button0Text = buttonList[0].GetComponentInChildren<Text>();
            button0Text.text = "個数を指定して送る";

            var button1 = buttonList[1].GetComponent<Button>();
            button1.onClick.AddListener(() => owner.ChangeState<UIChange>());
            var button1Text = buttonList[1].GetComponentInChildren<Text>();
            button1Text.text = "入れ替え";

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            _itemIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            _itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<ItemSlect>();
        }
        public override void OnProceed(UIBase owner)
        {
            _itemIcon.Buttons[_itemIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
    }

    private class UIChange : UIStateBase
    {
        private int _selectionNumber;
        private ItemIcon _itemIcon;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("UI変更ステートになた");
            switch (owner.GetComponent<UIItemBox>().current)
            {
                case CurrentUI.box:
                    _itemIcon = owner.GetComponent<UIItemBox>().ItemIconList[(int)IconType.BoxItemSelect];
                    _selectionNumber = _itemIcon.CurrentNunber;
                    break;
                case CurrentUI.poach:
                    _itemIcon = owner.GetComponent<UIItemBox>().ItemIconList[(int)IconType.PoachItemSelect];
                    _selectionNumber = _itemIcon.CurrentNunber;
                    break;
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("UI変更ステート");
            _itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());

        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<ItemSlect>();
        }
        public override void OnProceed(UIBase owner)
        {
            //お互いのUI座標を入れ替える
            var selectButton = _itemIcon.Buttons[_selectionNumber].GetComponent<ItemButton>();
            var currentButton = _itemIcon.Buttons[_itemIcon.CurrentNunber].GetComponent<ItemButton>();
            var itemDataList = GameManager.Instance.ItemDataList;
            ItemData data = new ItemData();
            if (selectButton.ID != "")
            {
                Debug.Log("S0");
                if (itemDataList.Keys.Contains(selectButton.ID))
                {
                    Debug.Log("S1");
                    int index = itemDataList.Keys.IndexOf(selectButton.ID);
                    data = itemDataList.Values[index];
                    switch (owner.GetComponent<UIItemBox>().current)
                    {
                        case CurrentUI.box:
                            data.BoxUINumber = _itemIcon.CurrentNunber;
                            Debug.Log("S2_box");
                            break;
                        case CurrentUI.poach:
                            data.PoachUINumber = _itemIcon.CurrentNunber;
                            Debug.Log("S2_poach");
                            break;
                        default:
                            break;
                    }
                    itemDataList.Values[index] = data;
                }
            }
            if (currentButton.ID != "")
            {
                Debug.Log("C0");
                if (itemDataList.Keys.Contains(currentButton.ID))
                {
                    Debug.Log("C1");
                    int index = itemDataList.Keys.IndexOf(currentButton.ID);
                    data = itemDataList.Values[index];
                    switch (owner.GetComponent<UIItemBox>().current)
                    {
                        case CurrentUI.box:
                            data.BoxUINumber = _selectionNumber;
                            Debug.Log("C2_box");
                            break;
                        case CurrentUI.poach:
                            data.PoachUINumber = _selectionNumber;
                            Debug.Log("C2_poach");
                            break;
                        default:
                            break;
                    }
                    itemDataList.Values[index] = data;
                }
            }

            itemDataList.DesrializeDictionary();
            owner.GetComponent<UIItemBox>().UISet();
            owner.ChangeState<ItemSlect>();

        }
    }

    private class NumberSelection : UIStateBase
    {
        private GameObject count;
        private int min, max, now;
        private string ID;
        private bool lockflg;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            if (prevState.GetType() == typeof(SubMune))
            {
                lockflg = false;
                count = Instantiate(Resources.Load("UI/Count"), Vector3.zero, Quaternion.identity) as GameObject;
                var rect = count.GetComponent<RectTransform>();
                Vector2 buttonSize = new Vector2();
                ItemIconData data = new ItemIconData();
                switch (owner.GetComponent<UIItemBox>().current)
                {
                    case CurrentUI.box:
                        buttonSize = owner.ItemIconList[(int)IconType.BoxItemSelect].IconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;
                        data = owner.ItemIconList[(int)IconType.BoxItemSelect].IconData;
                        break;
                    case CurrentUI.poach:
                        buttonSize = owner.ItemIconList[(int)IconType.PoachItemSelect].IconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;
                        data = owner.ItemIconList[(int)IconType.PoachItemSelect].IconData;
                        break;
                }
                var num = owner.ItemIconList[(int)IconType.BoxItemSelect].CurrentNunber;
                int w = Mathf.Abs((num % (int)data._tableSize.y) * (int)(buttonSize.x + data._padding)) + (int)(buttonSize.x + data._padding);
                int h = Mathf.Abs((num / (int)data._tableSize.y) * (int)(buttonSize.y + data._padding));
                var pos = new Vector2(data._leftTopPos.x + w, data._leftTopPos.y - h);
                rect.anchoredPosition = pos;
                count.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);

                now = 1;
                min = 1;
                switch (owner.GetComponent<UIItemBox>().current)
                {
                    case CurrentUI.box:
                        var list = owner.ItemIconList[(int)IconType.BoxItemSelect];
                        ID = list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID;
                        var data1 = GameManager.Instance.ItemDataList.Dictionary[ID];
                        max = data1.BoxHoldNumber;
                        if (max > data1.PoachStackNumber - data1.PoachHoldNumber)
                        {
                            max = data1.PoachStackNumber - data1.PoachHoldNumber;
                        }
                        break;
                    case CurrentUI.poach:
                        var list2 = owner.ItemIconList[(int)IconType.PoachItemSelect];
                        ID = list2.Buttons[list2.CurrentNunber].GetComponent<ItemButton>().ID;
                        var data2 = GameManager.Instance.ItemDataList.Dictionary[ID];
                        max = data2.PoachHoldNumber;
                        if (max > data2.BoxStackNumber - data2.BoxHoldNumber)
                        {
                            max = data2.BoxStackNumber - data2.BoxHoldNumber;
                        }
                        break;
                }
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            Destroy(count);
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
        public override void OnProceed(UIBase owner)
        {
            //アイテムの入れ替えpoach(box)からbox(poach)へ最大量送る
            if (owner.GetComponent<UIItemBox>().current == CurrentUI.box)
            {
                if (!owner.ItemIconList[(int)IconType.BoxItemSelect].CheckCurrentNunberItem()) return;
                var list = owner.ItemIconList[(int)IconType.BoxItemSelect];
                var data = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.PoachItemSelect].FirstNotSetNumber();
                GameManager.Instance.ItemDataList.BoxToPoach(data.ID, now, UINumber);

                owner.GetComponent<UIItemBox>().UISet();
            }
            else
            {
                if (!owner.ItemIconList[(int)IconType.PoachItemSelect].CheckCurrentNunberItem()) return;
                var list = owner.ItemIconList[(int)IconType.PoachItemSelect];
                var data = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.BoxItemSelect].FirstNotSetNumber();
                GameManager.Instance.ItemDataList.PoachToBox(data.ID, now, UINumber);

                owner.GetComponent<UIItemBox>().UISet();

            }
            owner.ChangeState<ItemSlect>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<ItemSlect>();
        }
    }

    enum IconType
    {
        TypeSelect,
        BoxItemSelect,
        PoachItemSelect,
        SubMenuSelect
    }
    public void UISet()
    {
        var boxList = ItemIconList[(int)IconType.BoxItemSelect].Buttons;
        //ボックスリストのUIセット
        foreach (var item in boxList)
        {
            var ibutton = item.GetComponent<ItemButton>();
            ibutton.clear();
        }

        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
        {
            if (item.Value.BoxHoldNumber == 0) continue;
            var ibutton = boxList[item.Value.BoxUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.box);
        }

        var poachList = ItemIconList[(int)IconType.PoachItemSelect].Buttons;
        //ポーチリストのUIセット
        foreach (var item in poachList)
        {
            var ibutton = item.GetComponent<ItemButton>();
            ibutton.clear();
        }

        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
        {
            if (item.Value.PoachHoldNumber == 0) continue;
            var ibutton = poachList[item.Value.PoachUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
        }

    }

}
enum CurrentUI
{
    box,
    poach
}
