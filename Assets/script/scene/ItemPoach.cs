using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor;
using System;

public class ItemPoach : MonoBehaviour
{
    private ItemHolder _itemHolder;
    [SerializeField] ItemIcon _itemIcons;
    [SerializeField] ItemIcon _selectIcons;

    //インプットシステム
    [SerializeField] private InputControls _input;
    private InputAction _inputAction;
    public InputAction InputAction { get => _inputAction; }

    private ItemUIState _currentState;

    private Player _player;

    private void Awake()
    {
        _itemHolder = GetComponent<ItemHolder>();
        _input = new InputControls();


    }
    private void OnEnable()
    {
        _inputAction = _input.UI.Selection;
        _input.UI.Proceed.started += Proceed;
        _input.UI.Back.started += Back;
        _input.UI.Menu.started += UIMenu;
       _input.UI.Enable();
    }


    private void OnDisable()
    {
        _input.UI.Proceed.started -= Proceed;
        _input.UI.Back.started -= Back;
        _input.UI.Menu.started -= UIMenu;
        _input.UI.Disable();
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _currentState = new CanvasClose();
        _currentState.OnEnter(this, null);

    }

    void Update()
    {
        _currentState.OnUpdate(this);
    }
    private void Proceed(InputAction.CallbackContext obj)
    {
        Debug.Log("Proceed_ItemPoach");
        _currentState.OnProceed(this);
    }

    private void Back(InputAction.CallbackContext obj)
    {
        Debug.Log("Back_ItemPoach");
        _currentState.OnBack(this);
    }

    private void UIMenu(InputAction.CallbackContext obj)
    {
        Debug.Log("Menu_ItemPoach");
        _currentState.OnMenu(this);
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
        public virtual void OnEnter(ItemPoach owner, ItemUIState prevState) { }
        public virtual void OnUpdate(ItemPoach owner) { }
        public virtual void OnExit(ItemPoach owner, ItemUIState nextState) { }
        public virtual void OnProceed(ItemPoach owner) { }
        public virtual void OnBack(ItemPoach owner) { }
        public virtual void OnMenu(ItemPoach owner) { }
    }
    class CanvasClose : ItemUIState
    {
        public override void OnEnter(ItemPoach owner, ItemUIState prevState)
        {
            owner._player.IsAction = true;
        }
        public override void OnMenu(ItemPoach owner)
        {
                Debug.Log("選択画面へ");
                owner._player.IsAction = false;
                owner.ChangeState<TypeSerect>();
        }
    }
    class TypeSerect : ItemUIState
    {
        private Canvas _canvas;
        private RunOnce _once = new RunOnce();
        public override void OnEnter(ItemPoach owner, ItemUIState prevState)
        {
            owner._player.IsAction = false;
            _canvas = GameManager.Instance.ItemCanvas.Canvas;
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
        public override void OnExit(ItemPoach owner, ItemUIState nextState)
        {
            owner._selectIcons.DeleteButton();
        }
        public override void OnUpdate(ItemPoach owner)
        {

            owner._selectIcons.Select(owner._inputAction.ReadValue<Vector2>());
            if ((owner._selectIcons.TableSize.x * owner._selectIcons.TableSize.y) > 0)
            {
                owner._selectIcons.Buttons[owner._selectIcons.CurrentNunber].GetComponent<Button>().Select();
            }
        }
        public override void OnProceed(ItemPoach owner)
        {
            if (owner._selectIcons.CurrentNunber == 0)
            {
                Debug.Log("OnProceed_ChangeStateItemSerect");
                owner.ChangeState<ItemSerect>();
            }
            else if (owner._selectIcons.CurrentNunber == 1)
            {
                Debug.Log("OnProceed_ChangeStateWeaponSerect");
                owner.ChangeState<WeaponSerect>();
            }

        }
        public override void OnBack(ItemPoach owner)
        {
            owner.ChangeState<CanvasClose>();
        }

    }
    class ItemSerect : ItemUIState
    {
        private Canvas _canvas;
        private RunOnce _once = new RunOnce();
        private state _itemState;
        public override void OnEnter(ItemPoach owner, ItemUIState prevState)
        {
            _itemState = state.itemSelect;
            _canvas = GameManager.Instance.ItemCanvas.Canvas;
            var buttons = owner._itemIcons.CreateButton();
            foreach (var item in buttons)
            {
                item.transform.parent = _canvas.transform;
            }
            foreach (var item in owner._itemHolder.Dictionary)
            {
                var data = GameManager.Instance.ItemDataList.Dictionary[item.Key];
                if (data.BoxHoldNumber == 0) continue;
                var ibutton = buttons[data.BoxUINumber].GetComponent<ItemButton>();
                ibutton.SetID(data.ID, ItemBoxOrPoach.poach);
            }
        }
        public override void OnExit(ItemPoach owner, ItemUIState nextState)
        {
            owner._itemIcons.DeleteButton();

        }
        public override void OnUpdate(ItemPoach owner)
        {
            Debug.Log("ItemSerect");

            switch (_itemState)
            {
                case state.itemSelect:
                    owner._itemIcons.Select(owner._inputAction.ReadValue<Vector2>());
                    break;
                case state.itemExchange:
                    break;
                default:
                    break;
            }
            owner._itemIcons.Buttons[owner._itemIcons.CurrentNunber].GetComponent<Button>().Select();
        }

        public override void OnProceed(ItemPoach owner)
        {
            switch (_itemState)
            {
                case state.itemSelect:
                    break;
                case state.itemExchange:
                    break;
                default:
                    break;
            }
        }
        public override void OnBack(ItemPoach owner)
        {
            switch (_itemState)
            {
                case state.itemSelect:
                    owner.ChangeState<TypeSerect>();
                    break;
                case state.itemExchange:
                    break;
                default:
                    break;
            }

        }

        enum state
        {
            itemSelect,
            itemExchange
        }

    }
    class WeaponSerect : ItemUIState
    {
        public override void OnBack(ItemPoach owner)
        {
            owner.ChangeState<CanvasClose>();
        }
        public override void OnUpdate(ItemPoach owner)
        {
            Debug.Log("WeaponSerect");
        }
    }
}
