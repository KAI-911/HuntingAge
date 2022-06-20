using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
public class Blacksmith : MonoBehaviour
{
    ///表示するテキスト
    [SerializeField] Text _weaponName;
    [SerializeField] Text _weaponContents;
    //プレイヤーが近くまで来たか判断
    [SerializeField] TargetChecker _questbordChecker;
    [SerializeField] TargetChecker _gateChecker;
    //キャンバス
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _buttonParent;

    //確認用
    [SerializeField] Canvas _popUp;
    [SerializeField] Button _proceedButton;
    [SerializeField] Button _backButton;
    [SerializeField] Text _infoText;

    //ボタンの配列
    [SerializeField] List<GameObject> _buttons;
    //選択中のボタン番号
    [SerializeField] int _currentButtonNumber;

    //インプットシステム
    [SerializeField] private InputControls _input;
    private InputAction _inputAction;
    public InputAction InputAction { get => _inputAction; }

    // Start is called before the first frame update
    private UIState _currentState;
    private void Awake()
    {
        _input = new InputControls();
        _buttonRunOnce = new RunOnce();
        _serectRunOnce = new RunOnce();
        _questHolder = new QuestHolder();
        _currentState = new CanvasClose();
        _currentState.OnEnter(this, null);
    }
    void Start()
    {
        _canvas.enabled = false;
        _popUp.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
