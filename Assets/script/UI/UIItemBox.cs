using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemBox : UIBase
{
    [SerializeField] ItemListObject _dataList;
    [SerializeField] TargetChecker _targetChecker;

    private void Start()
    {
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IB_TypeSelect"]);
        ItemIconList[(int)IconType.BoxItemSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IB_ItemSelect"]);
        ItemIconList[(int)IconType.PoachItemSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]);
        //ItemIconList[(int)IconType.SubMenuSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IB_SubMenu"]);
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
        CurrentUI current;
        Level _level;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            current = CurrentUI.box;
            _level = Level.Search;

            var boxList = owner.ItemIconList[(int)IconType.BoxItemSelect].CreateButton();
            ////ボックスリストのUIセット
            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.BoxHoldNumber == 0) continue;
                var ibutton = boxList[item.Value.BoxUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Key, ItemBoxOrPoach.box);
            }


            var poachList = owner.ItemIconList[(int)IconType.PoachItemSelect].CreateButton();

            ////ポーチリストのUIセット
            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.PoachHoldNumber == 0) continue;
                var ibutton = poachList[item.Value.PoachUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
            }


        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.BoxItemSelect].DeleteButton();
            owner.ItemIconList[(int)IconType.PoachItemSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            //どっちのリストを見ているか
            if (UIManager.Instance.InputCurrentChange.ReadValue<Vector2>().sqrMagnitude > 0)
            {
                if (UIManager.Instance.InputCurrentChange.ReadValue<Vector2>().x > 0)current = CurrentUI.box;
                else current = CurrentUI.poach;
            }

            switch (current)
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
            switch (_level)
            {
                case Level.Search:
                    //アイテムの入れ替えpoach(box)からbox(poach)へ最大量送る
                    if (current == CurrentUI.box)
                    {
                        if (!owner.ItemIconList[(int)IconType.BoxItemSelect].CheckCurrentNunberItem()) return;
                        var list = owner.ItemIconList[(int)IconType.BoxItemSelect];
                        var data = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
                        int needNumber = data.PoachStackNumber - data.PoachHoldNumber;

                        int eraseNumber = needNumber;
                        if(needNumber>data.BoxHoldNumber) eraseNumber = data.BoxHoldNumber;
                        int index = GameManager.Instance.ItemDataList.Keys.IndexOf(data.ID);
                        var date = GameManager.Instance.ItemDataList.Values[index];
                        //UIの位置を設定
                        if(data.PoachHoldNumber<=0)
                        {
                            data.PoachUINumber = owner.ItemIconList[(int)IconType.PoachItemSelect].FirstNotSetNumber();
                        }
                        data.BoxHoldNumber -= eraseNumber;
                        data.PoachHoldNumber += eraseNumber;
                        GameManager.Instance.ItemDataList.Values[index] = data;
                        GameManager.Instance.ItemDataList.DesrializeDictionary();

                        var boxList = owner.ItemIconList[(int)IconType.BoxItemSelect].Buttons;
                        ////ボックスリストのUIセット
                        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
                        {
                            if (item.Value.BoxHoldNumber == 0) continue;
                            var ibutton = boxList[item.Value.BoxUINumber].GetComponent<ItemButton>();
                            ibutton.SetID(item.Key, ItemBoxOrPoach.box);
                        }


                        var poachList = owner.ItemIconList[(int)IconType.PoachItemSelect].Buttons;

                        ////ポーチリストのUIセット
                        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
                        {
                            if (item.Value.PoachHoldNumber == 0) continue;
                            var ibutton = poachList[item.Value.PoachUINumber].GetComponent<ItemButton>();
                            ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
                        }
                    }
                    else
                    {
                        if (!owner.ItemIconList[(int)IconType.PoachItemSelect].CheckCurrentNunberItem()) return;
                        var list = owner.ItemIconList[(int)IconType.PoachItemSelect];
                        var data = GameManager.Instance.ItemDataList.Dictionary[list.Buttons[list.CurrentNunber].GetComponent<ItemButton>().ID];
                        int needNumber = data.BoxStackNumber - data.PoachHoldNumber;
                        int eraseNumber = needNumber;
                        if (needNumber > data.PoachHoldNumber) eraseNumber = data.PoachHoldNumber;
                        int index = GameManager.Instance.ItemDataList.Keys.IndexOf(data.ID);
                        var date = GameManager.Instance.ItemDataList.Values[index];
                        //UIの位置を設定
                        if (data.BoxHoldNumber <= 0)
                        {
                            data.BoxUINumber = owner.ItemIconList[(int)IconType.BoxItemSelect].FirstNotSetNumber();
                        }
                        data.PoachHoldNumber -= eraseNumber;
                        data.BoxHoldNumber += eraseNumber;
                        GameManager.Instance.ItemDataList.Values[index] = data;
                        GameManager.Instance.ItemDataList.DesrializeDictionary();

                        var boxList = owner.ItemIconList[(int)IconType.BoxItemSelect].Buttons;
                        ////ボックスリストのUIセット
                        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
                        {
                            if (item.Value.BoxHoldNumber == 0) continue;
                            var ibutton = boxList[item.Value.BoxUINumber].GetComponent<ItemButton>();
                            ibutton.SetID(item.Key, ItemBoxOrPoach.box);
                        }


                        var poachList = owner.ItemIconList[(int)IconType.PoachItemSelect].Buttons;

                        ////ポーチリストのUIセット
                        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
                        {
                            if (item.Value.PoachHoldNumber == 0) continue;
                            var ibutton = poachList[item.Value.PoachUINumber].GetComponent<ItemButton>();
                            ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
                        }








                        ////ボックスリストのUIセット
                        //var boxList = owner.ItemIconList[(int)IconType.BoxItemSelect].CreateButton();
                        //foreach (var item in owner.GetComponent<UIItemBox>()._itemBoxholder.Dictionary)
                        //{
                        //    var ibutton = boxList[item.Value].GetComponent<ItemButton>();
                        //    ibutton.SetID(item.Key, ItemBoxOrPoach.box);
                        //}
                        ////ポーチリストのUIセット
                        //var poachList = owner.ItemIconList[(int)IconType.PoachItemSelect].CreateButton();
                        //foreach (var item in owner.GetComponent<UIItemBox>()._itemPoachholder.Dictionary)
                        //{
                        //    var ibutton = poachList[item.Value].GetComponent<ItemButton>();
                        //    ibutton.SetID(item.Key, ItemBoxOrPoach.box);
                        //}

                    }
                    break;
                case Level.Select:
                    break;
                case Level.SubMenu:
                    break;
                default:
                    break;
            }
        }
        public override void OnBack(UIBase owner)
        {
            switch (_level)
            {
                case Level.Search:
                    owner.ChangeState<FirstSlect>();
                    break;
                case Level.Select:
                    break;
                case Level.SubMenu:
                    break;
                default:
                    break;
            }
        }
        public override void OnSubMenu(UIBase owner)
        {
            switch (_level)
            {
                case Level.Search:
                    _level = Level.SubMenu;
                    //サブメニューを表示する場所を決める
                    var list = owner.GetComponent<UIItemBox>().ItemIconList[(int)IconType.SubMenuSelect];
                    switch (current)
                    {
                        case CurrentUI.box:
                            //list.GetComponent<ItemIcon>().
                            break;
                        case CurrentUI.poach:
                            break;
                        default:
                            break;
                    }
                    break;
                case Level.Select:
                    break;
                case Level.SubMenu:
                    break;
                default:
                    break;
            }

        }
        enum CurrentUI
        {
            box,
            poach
        }
        enum Level
        {
            Search,
            Select,
            SubMenu
        }

    }
    enum IconType
    {
        TypeSelect,
        BoxItemSelect,
        PoachItemSelect,
        SubMenuSelect
    }
}
