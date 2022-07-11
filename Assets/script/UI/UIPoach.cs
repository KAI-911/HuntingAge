using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoach : UIBase
{
    [SerializeField] ItemListObject dataList;
    private string _addItemID;
    private int _addNumber;
    private void Start()
    {
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.ItemSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }
    private class Close : UIStateBase
    {

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UIManager.Instance._player.IsAction = true;
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
            var size = owner.ItemIconList[(int)IconType.TypeSelect].IconData;
            size._tableSize = new Vector2(1, 1);
            owner.ItemIconList[(int)IconType.TypeSelect].SetIcondata(size);
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

    private class AddItem : UIStateBase
    {
        ItemIcon itemIcon;
        int addNum;
        string addID;
        float time;
        RunOnce run = new RunOnce();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            itemIcon = owner.ItemIconList[(int)IconType.Confirmation];
            addNum = owner.GetComponent<UIPoach>()._addNumber;
            addID = owner.GetComponent<UIPoach>()._addItemID;
            time = 1;
            var data = itemIcon.IconData;
            Debug.Log(addNum);
            if (addNum == -1)
            {
                Debug.LogError("キーが見つからない");
                owner.ChangeState<Close>();
                return;
            }
            else if (addNum == -2)
            {
                data._tableSize = new Vector2(1, 2);
                data._textData.text = "手持ちがいっぱいです 入れ替えますか";
            }
            else if (addNum == -3)
            {
                data._textData.text = "これ以上持てません";
                data._tableSize = new Vector2(0, 0);
                _ = run.WaitForAsync(time, () => owner.ChangeState<Close>());
            }
            else
            {
                var itemdata = GameManager.Instance.ItemDataList.Dictionary[addID];
                data._textData.text = itemdata.Name + "を" + addNum + "個入手しました";
                data._tableSize = new Vector2(0, 0);
                _ = run.WaitForAsync(time, () => owner.ChangeState<Close>());
            }

            itemIcon.SetIcondata(data);
            itemIcon.CreateButton();
            //ボタンがない場合
            if ((data._tableSize.x * data._tableSize.y) == 0)
            {
                var backImage = itemIcon.ButtonBackObj.GetComponent<RectTransform>();
                backImage.sizeDelta = new Vector2(200, 50);
                backImage.anchoredPosition = new Vector2(-backImage.sizeDelta.x / 2, backImage.sizeDelta.y);
                var text = itemIcon.TextObj.GetComponent<RectTransform>();
                text.anchoredPosition = backImage.anchoredPosition;
                text.sizeDelta = backImage.sizeDelta;
            }
            else
            {
                var button0 = itemIcon.Buttons[0].GetComponent<Button>();
                button0.onClick.AddListener(() => owner.ChangeState<ItemChange>());
                var button0Text = itemIcon.Buttons[0].GetComponentInChildren<Text>();
                button0Text.text = "はい";
                var button1 = itemIcon.Buttons[1].GetComponent<Button>();
                button1.onClick.AddListener(() => owner.ChangeState<Close>());
                var button1Text = itemIcon.Buttons[1].GetComponentInChildren<Text>();
                button1Text.text = "いいえ";
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            itemIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            if (itemIcon.Buttons.Count > 0)
            {
                itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            }
        }
        public override void OnProceed(UIBase owner)
        {
            if (itemIcon.Buttons.Count > 0)
            {
                itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
            }
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
    }

    private class ItemChange : UIStateBase
    {
        ItemIcon itemIcon;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            itemIcon = owner.ItemIconList[(int)IconType.ItemSelect];
            UIManager.Instance._player.IsAction = false;
            var list = itemIcon.CreateButton();
            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.PoachHoldNumber == 0) continue;
                var ibutton = list[item.Value.PoachUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Value.ID, ItemBoxOrPoach.poach);
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            UIManager.Instance._player.IsAction = true;
            itemIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            var icon = itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<ItemButton>();
            if (!GameManager.Instance.ItemDataList.Keys.Contains(icon.ID)) return;
            int eraseindex = GameManager.Instance.ItemDataList.Keys.IndexOf(icon.ID);
            int addindex = GameManager.Instance.ItemDataList.Keys.IndexOf(owner.GetComponent<UIPoach>()._addItemID);
            var eraseItemData = GameManager.Instance.ItemDataList.Values[eraseindex];
            var addItemData = GameManager.Instance.ItemDataList.Values[addindex];
            eraseItemData.PoachHoldNumber = 0;
            addItemData.PoachHoldNumber = 1;
            addItemData.PoachUINumber = eraseItemData.PoachUINumber;
            GameManager.Instance.ItemDataList.Values[eraseindex] = eraseItemData;
            GameManager.Instance.ItemDataList.Values[addindex] = addItemData;
            GameManager.Instance.ItemDataList.DesrializeDictionary();
            Debug.Log("addItemData h" + addItemData.PoachHoldNumber + " ui " + addItemData.PoachUINumber);
            Debug.Log("eraseItemData h" + eraseItemData.PoachHoldNumber + " ui " + eraseItemData.PoachUINumber);
            owner.ChangeState<Close>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<AddItem>();
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="move"></param>
    /// <returns> 
    /// -1 キーが見つからない
    /// -2 UIの枠がない
    /// -3 上限まで持っている
    /// </returns>
    public int AddPoach(string ID, int move)
    {
        int returnValue = move;
        var itemdata = GameManager.Instance.ItemDataList;
        if (!itemdata.Keys.Contains(ID)) return -1;
        _addItemID = ID;
        //アイテムポーチにアイテムがある
        if (itemdata.Dictionary[ID].PoachHoldNumber > 0)
        {
            //上限まで持っている
            if (itemdata.Dictionary[ID].PoachHoldNumber == itemdata.Dictionary[ID].PoachStackNumber)
            {
                returnValue = -3;
                _addNumber = returnValue;
                Debug.Log(_addNumber);
                ChangeState<AddItem>();
                return returnValue;
            }
            returnValue = itemdata.GetToPoach(ID, move, itemdata.Dictionary[ID].PoachUINumber);
            itemdata.DesrializeDictionary();
            _addNumber = returnValue;
            Debug.Log(_addNumber);
            ChangeState<AddItem>();
            return returnValue;
        }
        //アイテムポーチにない場合UIの位置を設定して追加
        var data = ItemIconList[(int)IconType.ItemSelect].IconData;
        int num = (int)data._tableSize.x * (int)data._tableSize.y;
        List<int> vs = new List<int>();
        for (int i = 0; i < num; i++) vs.Add(i);
        foreach (var item in itemdata.Values)
        {
            if (vs.Count == 0)
            {
                break;
            }
            if (item.PoachHoldNumber <= 0) continue;
            vs.Remove(item.PoachUINumber);
        }

        if (vs.Count == 0)
        {
            returnValue = -2;
            _addNumber = returnValue;
            Debug.Log(_addNumber);
            ChangeState<AddItem>();
            return returnValue;
        }
        itemdata.GetToPoach(ID, move, vs[0]);
        itemdata.DesrializeDictionary();
        _addNumber = returnValue;
        ChangeState<AddItem>();
        return returnValue;
    }

    enum IconType
    {
        TypeSelect,
        ItemSelect,
        Confirmation
    }

}
