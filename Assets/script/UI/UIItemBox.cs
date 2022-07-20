using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemBox : UIBase
{
    [SerializeField] TargetChecker _targetChecker;
    CurrentUI current;

    private void Start()
    {
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.BoxItemSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IB_ItemSelect"]);
        ItemIconList[(int)IconType.PoachItemSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]);
        ItemIconList[(int)IconType.SubMenuSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IB_SubMenu"]);
        ItemIconList[(int)IconType.WeaponSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IB_ItemSelect"]);

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
        ItemIcon _itemIcon = new ItemIcon();

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            _itemIcon = owner.ItemIconList[(int)IconType.TypeSelect];
            _itemIcon.SetTable(new Vector2(2, 1));
            var list = _itemIcon.CreateButton();
            _itemIcon.SetButtonOnClick(0, () => owner.ChangeState<ItemSlect>());
            _itemIcon.SetButtonText(0, "アイテム選択");
            _itemIcon.SetButtonOnClick(1, () => owner.ChangeState<weaponSelect>());
            _itemIcon.SetButtonText(1, "武器選択");

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            _itemIcon.DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            _itemIcon.CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
        public override void OnUpdate(UIBase owner)
        {
            _itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
    }
    private class ItemSlect : UIStateBase
    {
        private ItemIcon itemIcon_box = new ItemIcon();
        private ItemIcon itemIcon_poach = new ItemIcon();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            itemIcon_box = owner.ItemIconList[(int)IconType.BoxItemSelect];
            itemIcon_poach = owner.ItemIconList[(int)IconType.PoachItemSelect];
            if (prevState.GetType() == typeof(FirstSlect))
            {
                owner.GetComponent<UIItemBox>().current = CurrentUI.box;
                itemIcon_box.CreateButton();
                itemIcon_poach.CreateButton();
                owner.GetComponent<UIItemBox>().UISet();
            }

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            if (nextState.GetType() == typeof(FirstSlect))
            {
                itemIcon_box.DeleteButton();
                itemIcon_poach.DeleteButton();
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("select");
            //どっちのリストを見ているか

            if (UIManager.Instance.InputCurrentChange.ReadValue<Vector2>().sqrMagnitude > 0)
            {
                float value = UIManager.Instance.InputCurrentChange.ReadValue<Vector2>().x;
                if (value > 0) owner.GetComponent<UIItemBox>().current = CurrentUI.box;
                else owner.GetComponent<UIItemBox>().current = CurrentUI.poach;
            }
            switch (owner.GetComponent<UIItemBox>().current)
            {
                case CurrentUI.box:
                    itemIcon_box.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
                    break;
                case CurrentUI.poach:
                    itemIcon_poach.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
                    break;
            }
        }
        public override void OnProceed(UIBase owner)
        {
            //アイテムの入れ替えpoach(box)からbox(poach)へ最大量送る
            if (owner.GetComponent<UIItemBox>().current == CurrentUI.box)
            {
                if (!itemIcon_box.CheckCurrentNunberItem()) return;
                var data = GameManager.Instance.MaterialDataList.Dictionary[itemIcon_box.Buttons[itemIcon_box.CurrentNunber].GetComponent<ItemButton>().ID];
                int needNumber = data.PoachStackNumber - data.PoachHoldNumber;
                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.PoachItemSelect].FirstNotSetNumber();
                if (UINumber != -1) GameManager.Instance.MaterialDataList.BoxToPoach(data.ID, needNumber, UINumber);

                owner.GetComponent<UIItemBox>().UISet();
            }
            else
            {
                if (!itemIcon_poach.CheckCurrentNunberItem()) return;
                var data = GameManager.Instance.MaterialDataList.Dictionary[itemIcon_poach.Buttons[itemIcon_poach.CurrentNunber].GetComponent<ItemButton>().ID];
                int needNumber = data.BoxStackNumber - data.BoxHoldNumber;
                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.BoxItemSelect].FirstNotSetNumber();
                if (UINumber != -1) GameManager.Instance.MaterialDataList.PoachToBox(data.ID, needNumber, UINumber);

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
            switch (owner.GetComponent<UIItemBox>().current)
            {
                case CurrentUI.box:
                    _itemIcon.SetLeftTopPos(owner.ItemIconList[(int)IconType.BoxItemSelect].ImagePos());
                    break;
                case CurrentUI.poach:
                    _itemIcon.SetLeftTopPos(owner.ItemIconList[(int)IconType.PoachItemSelect].ImagePos());
                    break;
            }

            _itemIcon.CreateButton();
            _itemIcon.SetButtonText(0, "個数を指定して送る");
            _itemIcon.SetButtonOnClick(0, () => owner.ChangeState<NumberSelection>());
            _itemIcon.SetButtonText(1, "入れ替え");
            _itemIcon.SetButtonOnClick(1, () => owner.ChangeState<UIChange>());

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
            _itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());

        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<ItemSlect>();
        }
        public override void OnProceed(UIBase owner)
        {
            //お互いのUI座標を入れ替える
            var selectButton = _itemIcon.Buttons[_selectionNumber].GetComponent<ItemButton>();
            var currentButton = _itemIcon.Buttons[_itemIcon.CurrentNunber].GetComponent<ItemButton>();
            var MaterialDataList = GameManager.Instance.MaterialDataList;
            MaterialData data = new MaterialData();
            if (selectButton.ID != "")
            {
                if (MaterialDataList.Keys.Contains(selectButton.ID))
                {
                    int index = MaterialDataList.Keys.IndexOf(selectButton.ID);
                    data = MaterialDataList.Values[index];
                    switch (owner.GetComponent<UIItemBox>().current)
                    {
                        case CurrentUI.box:
                            data.BoxUINumber = _itemIcon.CurrentNunber;
                            break;
                        case CurrentUI.poach:
                            data.PoachUINumber = _itemIcon.CurrentNunber;
                            break;
                        default:
                            break;
                    }
                    MaterialDataList.Values[index] = data;
                }
            }
            if (currentButton.ID != "")
            {
                if (MaterialDataList.Keys.Contains(currentButton.ID))
                {
                    int index = MaterialDataList.Keys.IndexOf(currentButton.ID);
                    data = MaterialDataList.Values[index];
                    switch (owner.GetComponent<UIItemBox>().current)
                    {
                        case CurrentUI.box:
                            data.BoxUINumber = _selectionNumber;
                            break;
                        case CurrentUI.poach:
                            data.PoachUINumber = _selectionNumber;
                            break;
                        default:
                            break;
                    }
                    MaterialDataList.Values[index] = data;
                }
            }

            MaterialDataList.DesrializeDictionary();
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
                count = Instantiate(Resources.Load("UI/Count"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
                switch (owner.GetComponent<UIItemBox>().current)
                {
                    case CurrentUI.box:
                        owner.ItemIconList[(int)IconType.BoxItemSelect].AdjustmentImage(count.GetComponent<RectTransform>());
                        break;
                    case CurrentUI.poach:
                        owner.ItemIconList[(int)IconType.PoachItemSelect].AdjustmentImage(count.GetComponent<RectTransform>());
                        break;
                }


                now = 1;
                min = 1;
                switch (owner.GetComponent<UIItemBox>().current)
                {
                    case CurrentUI.box:
                        var list = owner.ItemIconList[(int)IconType.BoxItemSelect];
                        ID = list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID;
                        var data1 = GameManager.Instance.MaterialDataList.Dictionary[ID];
                        max = data1.BoxHoldNumber;
                        if (max > data1.PoachStackNumber - data1.PoachHoldNumber)
                        {
                            max = data1.PoachStackNumber - data1.PoachHoldNumber;
                        }
                        break;
                    case CurrentUI.poach:
                        var list2 = owner.ItemIconList[(int)IconType.PoachItemSelect];
                        ID = list2.Buttons[list2.CurrentNunber].GetComponent<ItemButton>().ID;
                        var data2 = GameManager.Instance.MaterialDataList.Dictionary[ID];
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
                var data = GameManager.Instance.MaterialDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.PoachItemSelect].FirstNotSetNumber();
                GameManager.Instance.MaterialDataList.BoxToPoach(data.ID, now, UINumber);

                owner.GetComponent<UIItemBox>().UISet();
            }
            else
            {
                if (!owner.ItemIconList[(int)IconType.PoachItemSelect].CheckCurrentNunberItem()) return;
                var list = owner.ItemIconList[(int)IconType.PoachItemSelect];
                var data = GameManager.Instance.MaterialDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.BoxItemSelect].FirstNotSetNumber();
                GameManager.Instance.MaterialDataList.PoachToBox(data.ID, now, UINumber);

                owner.GetComponent<UIItemBox>().UISet();

            }
            owner.ChangeState<ItemSlect>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<ItemSlect>();
        }
    }
    private class weaponSelect : UIStateBase
    {
        ItemIcon itemIcon;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            itemIcon = owner.ItemIconList[(int)IconType.WeaponSelect];
            itemIcon.CreateButton();
            foreach (var item in GameManager.Instance.WeaponDataList.Dictionary)
            {
                if (!item.Value.BoxPossession) continue;
                itemIcon.Buttons[item.Value.BoxUINumber].GetComponent<ItemButton>().SetWeaponID(item.Value.ID);
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            itemIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            if (!itemIcon.CheckCurrentNunberItem()) return;
            var weaponID = itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<ItemButton>().ID;
            UIManager.Instance._player.WeaponID = weaponID;
            owner.GetComponent<UIItemBox>().WeaponUISet();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<FirstSlect>();
        }

    }
    enum IconType
    {
        TypeSelect,
        BoxItemSelect,
        PoachItemSelect,
        SubMenuSelect,
        WeaponSelect
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

        foreach (var item in GameManager.Instance.MaterialDataList.Dictionary)
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

        foreach (var item in GameManager.Instance.MaterialDataList.Dictionary)
        {
            if (item.Value.PoachHoldNumber == 0) continue;
            var ibutton = poachList[item.Value.PoachUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
        }

    }
    public void WeaponUISet()
    {
        var weaponList = ItemIconList[(int)IconType.WeaponSelect].Buttons;
        //ボックスリストのUIセット
        foreach (var item in weaponList)
        {
            var ibutton = item.GetComponent<ItemButton>();
            ibutton.clear();
        }
        foreach (var item in GameManager.Instance.WeaponDataList.Dictionary)
        {
            if (!item.Value.BoxPossession) continue;
            var ibutton = weaponList[item.Value.BoxUINumber].GetComponent<ItemButton>();
            ibutton.SetWeaponID(item.Value.ID);
        }
    }

}
enum CurrentUI
{
    box,
    poach
}
