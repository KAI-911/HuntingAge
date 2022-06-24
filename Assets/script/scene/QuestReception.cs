using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
public class QuestReception : MonoBehaviour
{
    ///�\������e�L�X�g
    [SerializeField] Text _questName;
    [SerializeField] Text _questContents;
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

    //���x���ʃN�G�X�g�̂܂Ƃ܂�
    [SerializeField] QuestHolder _questHolder;

    //�C���v�b�g�V�X�e��
    [SerializeField] private InputControls _input;
    private InputAction _inputAction;
    public InputAction InputAction { get => _inputAction; }


    [SerializeField] private RunOnce _buttonRunOnce;
    [SerializeField] private RunOnce _serectRunOnce;

    [SerializeField] private Vector3 firstButtonPos;
    [SerializeField] private float buttonBetween;

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

    void Update()
    {
        _currentState.OnUpdate(this);
    }

    public void SelectQuest_Rec(string QuestID)
    {
        GameManager.Instance.Quest.QusetSelect(QuestID);
        var data = GameManager.Instance.Quest.QuestData;
        string str = "";
        switch (data.Clear)
        {
            case ClearConditions.TargetSubjugation:
                Dictionary<string, int> list = new Dictionary<string, int>();
                foreach (var enemy in data.TargetName)
                {
                    //���ɒǉ�����Ă���ꍇ
                    if (list.ContainsKey(enemy))
                    {
                        list[enemy]++;
                    }
                    else
                    {
                        list.Add(enemy, 1);
                    }
                }

                str += "�N���A����: ";
                foreach (var item in list)
                {
                    var tmp = SaveData.GetClass<EnemyData>(item.Key, new EnemyData());

                    str += tmp.DisplayName + "��" + item.Value + "�̓�������\n";
                }
                break;
            case ClearConditions.Gathering:
                break;
            default:
                break;
        }
        str += "���s����: " + (int)(data.Failure + 1) + "��͐s����\n";
        switch (data.Field)
        {
            case Scene.Forest:
                str += "���: �X��";
                break;
            case Scene.Animal:
                str += "���: �����p";
                break;
            default:
                break;
        }
        _questName.text = data.Name;
        _questContents.text = str;

    }
    public void GoToQuest_Rec()
    {
        GameManager.Instance.Quest.GoToQuset();
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
        public virtual void OnEnter(QuestReception owner, UIState prevState)
        {

        }
        public virtual void OnUpdate(QuestReception owner)
        {

        }
        public virtual void OnExit(QuestReception owner, UIState nextState)
        {

        }
        public virtual void OnProceed(QuestReception owner)
        {

        }
        public virtual void OnBack(QuestReception owner)
        {

        }
    }
    [Serializable]
    public class CanvasClose : UIState
    {
        public override void OnEnter(QuestReception owner, UIState prevState)
        {
            owner.ButtonDelete();
            owner._canvas.enabled = false;
            owner._popUp.enabled = false;
        }
        public override void OnUpdate(QuestReception owner)
        {
        }
        public override void OnExit(QuestReception owner, UIState nextState)
        {

        }
        public override void OnProceed(QuestReception owner)
        {
            //�߂��ɗ��Ă��� && ����{�^���������Ă��� && �L�����o�X��active�łȂ�
            if (owner._questbordChecker.TriggerHit && !owner._canvas.enabled)
            {
                owner.ChangeState<LevelSerect>();
            }
        }
        public override void OnBack(QuestReception owner)
        {

        }
    }
    public class LevelSerect : UIState
    {
        public override void OnEnter(QuestReception owner, UIState prevState)
        {
            owner._canvas.enabled = true;
            owner._popUp.enabled = false;

            //���x���I�����
            owner.ButtonDelete();
            owner._currentButtonNumber = owner._questHolder.QuestLevel - 1;
            if (owner._currentButtonNumber < 0) owner._currentButtonNumber = 0;
            owner._questName.text = "";
            owner._questContents.text = "";
            owner._canvas.enabled = true;
            //�{�^���̒ǉ�
            var villageData = SaveData.GetClass("Village", new VillageData());
            for (int i = 0; i < villageData.VillageLevel; i++)
            {
                //�{�^���̈ʒu�ݒ�
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //�C���X�^���X��
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //�e�L�X�g�̐ݒ�
                var text = obj.GetComponentInChildren<Text>();
                text.text = "level" + (i + 1);
                //�e�̐ݒ�
                obj.transform.parent = owner._buttonParent.transform;
                //�{�^���������ꂽ�Ƃ��̐ݒ�
                var button = obj.GetComponent<Button>();
                int num = i + 1;
                string questHolder = "QuestHolder";
                questHolder += String.Format("{0:D3}", num);
                //�{�^���������ꂽ�Ƃ��̏���
                //_questHolder�ɏ�������QuestSerect�Ɉړ�
                button.onClick.AddListener(() => { owner._questHolder = SaveData.GetClass(questHolder, owner._questHolder); owner.ChangeState<QuestSerect>(); });
                owner._buttons.Add(obj);
            }

        }
        public override void OnUpdate(QuestReception owner)
        {
            //�R���g���[���[�őI���ł���悤�ɂ���
            owner.Serect();


        }
        public override void OnExit(QuestReception owner, UIState nextState)
        {
            owner.ButtonDelete();
        }
        public override void OnProceed(QuestReception owner)
        {
            owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(QuestReception owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<CanvasClose>();
        }
    }
    public class QuestSerect : UIState
    {
        private RunOnce _runOnce;
        private Button _currntButton;
        private bool _confirmation;
        public override void OnEnter(QuestReception owner, UIState prevState)
        {
            owner._canvas.enabled = true;
            owner._popUp.enabled = false;

            _runOnce = new RunOnce();
            owner.ButtonDelete();
            for (int i = 0; i < owner._questHolder.QuestDataID.Count; i++)
            {
                //�{�^���̈ʒu�ݒ�
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //�C���X�^���X��
                QuestData questData = SaveData.GetClass(owner._questHolder.QuestDataID[i], new QuestData());
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //�e�̐ݒ�
                obj.transform.parent = owner._buttonParent.transform;
                //�e�L�X�g�̐ݒ�
                var text = obj.GetComponentInChildren<Text>();
                text.text = questData.Name;
                //�{�^���������ꂽ�Ƃ��̐ݒ�
                var button = obj.GetComponent<Button>();
                button.onClick.AddListener(() => owner.SelectQuest_Rec(questData.ID));
                owner._buttons.Add(obj);
                _currntButton = owner._proceedButton;
            }

            if (owner._buttons.Count >= 1)
            {
                owner._currentButtonNumber = 0;
                owner._buttons[0].GetComponent<Button>().onClick.Invoke();
            }

            _confirmation = false;
            owner._proceedButton.onClick.RemoveAllListeners();
            owner._backButton.onClick.RemoveAllListeners();

            owner._proceedButton.onClick.AddListener(() => owner.ChangeState<QuestDecision>());
            owner._backButton.onClick.AddListener(() => owner._currentState.OnBack(owner));

        }
        public override void OnUpdate(QuestReception owner)
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
        public override void OnExit(QuestReception owner, UIState nextState)
        {
            owner.ButtonDelete();
            owner._popUp.enabled = false;

        }

