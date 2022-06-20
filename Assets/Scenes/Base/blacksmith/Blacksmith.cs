using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
public class Blacksmith : MonoBehaviour
{
    ///�\������e�L�X�g
    [SerializeField] Text _weaponName;
    [SerializeField] Text _weaponContents;
    //�v���C���[���߂��܂ŗ��������f
    [SerializeField] TargetChecker _questbordChecker;
    [SerializeField] TargetChecker _gateChecker;
    //�L�����o�X
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _buttonParent;

    //�m�F�p
    [SerializeField] Canvas _popUp;
    [SerializeField] Button _proceedButton;
    [SerializeField] Button _backButton;
    [SerializeField] Text _infoText;

    //�{�^���̔z��
    [SerializeField] List<GameObject> _buttons;
    //�I�𒆂̃{�^���ԍ�
    [SerializeField] int _currentButtonNumber;

    //�C���v�b�g�V�X�e��
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
