using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private InputControls _input;
    [SerializeField] private List<UIBase> _UIList;
    [SerializeField] UIPresetDataList _UIPresetData;
    private InputAction _inputSelection;
    private InputAction _inputCurrentChange;
    public Player _player;
    public InputAction InputSelection { get => _inputSelection; }
    public InputAction InputCurrentChange { get => _inputCurrentChange; }
    public UIPresetDataList UIPresetData { get => _UIPresetData; set => _UIPresetData = value; }

    protected override void Awake()
    {
        base.Awake();
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
        _inputSelection = _input.UI.Selection;
        _inputCurrentChange = _input.UI.CurrentChange;
        _input.UI.Proceed.started += UIProceed;
        _input.UI.Back.started += UIBack;
        _input.UI.Menu.started += UIMenu;
        _input.UI.SubMenu.started += UISubMenu;
        _input.UI.Enable();
    }

    private void OnDisable()
    {
        _input.UI.Proceed.started -= UIProceed;
        _input.UI.Back.started -= UIBack;
        _input.UI.Menu.started -= UIMenu;
        _input.UI.SubMenu.started -= UISubMenu;
        _input.UI.Disable();
    }

    private void UIMenu(InputAction.CallbackContext obj)
    {
        foreach (var ui in _UIList)
        {
            ui.Menu();
        }
    }
    private void UISubMenu(InputAction.CallbackContext obj)
    {
        foreach (var ui in _UIList)
        {
            ui.SubMenu();
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
