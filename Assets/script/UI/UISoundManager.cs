using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UISoundManager : Singleton<UISoundManager>
{
    [SerializeField] private InputControls _input;
    [SerializeField] private List<UIBase> _UIList;
    [SerializeField] UIPresetDataList _UIPresetData;
    [SerializeField] GameObject _cursorSE;
    [SerializeField] GameObject _questSE;
    [SerializeField] GameObject _decisionSE;
    [SerializeField] GameObject _kickAttackSE;
    [SerializeField] GameObject _kickSwingSE;
    [SerializeField] GameObject _swordAttackSE;
    [SerializeField] GameObject _swordSwingSE;


    private InputAction _inputSelection;
    private InputAction _inputCurrentChange;
    private InputAction _inputItemView;
    public Player _player;
    public InputAction InputSelection { get => _inputSelection; }
    public InputAction InputCurrentChange { get => _inputCurrentChange; }
    public InputAction InputItemView { get => _inputItemView; }
    public UIPresetDataList UIPresetData { get => _UIPresetData; set => _UIPresetData = value; }
    public GameObject CursorSE { get => _cursorSE; }
    public GameObject QuestSE { get => _questSE; }
    public GameObject DecisionSE { get => _decisionSE; }
    public GameObject KickAttackSE { get => _kickAttackSE; }
    public GameObject KickSwingSE { get => _kickSwingSE; }
    public GameObject SwordAttackSE { get => _swordAttackSE; }
    public GameObject SwordSwingSE { get => _swordSwingSE; }

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
    public void PlaySwordAttackSE()
    {
        Instantiate(_swordAttackSE);
    }
    public void PlaySwordSwingSE()
    {
        Instantiate(_swordSwingSE);
    }
    public void PlayKickAttackSE()
    {
        Instantiate(_kickAttackSE);
    }
    public void PlayKickSwingSE()
    {
        Instantiate(_kickSwingSE);
    }
    private void OnEnable()
    {
        _inputSelection = _input.UI.Selection;
        _inputCurrentChange = _input.UI.CurrentChange;
        _inputItemView = _input.UI.UIItemView;
        _input.UI.Enable();
        //決定、進む
        _input.UI.Proceed.started += UIProceed;
        //否定、戻る
        _input.UI.Back.started += UIBack;
        //メニューを開く
        _input.UI.Menu.started += UIMenu;
        _input.UI.SubMenu.started += UISubMenu;
        _input.UI.UseItemSelect.started += UIUseItemSelectStart;
        _input.UI.UseItemSelect.canceled += UIUseItemSelectEnd;
        _input.UI.Title.started += NextVillage;
        _input.UI.ESC.started += ESC;
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
        _input.UI.Title.started -= NextVillage;
        _input.UI.ESC.started -= ESC;

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

    private void NextVillage(InputAction.CallbackContext obj)
    {
        GameManager.Instance.GoToVillage();
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        foreach (var ui in _UIList)
        {
            ui.SceneChenge();
        }
    }
    private void ESC(InputAction.CallbackContext obj)
    {
        if (Cursor.visible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

}
