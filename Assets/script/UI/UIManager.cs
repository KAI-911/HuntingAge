using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private InputControls _input;
    [SerializeField] private List<UIBase> _UIList;

    private InputAction _inputAction;
    public Player _player;
    public InputAction InputAction { get => _inputAction; }
    private void Awake()
    {
        _input = new InputControls();
    }
    public bool AddUIList(UIBase _uIBase)
    {
        if (_UIList.Contains(_uIBase)) return false;
        _UIList.Add(_uIBase);
        return true;
    }
    public bool EraseUIList(UIBase _uIBase)
    {
        if (!_UIList.Contains(_uIBase)) return false;
        _UIList.Remove(_uIBase);
        return true;
    }
    private void OnEnable()
    {
        _inputAction = _input.UI.Selection;
        _input.UI.Proceed.started += UIProceed;
        _input.UI.Back.started += UIBack;
        _input.UI.Menu.started += UIMenu;
        _input.UI.Enable();
    }


    private void OnDisable()
    {
        _input.UI.Proceed.started -= UIProceed;
        _input.UI.Back.started -= UIBack;
        _input.UI.Menu.started -= UIMenu;
        _input.UI.Disable();
    }

    private void UIMenu(InputAction.CallbackContext obj)
    {
        foreach (var ui in _UIList)
        {
            ui.Menu();
        }
    }

    private void UIBack(InputAction.CallbackContext obj)
    {
        foreach (var ui in _UIList)
        {
            ui.Backd();
        }
    }

    private void UIProceed(InputAction.CallbackContext obj)
    {
        foreach (var ui in _UIList)
        {
            ui.Proceed();
        }
    }
}
