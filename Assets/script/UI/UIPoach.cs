using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoach : UIBase
{
    [SerializeField] ItemListObject dataList;

    private void Start()
    {
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.ItemSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }


    private class Close : UIStateBase
    {

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("UIStateBase_close_OnEnter");
            UIManager.Instance._player.IsAction = true;
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("UIStateBase_close");
        }
        public override void OnMenu(UIBase owner)
        {
            if (!UIManager.Instance._player.IsAction) return;
            UIManager.Instance._player.IsAction = false;
            owner.ChangeState<FirstSlect>();
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
            if (prevState.GetType() == typeof(UIChange)) return;

            var list = owner.ItemIconList[(int)IconType.ItemSelect].CreateButton();

            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.PoachHoldNumber == 0) continue;
                var ibutton = list[item.Value.PoachUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Value.ID, ItemBoxOrPoach.poach);
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            if (nextState.GetType() == typeof(UIChange)) return;

            owner.ItemIconList[(int)IconType.ItemSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ItemSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            Debug.Log("ItemSlect_SecondSelect");
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ChangeState<UIChange>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<FirstSlect>();
        }
    }
    private class UIChange : UIStateBase
    {
        private int _selectionNumber;
        private ItemIcon _itemIcon;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("UI変更ステートになた");
            _itemIcon = owner.GetComponent<UIPoach>().ItemIconList[(int)IconType.ItemSelect];
            _selectionNumber = _itemIcon.CurrentNunber;

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
                if (itemDataList.Keys.Contains(selectButton.ID))
                {
                    int index = itemDataList.Keys.IndexOf(selectButton.ID);
                    data = itemDataList.Values[index];
                    data.PoachUINumber = _itemIcon.CurrentNunber;
                    itemDataList.Values[index] = data;
                }
            }
            if (currentButton.ID != "")
            {
                if (itemDataList.Keys.Contains(currentButton.ID))
                {
                    int index = itemDataList.Keys.IndexOf(currentButton.ID);
                    data = itemDataList.Values[index];
                    data.PoachUINumber = _selectionNumber;
                    itemDataList.Values[index] = data;
                }
            }

            itemDataList.DesrializeDictionary();
            owner.GetComponent<UIPoach>().UISet();
            owner.ChangeState<ItemSlect>();

        }
    }
    public void UISet()
    {
        var list = ItemIconList[(int)IconType.ItemSelect].Buttons;
        //UIセット
        foreach (var item in list)
        {
            var ibutton = item.GetComponent<ItemButton>();
            ibutton.clear();
        }

        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
        {
            if (item.Value.PoachHoldNumber == 0) continue;
            var ibutton = list[item.Value.PoachUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
        }


    }


    enum IconType
    {
        TypeSelect,
        ItemSelect
    }

}
