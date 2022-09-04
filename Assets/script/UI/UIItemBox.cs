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
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.BoxItemSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IB_ItemSelect"]);
        ItemIconList[(int)IconType.PoachItemSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]);
        ItemIconList[(int)IconType.SubMenuSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IB_SubMenu"]);
        ItemIconList[(int)IconType.WeaponSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IB_ItemSelect"]);

        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }


    private class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UISoundManager.Instance._player.IsAction = true;
        }
        public override void OnUpdate(UIBase owner)
        {
        }
        public override void OnProceed(UIBase owner)
        {
            if (!UISoundManager.Instance._player.IsAction) return;
            if (owner.gameObject.GetComponent<UIItemBox>()._targetChecker.TriggerHit)
            {
                UISoundManager.Instance.PlayDecisionSE();
                UISoundManager.Instance._player.IsAction = false;
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
            UISoundManager.Instance.PlayDecisionSE();
            _itemIcon.CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
        public override void OnUpdate(UIBase owner)
        {
            _itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
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

            if (UISoundManager.Instance.InputCurrentChange.ReadValue<Vector2>().sqrMagnitude > 0)
            {
                float value = UISoundManager.Instance.InputCurrentChange.ReadValue<Vector2>().x;
                if (value > 0) owner.GetComponent<UIItemBox>().current = CurrentUI.box;
                else owner.GetComponent<UIItemBox>().current = CurrentUI.poach;
            }
            switch (owner.GetComponent<UIItemBox>().current)
            {
                case CurrentUI.box:
                    itemIcon_box.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
                    break;
                case CurrentUI.poach:
                    itemIcon_poach.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
                    break;
            }
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            //アイテムの入れ替えpoach(box)からbox(poach)へ最大量送る
            if (owner.GetComponent<UIItemBox>().current == CurrentUI.box)
            {
                if (!itemIcon_box.CheckCurrentNunberItem()) return;
                if (GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(itemIcon_box.Buttons[itemIcon_box.CurrentNunber].GetComponent<ItemButton>().ID))
                {
                    var data = GameManager.Instance.MaterialDataList.Dictionary[itemIcon_box.Buttons[itemIcon_box.CurrentNunber].GetComponent<ItemButton>().ID];
                    int needNumber = data.PoachStackNumber - data.PoachHoldNumber;
                    //UIの位置を設定
                    int UINumber = owner.ItemIconList[(int)IconType.PoachItemSelect].FirstNotSetNumber();
                    if (UINumber != -1) GameManager.Instance.MaterialDataList.BoxToPoach(data.ID, needNumber, UINumber);
                }
                else if (GameManager.Instance.ItemDataList.Dictionary.ContainsKey(itemIcon_box.Buttons[itemIcon_box.CurrentNunber].GetComponent<ItemButton>().ID))
                {
                    var data = GameManager.Instance.ItemDataList.Dictionary[itemIcon_box.Buttons[itemIcon_box.CurrentNunber].GetComponent<ItemButton>().ID];
                    int needNumber = data.baseData.PoachStackNumber - data.baseData.PoachHoldNumber;
                    //UIの位置を設定
                    int UINumber = owner.ItemIconList[(int)IconType.PoachItemSelect].FirstNotSetNumber();
                    if (UINumber != -1) GameManager.Instance.ItemDataList.BoxToPoach(data.baseData.ID, needNumber, UINumber);

                }
                owner.GetComponent<UIItemBox>().UISet();
            }
            else
            {
                if (!itemIcon_poach.CheckCurrentNunberItem()) return;
                if (GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(itemIcon_poach.Buttons[itemIcon_poach.CurrentNunber].GetComponent<ItemButton>().ID))
                {
                    var data = GameManager.Instance.MaterialDataList.Dictionary[itemIcon_poach.Buttons[itemIcon_poach.CurrentNunber].GetComponent<ItemButton>().ID];
                    int needNumber = data.BoxStackNumber - data.BoxHoldNumber;
                    //UIの位置を設定
                    int UINumber = owner.ItemIconList[(int)IconType.BoxItemSelect].FirstNotSetNumber();
                    if (UINumber != -1) GameManager.Instance.MaterialDataList.PoachToBox(data.ID, needNumber, UINumber);
                }
                else if (GameManager.Instance.ItemDataList.Dictionary.ContainsKey(itemIcon_poach.Buttons[itemIcon_poach.CurrentNunber].GetComponent<ItemButton>().ID))
                {
                    var data = GameManager.Instance.ItemDataList.Dictionary[itemIcon_poach.Buttons[itemIcon_poach.CurrentNunber].GetComponent<ItemButton>().ID];
                    int needNumber = data.baseData.BoxStackNumber - data.baseData.BoxHoldNumber;
                    //UIの位置を設定
                    int UINumber = owner.ItemIconList[(int)IconType.BoxItemSelect].FirstNotSetNumber();
                    if (UINumber != -1) GameManager.Instance.ItemDataList.PoachToBox(data.baseData.ID, needNumber, UINumber);

                }
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
            _itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<ItemSlect>();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
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
            _itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());

        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<ItemSlect>();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            //お互いのUI座標を入れ替える
            var selectButton = _itemIcon.Buttons[_selectionNumber].GetComponent<ItemButton>();
            var currentButton = _itemIcon.Buttons[_itemIcon.CurrentNunber].GetComponent<ItemButton>();
            var MaterialDataList = GameManager.Instance.MaterialDataList;
            var ItemDataList = GameManager.Instance.ItemDataList;
            MaterialData data = new MaterialData();
            if (selectButton.ID != "")
            {
                int index = 0;

                if (MaterialDataList.Dictionary.ContainsKey(selectButton.ID))
                {
                    index = MaterialDataList.Keys.IndexOf(selectButton.ID);
                    data = MaterialDataList.Values[index];
                }
                else if (ItemDataList.Dictionary.ContainsKey(selectButton.ID))
                {
                    index = ItemDataList.Keys.IndexOf(selectButton.ID);
                    data = ItemDataList.Values[index].baseData;
                }

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

                if (MaterialDataList.Dictionary.ContainsKey(selectButton.ID))
                {
                    MaterialDataList.Values[index] = data;
                }
                else if (ItemDataList.Dictionary.ContainsKey(selectButton.ID))
                {
                    var tmp = ItemDataList.Values[index];
                    tmp.baseData = data;
                    ItemDataList.Values[index] = tmp;
                }
            }

            if (currentButton.ID != "")
            {
                int index = 0;

                if (MaterialDataList.Dictionary.ContainsKey(currentButton.ID))
                {
                    index = MaterialDataList.Keys.IndexOf(currentButton.ID);
                    data = MaterialDataList.Values[index];
                }
                else if (ItemDataList.Dictionary.ContainsKey(currentButton.ID))
                {
                    index = ItemDataList.Keys.IndexOf(currentButton.ID);
                    data = ItemDataList.Values[index].baseData;
                }

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

                if (MaterialDataList.Dictionary.ContainsKey(currentButton.ID))
                {
                    MaterialDataList.Values[index] = data;
                }
                else if (ItemDataList.Dictionary.ContainsKey(currentButton.ID))
                {
                    var tmp = ItemDataList.Values[index];
                    tmp.baseData = data;
                    ItemDataList.Values[index] = tmp;
                }
            }

            MaterialDataList.DesrializeDictionary();
            ItemDataList.DesrializeDictionary();
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
                MaterialData materialData = new MaterialData();
                switch (owner.GetComponent<UIItemBox>().current)
                {
                    case CurrentUI.box:
                        var list = owner.ItemIconList[(int)IconType.BoxItemSelect];
                        ID = list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID;
                        if (GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(ID))
                        {
                            materialData = GameManager.Instance.MaterialDataList.Dictionary[ID];
                        }
                        else if (GameManager.Instance.ItemDataList.Dictionary.ContainsKey(ID))
                        {
                            materialData = GameManager.Instance.ItemDataList.Dictionary[ID].baseData;
                        }
                        max = materialData.BoxHoldNumber;
                        if (max > materialData.PoachStackNumber - materialData.PoachHoldNumber)
                        {
                            max = materialData.PoachStackNumber - materialData.PoachHoldNumber;
                        }
                        break;
                    case CurrentUI.poach:
                        var list2 = owner.ItemIconList[(int)IconType.PoachItemSelect];
                        ID = list2.Buttons[list2.CurrentNunber].GetComponent<ItemButton>().ID;
                        if (GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(ID))
                        {
                            materialData = GameManager.Instance.MaterialDataList.Dictionary[ID];
                        }
                        else if (GameManager.Instance.ItemDataList.Dictionary.ContainsKey(ID))
                        {
                            materialData = GameManager.Instance.ItemDataList.Dictionary[ID].baseData;
                        }
                        max = materialData.PoachHoldNumber;
                        if (max > materialData.BoxStackNumber - materialData.BoxHoldNumber)
                        {
                            max = materialData.BoxStackNumber - materialData.BoxHoldNumber;
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
            var vec = UISoundManager.Instance.InputSelection.ReadValue<Vector2>();
            if (vec.sqrMagnitude > 0)
            {
                if (lockflg == false)
                {
                    UISoundManager.Instance.PlayCursorSE();
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
            UISoundManager.Instance.PlayDecisionSE();
            MaterialData materialData = new MaterialData();
            //アイテムの入れ替えpoach(box)からbox(poach)へ最大量送る
            if (owner.GetComponent<UIItemBox>().current == CurrentUI.box)
            {
                if (!owner.ItemIconList[(int)IconType.BoxItemSelect].CheckCurrentNunberItem()) return;
                var list = owner.ItemIconList[(int)IconType.BoxItemSelect];

                if (GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(ID))
                {
                    materialData = GameManager.Instance.MaterialDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];

                }
                else if (GameManager.Instance.ItemDataList.Dictionary.ContainsKey(ID))
                {
                    materialData = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID].baseData;

                }

                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.PoachItemSelect].FirstNotSetNumber();
                if (GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(ID))
                {
                    GameManager.Instance.MaterialDataList.BoxToPoach(materialData.ID, now, UINumber);
                }
                else if (GameManager.Instance.ItemDataList.Dictionary.ContainsKey(ID))
                {
                    GameManager.Instance.ItemDataList.BoxToPoach(materialData.ID, now, UINumber);
                }
                owner.GetComponent<UIItemBox>().UISet();
            }
            else
            {
                if (!owner.ItemIconList[(int)IconType.PoachItemSelect].CheckCurrentNunberItem()) return;
                var list = owner.ItemIconList[(int)IconType.PoachItemSelect];

                if (GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(ID))
                {
                    materialData = GameManager.Instance.MaterialDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];

                }
                else if (GameManager.Instance.ItemDataList.Dictionary.ContainsKey(ID))
                {
                    materialData = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID].baseData;

                }
                //UIの位置を設定
                int UINumber = owner.ItemIconList[(int)IconType.BoxItemSelect].FirstNotSetNumber();
                if (GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(ID))
                {
                    GameManager.Instance.MaterialDataList.PoachToBox(materialData.ID, now, UINumber);
                }
                else if (GameManager.Instance.ItemDataList.Dictionary.ContainsKey(ID))
                {
                    GameManager.Instance.ItemDataList.PoachToBox(materialData.ID, now, UINumber);
                }
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
            itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            if (!itemIcon.CheckCurrentNunberItem()) return;
            UISoundManager.Instance.PlayDecisionSE();
            var weaponID = itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<ItemButton>().ID;
            UISoundManager.Instance._player.WeaponID = weaponID;
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
        //一度リセット
        foreach (var item in boxList)
        {
            var ibutton = item.GetComponent<ItemButton>();
            ibutton.clear();
        }
        //ボックスリストのUIセット
        foreach (var item in GameManager.Instance.MaterialDataList.Dictionary)
        {
            if (item.Value.BoxHoldNumber == 0) continue;
            var ibutton = boxList[item.Value.BoxUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.box);
        }
        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
        {
            if (item.Value.baseData.BoxHoldNumber == 0) continue;
            var ibutton = boxList[item.Value.baseData.BoxUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.box);
        }
        var poachList = ItemIconList[(int)IconType.PoachItemSelect].Buttons;
        //一度リセット
        foreach (var item in poachList)
        {
            var ibutton = item.GetComponent<ItemButton>();
            ibutton.clear();
        }
        //ポーチリストのUIセット
        foreach (var item in GameManager.Instance.MaterialDataList.Dictionary)
        {
            if (item.Value.PoachHoldNumber == 0) continue;
            var ibutton = poachList[item.Value.PoachUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
        }
        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
        {
            if (item.Value.baseData.PoachHoldNumber == 0) continue;
            Debug.Log("poachItem");
            var ibutton = poachList[item.Value.baseData.PoachUINumber].GetComponent<ItemButton>();
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
