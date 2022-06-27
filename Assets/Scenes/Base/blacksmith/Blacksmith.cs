//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.InputSystem;
//using System;
//using UnityEngine.EventSystems;
//public class Blacksmith : MonoBehaviour
//{
//    ///�\������e�L�X�g
//    [SerializeField] Text _weaponName;
//    [SerializeField] Text _weaponContents;
//    //�v���C���[���߂��܂ŗ��������f
//    [SerializeField] TargetChecker _blacksmithChecker;
//    //�L�����o�X
//    [SerializeField] Canvas _canvas;
//    [SerializeField] GameObject _buttonParent;

//    //�m�F�p
//    [SerializeField] Canvas _popUp;
//    [SerializeField] Button _proceedButton;
//    [SerializeField] Button _backButton;
//    [SerializeField] Text _infoText;

//    //�{�^���̔z��
//    [SerializeField] List<GameObject> _buttons;
//    //�I�𒆂̃{�^���ԍ�
//    [SerializeField] int _currentButtonNumber;

//    [SerializeField] WeaponData _WeaponData;
//    private int productionWeapon;

//    //�C���v�b�g�V�X�e��
//    [SerializeField] private InputControls _input;
//    private InputAction _inputAction;
//    public InputAction InputAction { get => _inputAction; }

//    // Start is called before the first frame update
//    private UIState _currentState;
//    private void Awake()
//    {
//        _input = new InputControls();
//        _buttonRunOnce = new RunOnce();
//        _serectRunOnce = new RunOnce();
//        _questHolder = new QuestHolder();
//        _currentState = new CanvasClose();
//        _currentState.OnEnter(this, null);
//    }
//    void Start()
//    {
//        _canvas.enabled = false;
//        _popUp.enabled = false;
//    }
//    private void OnEnable()
//    {
//        _inputAction = _input.UI.Selection;
//        _input.UI.Proceed.started += Proceed;

//        _input.UI.Back.started += Back;

//        _input.UI.Enable();
//    }

//    private void OnDisable()
//    {
//        _input.UI.Proceed.started -= Proceed;
//        _input.UI.Back.started -= Back;

//        _input.UI.Disable();
//    }
//    // Update is called once per frame
//    void Update()
//    {
//        _currentState.OnUpdate(this);
//        var s = _currentState.GetType();
//        Debug.Log(s);
//    }

//    public void ModeSelect()
//    {

//    }






//    public void ChangeState<T>() where T : UIState, new()
//    {
//        var nextState = new T();
//        _currentState.OnExit(this, nextState);
//        nextState.OnEnter(this, _currentState);
//        _currentState = nextState;
//    }

//    public abstract class UIState
//    {
//        public virtual void OnEnter(Blacksmith owner, UIState prevState)
//        {

//        }
//        public virtual void OnUpdate(Blacksmith owner)
//        {

//        }
//        public virtual void OnExit(Blacksmith owner, UIState nextState)
//        {

//        }
//        public virtual void OnProceed(Blacksmith owner)
//        {

//        }
//        public virtual void OnBack(Blacksmith owner)
//        {

//        }
//    }

//    [Serializable]
//    public class CanvasClose : UIState
//    {
//        public override void OnEnter(Blacksmith owner, UIState prevState)
//        {
//            owner.ButtonDelete();
//            owner._canvas.enabled = false;
//            owner._popUp.enabled = false;
//        }
//        public override void OnUpdate(Blacksmith owner)
//        { }
//        public override void OnExit(Blacksmith owner, UIState nextState)
//        { }
//        public override void OnProceed(Blacksmith owner)
//        {
//            //�߂��ɗ��Ă��� && ����{�^���������Ă��� && �L�����o�X��active�łȂ�
//            if (owner._blacksmithChecker.TriggerHit && !owner._canvas.enabled)
//            {
//                owner.ChangeState<SelectMode>();
//            }
//        }
//        public override void OnBack(Blacksmith owner)
//        { }
//    }
//    public class SelectMode : UIState
//    {
//        public override void OnEnter(QuestReception owner, UIState prevState)
//        {
//            owner._canvas.enabled = true;
//            owner._popUp.enabled = false;

