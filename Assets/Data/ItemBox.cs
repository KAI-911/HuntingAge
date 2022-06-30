using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ItemBox : MonoBehaviour
{
    private ItemHolder _itemHolder;
    private TargetChecker _targetChecker;
    private ItemIcon _itemIcons;
    private SelectIcon _selectIcons;

    //インプットシステム
    [SerializeField] private InputControls _input;
    private InputAction _inputAction;
    public InputAction InputAction { get => _inputAction; }

    private ItemUIState _currentState;

    //ボタンの配列
    //縦に並ぶ感じ
    [SerializeField] List<GameObject> _buttons;
    //選択中のボタン番号
    [SerializeField] int _currentButtonNumber;

    private void Awake()
    {
        _itemHolder = GetComponent<ItemHolder>();
        _targetChecker = GetComponentInChildren<TargetChecker>();
        _itemIcons = GetComponent<ItemIcon>();
        _selectIcons = GetComponent<SelectIcon>();
        _input = new InputControls();

        _currentState = new CanvasClose();
        _currentState.OnEnter(this, null);

    }
    private void OnEnable()
    {
        _inputAction = _input.UI.Selection;
        _input.UI.Proceed.started += Proceed;
        _input.UI.Proceed.canceled += Unlock;
        _input.UI.Back.started += Back;
        _input.UI.Back.canceled += Unlock;
        _input.UI.Enable();
    }


    private void OnDisable()
    {
        _input.UI.Proceed.started -= Proceed;
        _input.UI.Proceed.canceled -= Unlock;
        _input.UI.Back.started -= Back;
        _input.UI.Back.canceled -= Unlock;

        _input.UI.Disable();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _currentState.OnUpdate(this);
    }
    private void Proceed(InputAction.CallbackContext obj)
    {
        Debug.Log("Proceed_ItemBox");
        _currentState.OnProceed(this);
    }

    private void Back(InputAction.CallbackContext obj)
    {
        Debug.Log("Back_ItemBox");
        _currentState.OnBack(this);
    }

    private void Unlock(InputAction.CallbackContext obj)
    {

    }

    public void ChangeState<T>() where T : ItemUIState, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }
    public class ItemUIState
    {
        public virtual void OnEnter(ItemBox owner, ItemUIState prevState) { }
        public virtual void OnUpdate(ItemBox owner) { }
        public virtual void OnExit(ItemBox owner, ItemUIState nextState) { }
        public virtual void OnProceed(ItemBox owner) { }
        public virtual void OnBack(ItemBox owner) { }
    }
    class CanvasClose : ItemUIState
    {
        public override void OnProceed(ItemBox owner)
        {
            if (owner._targetChecker.TriggerHit)
            {
                Debug.Log("選択画面へ");
                owner.ChangeState<TypeSerect>();
            }
        }
    }
    class TypeSerect : ItemUIState
    {
        private Canvas _canvas;
        private RunOnce _once = new RunOnce();
        private int _button = 0;
        public override void OnEnter(ItemBox owner, ItemUIState prevState)
        {
            _canvas = GameManager.Instance.ItemCanvas.Canvas;
            owner._selectIcons.ButtonSize = 2;
            var buttons = owner._selectIcons.CreateButton();
            //テキストの設定
            buttons[0].GetComponentInChildren<Text>().text = "アイテム整理";
            //親の設定
            buttons[0].transform.parent = _canvas.transform;
            //ボタンが押されたときの設定
            var button0 = buttons[0].GetComponent<Button>();
            button0.onClick.RemoveAllListeners();
            
            //テキストの設定
            buttons[1].GetComponentInChildren<Text>().text = "武器整理";
            //親の設定
            buttons[1].transform.parent = _canvas.transform;
            //ボタンが押されたときの設定
            var button1 = buttons[1].GetComponent<Button>();
            button1.onClick.RemoveAllListeners();
        }
        public override void OnExit(ItemBox owner, ItemUIState nextState)
        {
            owner._selectIcons.DeleteButton();
        }
        public override void OnUpdate(ItemBox owner)
        {

            float v = owner._inputAction.ReadValue<Vector2>().y;
            if (Mathf.Abs(v) > 0)
            {
                if (_once.Flg) return;
                if (v > 0)_button = 0;
                else _button = 1;
                _once.Flg = true;
            }
            else
            {
                _once.Flg = false;
            }
            owner._selectIcons.Buttons[_button].GetComponent<Button>().Select();
        }
        public override void OnProceed(ItemBox owner)
        {
            Debug.Log("OnProceed_item   " + _button);
            if (_button == 0)
            {
                Debug.Log("OnProceed_ChangeStateItemSerect");
                owner.ChangeState<ItemSerect>();
            }
            else if (_button == 1)
            {
                Debug.Log("OnProceed_ChangeStateWeaponSerect");
                owner.ChangeState<WeaponSerect>();
            }

        }
        public override void OnBack(ItemBox owner)
        {
            owner.ChangeState<CanvasClose>();
        }

    }
    class ItemSerect : ItemUIState
    {
        private Canvas _canvas;
        private RunOnce _once = new RunOnce();
        private Button _button;
        public override void OnEnter(ItemBox owner, ItemUIState prevState)
        {
            _canvas = GameManager.Instance.ItemCanvas.Canvas;
            var buttons = owner._itemIcons.CreateButton();
            foreach (var item in buttons)
            {
                item.transform.parent = _canvas.transform;
            }
        }
        public override void OnExit(ItemBox owner, ItemUIState nextState)
        {
            owner._itemIcons.DeleteButton();

        }
        public override void OnUpdate(ItemBox owner)
        {
            Debug.Log("ItemSerect");
        }
        public override void OnBack(ItemBox owner)
        {
            owner.ChangeState<TypeSerect>();
        }

    }
    class WeaponSerect : ItemUIState
    {
        public override void OnBack(ItemBox owner)
        {
            owner.ChangeState<CanvasClose>();
        }
        public override void OnUpdate(ItemBox owner)
        {
            Debug.Log("WeaponSerect");
        }
    }

}
