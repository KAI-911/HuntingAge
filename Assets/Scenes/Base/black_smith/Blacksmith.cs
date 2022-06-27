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
    [SerializeField] Text _blacksmithMode;
    //�v���C���[���߂��܂ŗ��������f
    [SerializeField] TargetChecker _blacksmithChecker;
    //�L�����o�X
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _buttonParent;

    //�m�F�p
    [SerializeField] Canvas _popUp;
    [SerializeField] Button _proceedButton;
    [SerializeField] Button _backButton;
    [SerializeField] Text _infoText;

    //���x���ʃN�G�X�g�̂܂Ƃ܂�
    [SerializeField] WeaponData _weaponData;

    //�{�^���̔z��
    [SerializeField] List<GameObject> _buttons;
    //�I�𒆂̃{�^���ԍ�
    [SerializeField] int _currentButtonNumber;

    [SerializeField] WeaponData _WeaponData;
    private int productionWeapon;
    private enum WeaponType
    {
        Axe = 1,
        Spear,
        Bow
    }

    //�C���v�b�g�V�X�e��
    [SerializeField] private InputControls _input;
    private InputAction _inputAction;
    public InputAction InputAction { get => _inputAction; }

    [SerializeField] private RunOnce _buttonRunOnce;
    [SerializeField] private RunOnce _serectRunOnce;

    [SerializeField] private Vector3 firstButtonPos;
    [SerializeField] private float buttonBetween;

    // Start is called before the first frame update
    private UIState _currentState;
    private void Awake()
    {
        _input = new InputControls();
        _buttonRunOnce = new RunOnce();
        _serectRunOnce = new RunOnce();
        _weaponData = new WeaponData();
        _currentState = new CanvasClose();
        _currentState.OnEnter(this, null);
    }
    void Start()
    {
        _canvas.enabled = false;
        _popUp.enabled = false;
    }
    private void OnEnable()
    {
        _inputAction = _input.UI.Selection;
        _input.UI.Proceed.started += Proceed;

        _input.UI.Back.started += Back;

        _input.UI.Enable();
    }

    private void OnDisable()
    {
        _input.UI.Proceed.started -= Proceed;
        _input.UI.Back.started -= Back;

        _input.UI.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        _currentState.OnUpdate(this);
        var s = _currentState.GetType();
        Debug.Log(s);
    }

    private void Serect()
    {
        float v = _inputAction.ReadValue<Vector2>().y;
        if (_currentButtonNumber > _buttons.Count && _buttons.Count >= 1) _currentButtonNumber = 0;

        if (Mathf.Abs(v) > 0)
        {
            if (_serectRunOnce.Flg) return;
            if (v < 0)
            {
                _currentButtonNumber++;
                if (_currentButtonNumber > _buttons.Count - 1) _currentButtonNumber = _buttons.Count - 1;
            }
            else
            {
                _currentButtonNumber--;
                if (_currentButtonNumber < 0) _currentButtonNumber = 0;
            }
            _serectRunOnce.Flg = true;
        }
        else
        {
            _serectRunOnce.Flg = false;
        }
        _buttons[_currentButtonNumber].GetComponent<Button>().Select();
    }

    public void ModeSelect()
    {

    }


    void ButtonDelete()
    {
        foreach (var item in _buttons)
        {
            Destroy(item);
        }
        _buttons.Clear();
    }

    private void Back(InputAction.CallbackContext obj)
    {
        Debug.Log("Back");
        _currentState.OnBack(this);

    }

    private void Proceed(InputAction.CallbackContext obj)
    {
        Debug.Log("Proceed");
        _currentState.OnProceed(this);
    }

    public void ChangeState<T>() where T : UIState, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }

    public abstract class UIState
    {
        public virtual void OnEnter(Blacksmith owner, UIState prevState)
        {

        }
        public virtual void OnUpdate(Blacksmith owner)
        {

        }
        public virtual void OnExit(Blacksmith owner, UIState nextState)
        {

        }
        public virtual void OnProceed(Blacksmith owner)
        {

        }
        public virtual void OnBack(Blacksmith owner)
        {

        }
    }

    [Serializable]
    public class CanvasClose : UIState
    {
        public override void OnEnter(Blacksmith owner, UIState prevState)
        {
            owner.ButtonDelete();
            owner._canvas.enabled = false;
            owner._popUp.enabled = false;
        }
        public override void OnUpdate(Blacksmith owner)
        { }
        public override void OnExit(Blacksmith owner, UIState nextState)
        { }
        public override void OnProceed(Blacksmith owner)
        {
            //�߂��ɗ��Ă��� && ����{�^���������Ă��� && �L�����o�X��active�łȂ�
            if (owner._blacksmithChecker.TriggerHit && !owner._canvas.enabled)
            {
                owner.ChangeState<SelectMode>();
            }
        }
        public override void OnBack(Blacksmith owner)
        { }
    }

    public class SelectMode : UIState
    {
        public override void OnEnter(Blacksmith owner, UIState prevState)
        {
            owner._canvas.enabled = true;
            owner._popUp.enabled = false;

            //���[�h�I�����
            owner.ButtonDelete();
            owner._canvas.enabled = true;

            owner._blacksmithMode.text = "��������H";

            //�{�^���̒ǉ�
            for (int i = 0; i < 2/*���Y�A�����̓��݂̂̂���*/; i++)
            {
                //�{�^���̈ʒu�ݒ�
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //�C���X�^���X��
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //�e�̐ݒ�
                obj.transform.parent = owner._buttonParent.transform;
                //�{�^���������ꂽ�Ƃ��̐ݒ�
                var button = obj.GetComponent<Button>();
                //�e�L�X�g�̐ݒ�
                var text = obj.GetComponentInChildren<Text>();
                if (i == 0)
                {
                    text.text = "���Y";
                    button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                }
                else
                {
                    text.text = "����";
                    button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                }
                //�{�^���������ꂽ�Ƃ��̏���
                owner._buttons.Add(obj);
            }
        }
        public override void OnUpdate(Blacksmith owner)
        {
            //�R���g���[���[�őI���ł���悤�ɂ���
            owner.Serect();
        }
        public override void OnExit(Blacksmith owner, UIState nextState)
        {
            owner.ButtonDelete();
        }
        public override void OnProceed(Blacksmith owner)
        {
            owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(Blacksmith owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<CanvasClose>();
        }
    }

    public class ProductionSelectMode : UIState
    {
        private RunOnce _runOnce;
        private Button _currntButton;

        public override void OnEnter(Blacksmith owner, UIState prevState)
        {
            owner._canvas.enabled = true;
            owner._popUp.enabled = false;

            //���[�h�I�����
            owner.ButtonDelete();
            owner._canvas.enabled = true;

            owner._blacksmithMode.text = "�������H";
            //�{�^���̒ǉ�
            for (int i = 0; i < 3/*���E���E�|*/; i++)
            {
                //�{�^���̈ʒu�ݒ�
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //�C���X�^���X��
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //�e�̐ݒ�
                obj.transform.parent = owner._buttonParent.transform;
                //�{�^���������ꂽ�Ƃ��̐ݒ�
                var button = obj.GetComponent<Button>();
                //�e�L�X�g�̐ݒ�
                var text = obj.GetComponentInChildren<Text>();
                switch (i)
                {
                    case 0:
                        text.text = "��";
                        owner.productionWeapon = (int)WeaponType.Axe;
                        button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                        break;
                    case 1:
                        text.text = "��";
                        owner.productionWeapon = (int)WeaponType.Spear;
                        button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                        break;
                    case 2:
                        text.text = "�|";
                        owner.productionWeapon = (int)WeaponType.Bow;
                        button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                        break;
                }
                //�{�^���������ꂽ�Ƃ��̏���
                owner._buttons.Add(obj);
            }
        }
        public override void OnUpdate(Blacksmith owner)
        {
            //�R���g���[���[�őI���ł���悤�ɂ���
            owner.Serect();
        }
        public override void OnExit(Blacksmith owner, UIState nextState)
        {
            owner.ButtonDelete();
        }
        public override void OnProceed(Blacksmith owner)
        {
            owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(Blacksmith owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<CanvasClose>();
        }
    }

    public class ProductionWeaponMode : UIState
    {
        private RunOnce _runOnce;
        private Button _currntButton;
        private bool _confirmation;
        public override void OnEnter(Blacksmith owner, UIState prevState)
        {
            owner._canvas.enabled = true;
            owner._popUp.enabled = false;

            //���[�h�I�����
            owner.ButtonDelete();
            owner._canvas.enabled = true;

            var data = GameManager.Instance;

            switch (owner.productionWeapon)
            {
                case 1:
                    owner._blacksmithMode.text = "����F��";
                    break;
                case 2:
                    owner._blacksmithMode.text = "����F��";
                    break;
                case 3:
                    owner._blacksmithMode.text = "����F�|";
                    break;
                default:
                    break;
            }

            //�{�^���̒ǉ�
            var villageData = SaveData.GetClass("Village", new VillageData());
            for (int i = 0; i < villageData.BlacksmithLevel; i++)
            {
                //�{�^���̈ʒu�ݒ�
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //�C���X�^���X��
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //�e�̐ݒ�
                obj.transform.parent = owner._buttonParent.transform;
                //�e�L�X�g�̐ݒ�
                var text = obj.GetComponentInChildren<Text>();
                //text.text = data.weaponName(owner.productionWeapon * 100 + i);
                //�{�^���������ꂽ�Ƃ��̐ݒ�
                var button = obj.GetComponent<Button>();
                //�{�^���������ꂽ�Ƃ��̏���
               // button.onClick.AddListener(/*boxHolder�ɕ���ǉ�*/);
                owner._buttons.Add(obj);
            }
        }
        public override void OnUpdate(Blacksmith owner)
        {
            if (_confirmation)
            {
                //�I���ł���悤�ɂ��Ă���
                float v = owner._inputAction.ReadValue<Vector2>().x;
                if (v < 0) _currntButton = owner._proceedButton;
                if (v > 0) _currntButton = owner._backButton;
                _currntButton.Select();
            }
            else
            {
                //�R���g���[���[�őI���ł���悤�ɂ���
                owner.Serect();
                var btn = owner._buttons[owner._currentButtonNumber].GetComponent<Button>();
                btn.onClick.Invoke();
                btn.Select();
            }
        }
        public override void OnExit(Blacksmith owner, UIState nextState)
        {
            owner.ButtonDelete();
        }
        public override void OnProceed(Blacksmith owner)
        {
            if (_confirmation)
            {
                _currntButton.onClick.Invoke();
            }
            else
            {
                _runOnce.Run(() =>
                {
                    owner._infoText.text = "���̃N�G�X�g���󒍂��܂���";
                    _confirmation = true;
                    owner._popUp.enabled = true;
                    owner._proceedButton.Select();
                });
            }
        }
        public override void OnBack(Blacksmith owner)
        {
            if (_confirmation)
            {
                _confirmation = false;
                _runOnce.Flg = false;
                owner._popUp.enabled = false;
                var btn = owner._buttons[owner._currentButtonNumber].GetComponent<Button>();
                btn.onClick.Invoke();
                btn.Select();
            }
            else
            {
                owner.ChangeState<ProductionSelectMode>();
            }
        }
    }

    public class EnhancementMode : UIState
    {

    }
}