//            //���[�h�I�����
//            owner.ButtonDelete();
//            owner._canvas.enabled = true;
//            //�{�^���̒ǉ�
//            for (int i = 0; i < 2/*���Y�A�����̓��݂̂̂���*/; i++)
//            {
//                //�{�^���̈ʒu�ݒ�
//                Vector3 pos = owner.firstButtonPos;
//                pos.y -= i * owner.buttonBetween;
//                //�C���X�^���X��
//                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
//                //�e�̐ݒ�
//                obj.transform.parent = owner._buttonParent.transform;
//                //�{�^���������ꂽ�Ƃ��̐ݒ�
//                var button = obj.GetComponent<Button>();
//                //�e�L�X�g�̐ݒ�
//                var text = obj.GetComponentInChildren<Text>();
//                if (i = 0)
//                {
//                    text.text = "���Y";
//                    button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
//                }
//                else
//                {
//                    text.text = "����";
//                    button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
//                }
//                //�{�^���������ꂽ�Ƃ��̏���
//                owner._buttons.Add(obj);
//            }
//        }
//        public override void OnUpdate(QuestReception owner)
//        {
//            //�R���g���[���[�őI���ł���悤�ɂ���
//            owner.Serect();
//        }
//        public override void OnExit(QuestReception owner, UIState nextState)
//        {
//            owner.ButtonDelete();
//        }
//        public override void OnProceed(QuestReception owner)
//        {
//            owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
//        }
//        public override void OnBack(QuestReception owner)
//        {
//            Debug.Log("modoru");
//            owner.ChangeState<CanvasClose>();
//        }
//    }
//    public class ProductionSelectMode : UIState
//    {
//        private RunOnce _runOnce;
//        private Button _currntButton;

//        private enum WeaponType
//        {
//            Axe = 1,
//            Spear,
//            Bow
//        }

//        public override void OnEnter(QuestReception owner, UIState prevState)
//        {
//            owner._canvas.enabled = true;
//            owner._popUp.enabled = false;

//            //���[�h�I�����
//            owner.ButtonDelete();
//            owner._canvas.enabled = true;
//            //�{�^���̒ǉ�
//            for (int i = 0; i < 3/*���E���E�|*/; i++)
//            {
//                //�{�^���̈ʒu�ݒ�
//                Vector3 pos = owner.firstButtonPos;
//                pos.y -= i * owner.buttonBetween;
//                //�C���X�^���X��
//                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
//                //�e�̐ݒ�
//                obj.transform.parent = owner._buttonParent.transform;
//                //�{�^���������ꂽ�Ƃ��̐ݒ�
//                var button = obj.GetComponent<Button>();
//                //�e�L�X�g�̐ݒ�
//                var text = obj.GetComponentInChildren<Text>();
//                switch (i)
//                {
//                    case 0:
//                        text.text = "��";
//                        productionWeapon = Axe;
//                        button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
//                    case 1:
//                        text.text = "��";
//                        productionWeapon = Spear;
//                        button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
//                    case 2:
//                        text.text = "�|";
//                        productionWeapon = Bow;
//                        button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
//                }
//                //�{�^���������ꂽ�Ƃ��̏���
//                owner._buttons.Add(obj);
//            }
//        }
//        public override void OnUpdate(QuestReception owner)
//        {
//            //�R���g���[���[�őI���ł���悤�ɂ���
//            owner.Serect();
//        }
//        public override void OnExit(QuestReception owner, UIState nextState)
//        {
//            owner.ButtonDelete();
//        }
//        public override void OnProceed(QuestReception owner)
//        {
//            owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
//        }
//        public override void OnBack(QuestReception owner)
//        {
//            Debug.Log("modoru");
//            owner.ChangeState<CanvasClose>();
//        }
//    }

//    public class ProductionWeaponMode : UIState
//    {
//        private RunOnce _runOnce;
//        private Button _currntButton;
//        public override void OnEnter(QuestReception owner, UIState prevState)
//        {
//            owner._canvas.enabled = true;
//            owner._popUp.enabled = false;

//            //���[�h�I�����
//            owner.ButtonDelete();
//            owner._canvas.enabled = true;
//            //�{�^���̒ǉ�
//            var villageData = SaveData.GetClass("Village", new VillageData());
//            for (int i = 0; i < villageData.BlacksmithLevel; i++)
//            {
//                //�{�^���̈ʒu�ݒ�
//                Vector3 pos = owner.firstButtonPos;
//                pos.y -= i * owner.buttonBetween;
//                //�C���X�^���X��
//                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
//                //�e�̐ݒ�
//                obj.transform.parent = owner._buttonParent.transform;
//                //�{�^���������ꂽ�Ƃ��̐ݒ�
//                var button = obj.GetComponent<Button>();
//                //�e�L�X�g�̐ݒ�
//                var text = obj.GetComponentInChildren<Text>();
//                //�{�^���������ꂽ�Ƃ��̏���
//                owner._buttons.Add(obj);
//            }
//        }
//        public override void OnUpdate(QuestReception owner)
//        {
//            //�R���g���[���[�őI���ł���悤�ɂ���
//            owner.Serect();
//        }
//        public override void OnExit(QuestReception owner, UIState nextState)
//        {
//            owner.ButtonDelete();
//        }
//        public override void OnProceed(QuestReception owner)
//        {
//            owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
//        }
//        public override void OnBack(QuestReception owner)
//        {
//            Debug.Log("modoru");
//            owner.ChangeState<CanvasClose>();
//        }
//    }


//    public class EnhancementMode : UIState
//    {

//    }
//}
