using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoach : UIBase
{
    private string _addItemID;
    private int _addNumber;
    private void Start()
    {
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.ItemSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }
    private class Close : UIStateBase
    {

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UISoundManager.Instance._player.IsAction = true;
        }
        public override void OnMenu(UIBase owner)
        {
            if (!UISoundManager.Instance._player.IsAction) return;
            UISoundManager.Instance._player.IsAction = false;
            UISoundManager.Instance.PlayDecisionSE();
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
            Debug.Log(owner.ItemIconList[(int)IconType.TypeSelect].TableSize);
            var button0 = list[0].GetComponent<Button>();
            button0.onClick.AddListener(() => owner.ChangeState<ItemSlect>());
            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "アイテム選択";

            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() =>
            {
                if (GameManager.Instance.Quest.QuestData.ID != "")
                {
                    owner.ChangeState<QuestView>();
                }
            });
            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "クエスト確認";

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            Debug.Log("UIStateBase_FirstSlect");
        }
    }
    private class ItemSlect : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            if (prevState.GetType() == typeof(UIChange)) return;

            owner.ItemIconList[(int)IconType.ItemSelect].CreateButton();

            owner.GetComponent<UIPoach>().UISet();
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            if (nextState.GetType() == typeof(UIChange)) return;

            owner.ItemIconList[(int)IconType.ItemSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ItemSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            Debug.Log("ItemSlect_SecondSelect");
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ChangeState<UIChange>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<FirstSlect>();
        }
    }
    private class QuestView : UIStateBase
    {
        GameObject questBord;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            questBord = Instantiate(Resources.Load("UI/QuestMenu"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;

            QuestData data = GameManager.Instance.Quest.QuestData;

            string str = "";
            switch (data.Clear)
            {
                case ClearConditions.TargetSubjugation:


                    str += "クリア条件: ";
                    foreach (var item in data.TargetName)
                    {
                        var tmp = GameManager.Instance.EnemyDataList.Dictionary[item.name];

                        str += tmp.DisplayName + "を" + item.number + "体討伐する\n";
                    }
                    break;
                case ClearConditions.Gathering:
                    str += "クリア条件: ";
                    foreach (var item in data.TargetName)
                    {
                        var tmp = GameManager.Instance.MaterialDataList.Dictionary[item.name];

                        str += tmp.Name + "を" + item.number + "個採取する\n";
                    }
                    break;
                default:
                    break;
            }
            str += "失敗条件: " + (int)(data.Failure + 1) + "回力尽きる\n";
            switch (data.Field)
            {
                case Scene.Forest:
                    str += "狩場: 森林";
                    break;
                case Scene.Animal:
                    str += "狩場: 実験用";
                    break;
                default:
                    break;
            }
            var texts = questBord.GetComponentsInChildren<Text>();
            texts[0].text = data.Name;
            texts[1].text = str;

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            Destroy(questBord);
        }
        public override void OnUpdate(UIBase owner)
        {
        }
        public override void OnProceed(UIBase owner)
        {
        }
        public override void OnBack(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
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
            _itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());

        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
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
                if (MaterialDataList.Keys.Contains(selectButton.ID))
                {
                    int index = MaterialDataList.Keys.IndexOf(selectButton.ID);
                    data = MaterialDataList.Values[index];
                    data.PoachUINumber = _itemIcon.CurrentNunber;
                    MaterialDataList.Values[index] = data;
                }
                else if (ItemDataList.Dictionary.ContainsKey(selectButton.ID))
                {
                    int index = ItemDataList.Keys.IndexOf(selectButton.ID);
                    data = ItemDataList.Values[index].baseData;
                    data.PoachUINumber = _itemIcon.CurrentNunber;
                    var tmp = ItemDataList.Values[index];
                    tmp.baseData = data;
                    ItemDataList.Values[index] = tmp;
                }
            }
            if (currentButton.ID != "")
            {
                if (MaterialDataList.Keys.Contains(currentButton.ID))
                {
                    int index = MaterialDataList.Keys.IndexOf(currentButton.ID);
                    data = MaterialDataList.Values[index];
                    data.PoachUINumber = _selectionNumber;
                    MaterialDataList.Values[index] = data;
                }
                else if (ItemDataList.Dictionary.ContainsKey(currentButton.ID))
                {
                    int index = ItemDataList.Keys.IndexOf(currentButton.ID);
                    data = ItemDataList.Values[index].baseData;
                    data.PoachUINumber = _selectionNumber;
                    var tmp = ItemDataList.Values[index];
                    tmp.baseData = data;
                    ItemDataList.Values[index] = tmp;
                }
            }

            MaterialDataList.DesrializeDictionary();
            ItemDataList.DesrializeDictionary();
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
            Debug.Log(owner.GetComponent<UIPoach>()._addItemID + "    _addItemID");
            if (addID == null) return;
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
                var itemdata = GameManager.Instance.MaterialDataList.Dictionary[addID];
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
                itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            }
        }
        public override void OnProceed(UIBase owner)
        {
            if (itemIcon.Buttons.Count > 0)
            {
                UISoundManager.Instance.PlayDecisionSE();
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
            UISoundManager.Instance._player.IsAction = false;
            var list = itemIcon.CreateButton();
            foreach (var item in GameManager.Instance.MaterialDataList.Dictionary)
            {
                if (item.Value.PoachHoldNumber == 0) continue;
                var ibutton = list[item.Value.PoachUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Value.ID, ItemBoxOrPoach.poach);
            }
            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.baseData.PoachHoldNumber == 0) continue;
                var ibutton = list[item.Value.baseData.PoachUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Value.baseData.ID, ItemBoxOrPoach.poach);
            }

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            UISoundManager.Instance._player.IsAction = true;
            itemIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            var OWNER = owner.GetComponent<UIPoach>();
            var icon = itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<ItemButton>();
            MaterialDataList materialList = GameManager.Instance.MaterialDataList;
            ItemDataList ItemList = GameManager.Instance.ItemDataList;

            //マテリアルとマテリアルの交換
            if (materialList.Keys.Contains(icon.ID) && materialList.Keys.Contains(OWNER._addItemID))
            {

                int eraseindex = materialList.Keys.IndexOf(icon.ID);
                int addindex = materialList.Keys.IndexOf(OWNER._addItemID);
                var eraseItemData = materialList.Values[eraseindex];
                var addItemData = materialList.Values[addindex];
                eraseItemData.PoachHoldNumber = 0;
                addItemData.PoachHoldNumber = 1;
                addItemData.PoachUINumber = eraseItemData.PoachUINumber;
                materialList.Values[eraseindex] = eraseItemData;
                materialList.Values[addindex] = addItemData;
                materialList.DesrializeDictionary();
            }
            //アイテムとアイテムの交換
            else if (ItemList.Keys.Contains(icon.ID) && ItemList.Keys.Contains(OWNER._addItemID))
            {
                int eraseindex = ItemList.Keys.IndexOf(icon.ID);
                int addindex = ItemList.Keys.IndexOf(owner.GetComponent<UIPoach>()._addItemID);
                var eraseItemData = ItemList.Values[eraseindex];
                var addItemData = ItemList.Values[addindex];
                eraseItemData.baseData.PoachHoldNumber = 0;
                addItemData.baseData.PoachHoldNumber = 1;
                addItemData.baseData.PoachUINumber = eraseItemData.baseData.PoachUINumber;
                ItemList.Values[eraseindex] = eraseItemData;
                ItemList.Values[addindex] = addItemData;
                ItemList.DesrializeDictionary();
            }
            //アイテムとマテリアルの交換
            else if (ItemList.Keys.Contains(icon.ID) && materialList.Keys.Contains(OWNER._addItemID))
            {
                int eraseindex = ItemList.Keys.IndexOf(icon.ID);
                int addindex = materialList.Keys.IndexOf(owner.GetComponent<UIPoach>()._addItemID);
                var eraseItemData = ItemList.Values[eraseindex];
                var addItemData = materialList.Values[addindex];
                eraseItemData.baseData.PoachHoldNumber = 0;
                addItemData.PoachHoldNumber = 1;
                addItemData.PoachUINumber = eraseItemData.baseData.PoachUINumber;
                ItemList.Values[eraseindex] = eraseItemData;
                materialList.Values[addindex] = addItemData;
                ItemList.DesrializeDictionary();
                materialList.DesrializeDictionary();
            }
            //マテリアルとアイテムの交換
            else if (materialList.Keys.Contains(icon.ID) && ItemList.Keys.Contains(OWNER._addItemID))
            {
                int eraseindex = materialList.Keys.IndexOf(icon.ID);
                int addindex = ItemList.Keys.IndexOf(owner.GetComponent<UIPoach>()._addItemID);
                var eraseItemData = materialList.Values[eraseindex];
                var addItemData = ItemList.Values[addindex];
                eraseItemData.PoachHoldNumber = 0;
                addItemData.baseData.PoachHoldNumber = 1;
                addItemData.baseData.PoachUINumber = eraseItemData.PoachUINumber;
                materialList.Values[eraseindex] = eraseItemData;
                ItemList.Values[addindex] = addItemData;
                ItemList.DesrializeDictionary();
                materialList.DesrializeDictionary();
            }
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

        foreach (var item in GameManager.Instance.MaterialDataList.Dictionary)
        {
            if (item.Value.PoachHoldNumber == 0) continue;
            var ibutton = list[item.Value.PoachUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
        }
        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
        {
            if (item.Value.baseData.PoachHoldNumber == 0) continue;
            var ibutton = list[item.Value.baseData.PoachUINumber].GetComponent<ItemButton>();
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
        var materialdata = GameManager.Instance.MaterialDataList;
        var itemdata = GameManager.Instance.ItemDataList;

        //アイテムポーチにアイテムがある
        if (materialdata.Keys.Contains(ID))
        {
            if (materialdata.Dictionary[ID].PoachHoldNumber > 0)
            {
                //上限まで持っている 
                if (materialdata.Dictionary[ID].PoachHoldNumber == materialdata.Dictionary[ID].PoachStackNumber)
                {
                    returnValue = -3;
                    _addNumber = returnValue;
                    Debug.Log(_addNumber + "   " + ID);
                    _addItemID = ID;
                    ChangeState<AddItem>();
                    return returnValue;
                }
                returnValue = materialdata.GetToPoach(ID, move, materialdata.Dictionary[ID].PoachUINumber);
                materialdata.DesrializeDictionary();
                _addNumber = returnValue;
                Debug.Log(_addNumber);
                ChangeState<AddItem>();
                return returnValue;
            }
        }
        else if (itemdata.Keys.Contains(ID))
        {
            if (itemdata.Dictionary[ID].baseData.PoachHoldNumber > 0)
            {
                //上限まで持っている
                if (itemdata.Dictionary[ID].baseData.PoachHoldNumber == itemdata.Dictionary[ID].baseData.PoachStackNumber)
                {
                    returnValue = -3;
                    _addNumber = returnValue;
                    Debug.Log(_addNumber);
                    ChangeState<AddItem>();
                    return returnValue;
                }
                returnValue = itemdata.GetToPoach(ID, move, itemdata.Dictionary[ID].baseData.PoachUINumber);
                itemdata.DesrializeDictionary();
                _addNumber = returnValue;
                Debug.Log(_addNumber);
                ChangeState<AddItem>();
                return returnValue;
            }
        }
        else
        {
            return -1;
        }


        _addItemID = ID;
        //アイテムポーチにない場合UIの位置を設定して追加
        var data = ItemIconList[(int)IconType.ItemSelect].IconData;
        int num = (int)data._tableSize.x * (int)data._tableSize.y;
        List<int> vs = new List<int>();
        for (int i = 0; i < num; i++) vs.Add(i);
        foreach (var item in materialdata.Values)
        {
            if (vs.Count == 0)
            {
                break;
            }
            if (item.PoachHoldNumber <= 0) continue;
            vs.Remove(item.PoachUINumber);
        }
        foreach (var item in itemdata.Values)
        {
            if (vs.Count == 0)
            {
                break;
            }
            if (item.baseData.PoachHoldNumber <= 0) continue;
            vs.Remove(item.baseData.PoachUINumber);
        }
        if (vs.Count == 0)
        {
            returnValue = -2;
            _addNumber = returnValue;
            Debug.Log(_addNumber);
            ChangeState<AddItem>();
            return returnValue;
        }

        if (materialdata.Keys.Contains(ID))
        {
            materialdata.GetToPoach(ID, move, vs[0]);
            materialdata.DesrializeDictionary();
        }
        else if (itemdata.Keys.Contains(ID))
        {
            itemdata.GetToPoach(ID, move, vs[0]);
            itemdata.DesrializeDictionary();
        }
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