        public override void OnProceed(QuestReception owner)
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

        public override void OnBack(QuestReception owner)
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
                owner.ChangeState<LevelSerect>();
            }
        }
    }
    public class QuestDecision : UIState
    {
        private Button _currntButton;

        public override void OnEnter(QuestReception owner, UIState prevState)
        {
            owner.ButtonDelete();
            owner._canvas.enabled = false;
            owner._popUp.enabled = false;
            _currntButton = owner._proceedButton;
        }
        public override void OnUpdate(QuestReception owner)
        {
            Debug.Log("�N�G�X�g�󒍒�");
            if (owner._popUp.enabled)
            {
                float v = owner._inputAction.ReadValue<Vector2>().x;
                if (v < 0) _currntButton = owner._proceedButton;
                if (v > 0) _currntButton = owner._backButton;
                _currntButton.Select();

            }
        }
        public override void OnExit(QuestReception owner, UIState nextState)
        {
            owner._popUp.enabled = false;
        }
        public override void OnProceed(QuestReception owner)
        {
            if (owner._questbordChecker.TriggerHit)
            {
                //�N�G�X�g�j�����邩�ǂ���
                if (!owner._popUp.enabled)
                {
                    owner._infoText.text = "���̃N�G�X�g��j�����܂���";
                    owner._popUp.enabled = true;
                    owner._proceedButton.onClick.RemoveAllListeners();
                    owner._backButton.onClick.RemoveAllListeners();
                    owner._proceedButton.onClick.AddListener(() => { owner.ChangeState<CanvasClose>(); owner._popUp.enabled = false; });
                    owner._backButton.onClick.AddListener(() => owner._popUp.enabled = false);
                }
                else
                {
                    _currntButton.onClick.Invoke();
                }
            }


            if (owner._gateChecker.TriggerHit)
            {
                owner.GoToQuest_Rec();
            }
        }
        public override void OnBack(QuestReception owner)
        {
            if (owner._questbordChecker.TriggerHit && owner._popUp.enabled)
            {
                owner._popUp.enabled = false;
            }
        }

    }

}
