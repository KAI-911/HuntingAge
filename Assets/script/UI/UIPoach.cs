using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoach : UIBase
{
    [SerializeField] ItemListObject dataList;

    private void Start()
    {
        UIManager.AddUIList(this);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }


    private class Close : UIStateBase
    {

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("UIStateBase_close_OnEnter");
            owner.UIManager._player.IsAction = true;
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("UIStateBase_close");
        }
        public override void OnMenu(UIBase owner)
        {
            owner.UIManager._player.IsAction = false;
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
            var list = owner.FirstSelect.CreateButton();
            var button0 = list[0].GetComponent<Button>();
            button0.onClick.AddListener(()=>owner.ChangeState<ItemSlect>());
            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "アイテム選択";
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.FirstSelect.DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            owner.FirstSelect.Buttons[owner.FirstSelect.CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.FirstSelect.Select(owner.UIManager.InputAction.ReadValue<Vector2>());
            Debug.Log("UIStateBase_FirstSlect");
        }
    }
    private class ItemSlect : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var list = owner.SecondSelect.CreateButton();

            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.PoachHoldNumber == 0) continue;
                var ibutton = list[item.Value.BoxUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Value.ID);
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.SecondSelect.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.SecondSelect.Select(owner.UIManager.InputAction.ReadValue<Vector2>());
            Debug.Log("ItemSlect_SecondSelect");
        }
        public override void OnProceed(UIBase owner)
        {
            
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<FirstSlect>();
        }

    }
}
