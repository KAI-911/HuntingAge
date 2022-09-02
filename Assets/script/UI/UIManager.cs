using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private InputControls _input;
    [SerializeField] private List<UIBase> _UIList;
    [SerializeField] UIPresetDataList _UIPresetData;
    [SerializeField] GameObject _cursorSE;
    [SerializeField] GameObject _questSE;
    [SerializeField] GameObject _decisionSE;
    private InputAction _inputSelection;
    private InputAction _inputCurrentChange;
    public Player _player;
    public InputAction InputSelection { get => _inputSelection; }
    public InputAction InputCurrentChange { get => _inputCurrentChange; }
    public UIPresetDataList UIPresetData { get => _UIPresetData; set => _UIPresetData = value; }

    protected override void Awake()
    {
        _input = new InputControls();
        base.Awake();

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
    public void PlayCursorSE()
    {
        Instantiate(_cursorSE);
    }
    public void PlayQuestSE()
    {
        Instantiate(_questSE);
    }
    public void PlayDecisionSE()
    {
        Instantiate(_decisionSE);
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
        _input.UI.UseItemSelect.started += UIUseItemSelectStart;
        _input.UI.UseItemSelect.canceled += UIUseItemSelectEnd;
        _input.UI.boxbutton.started += UIBoxPush;
        _input.UI.trianglebutton.started += UITrianglePush;
        SceneManager.sceneLoaded += OnSceneLoaded;

    }


    private void OnDisable()
    {
        _input.UI.Proceed.started -= UIProceed;
        _input.UI.Back.started -= UIBack;
        _input.UI.Menu.started -= UIMenu;
        _input.UI.SubMenu.started -= UISubMenu;
        _input.UI.Disable();
        _input.UI.UseItemSelect.started -= UIUseItemSelectStart;
        _input.UI.UseItemSelect.canceled -= UIUseItemSelectEnd;
        _input.UI.boxbutton.started -= UIBoxPush;
        _input.UI.trianglebutton.started -= UITrianglePush;
        SceneManager.sceneLoaded -= OnSceneLoaded;

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
    private void UIUseItemSelectStart(InputAction.CallbackContext obj)
    {
        foreach (var ui in _UIList)
        {
            ui.UseItemSelectStart();
        }
    }
    private void UIUseItemSelectEnd(InputAction.CallbackContext obj)
    {
        foreach (var ui in _UIList)
        {
            ui.UseItemSelectEnd();
        }
    }
    private void UITrianglePush(InputAction.CallbackContext obj)
    {
        foreach (var ui in _UIList)
        {
            ui.PushTriangleButton();
        }
    }

    private void UIBoxPush(InputAction.CallbackContext obj)
    {
        foreach (var ui in _UIList)
        {
            ui.PushBoxButton();
        }
    }
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        foreach (var ui in _UIList)
        {
            ui.SceneChenge();
        }
    }

}
