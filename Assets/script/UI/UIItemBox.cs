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
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            current = CurrentUI.box;
            var boxList = owner.ItemIconList[(int)IconType.BoxItemSelect].CreateButton();
            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.BoxHoldNumber == 0) continue;
                var ibutton = boxList[item.Value.BoxUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Value.ID, ItemBoxOrPoach.box);
            }
            var poachList = owner.ItemIconList[(int)IconType.PoachItemSelect].CreateButton();
            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.PoachHoldNumber == 0) continue;
                var ibutton = poachList[item.Value.PoachUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Value.ID, ItemBoxOrPoach.poach);
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.BoxItemSelect].DeleteButton();
            owner.ItemIconList[(int)IconType.PoachItemSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            if (UIManager.Instance.InputCurrentChange.ReadValue<Vector2>().sqrMagnitude > 0)
            {
                if (UIManager.Instance.InputCurrentChange.ReadValue<Vector2>().x > 0)
                {
                    current = CurrentUI.box;
                }
                else
                {
                    current = CurrentUI.poach;
                }
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

        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<FirstSlect>();
        }
        enum CurrentUI
        {
            box,
            poach
        }

    }
    enum IconType
    {
        TypeSelect,
        BoxItemSelect,
        PoachItemSelect
    }
}
