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
    [SerializeField] Text _blacksmithMode;
    //プレイヤーが近くまで来たか判断
    [SerializeField] TargetChecker _blacksmithChecker;
    //キャンバス
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _buttonParent;

    //確認用
    [SerializeField] Canvas _popUp;
    [SerializeField] Button _proceedButton;
    [SerializeField] Button _backButton;
    [SerializeField] Text _infoText;

    //レベル別クエストのまとまり
    [SerializeField] WeaponData _weaponData;

    //ボタンの配列
    [SerializeField] List<GameObject> _buttons;
    //選択中のボタン番号
    [SerializeField] int _currentButtonNumber;

    [SerializeField] WeaponData _WeaponData;
    private int productionWeapon;
    private enum WeaponType
    {
        Axe = 1,
        Spear,
        Bow
    }

    //インプットシステム
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
            //近くに来ている && 決定ボタンを押している && キャンバスがactiveでない
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

            //モード選択画面
            owner.ButtonDelete();
            owner._canvas.enabled = true;

            owner._blacksmithMode.text = "何をする？";

            //ボタンの追加
            for (int i = 0; i < 2/*生産、強化の二種のみのため*/; i++)
            {
                //ボタンの位置設定
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //インスタンス化
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //親の設定
                obj.transform.parent = owner._buttonParent.transform;
                //ボタンが押されたときの設定
                var button = obj.GetComponent<Button>();
                //テキストの設定
                var text = obj.GetComponentInChildren<Text>();
                if (i == 0)
                {
                    text.text = "生産";
                    button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                }
                else
                {
                    text.text = "強化";
                    button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                }
                //ボタンが押されたときの処理
                owner._buttons.Add(obj);
            }
        }
        public override void OnUpdate(Blacksmith owner)
        {
            //コントローラーで選択できるようにする
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

            //モード選択画面
            owner.ButtonDelete();
            owner._canvas.enabled = true;

            owner._blacksmithMode.text = "何を作る？";
            //ボタンの追加
            for (int i = 0; i < 3/*斧・槍・弓*/; i++)
            {
                //ボタンの位置設定
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //インスタンス化
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //親の設定
                obj.transform.parent = owner._buttonParent.transform;
                //ボタンが押されたときの設定
                var button = obj.GetComponent<Button>();
                //テキストの設定
                var text = obj.GetComponentInChildren<Text>();
                switch (i)
                {
                    case 0:
                        text.text = "斧";
                        owner.productionWeapon = (int)WeaponType.Axe;
                        button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                        break;
                    case 1:
                        text.text = "槍";
                        owner.productionWeapon = (int)WeaponType.Spear;
                        button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                        break;
                    case 2:
                        text.text = "弓";
                        owner.productionWeapon = (int)WeaponType.Bow;
                        button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                        break;
                }
                //ボタンが押されたときの処理
                owner._buttons.Add(obj);
            }
        }
        public override void OnUpdate(Blacksmith owner)
        {
            //コントローラーで選択できるようにする
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

            //モード選択画面
            owner.ButtonDelete();
            owner._canvas.enabled = true;

            var data = GameManager.Instance;

            switch (owner.productionWeapon)
            {
                case 1:
                    owner._blacksmithMode.text = "制作：斧";
                    break;
                case 2:
                    owner._blacksmithMode.text = "制作：槍";
                    break;
                case 3:
                    owner._blacksmithMode.text = "制作：弓";
                    break;
                default:
                    break;
            }

            //ボタンの追加
            var villageData = SaveData.GetClass("Village", new VillageData());
            for (int i = 0; i < villageData.BlacksmithLevel; i++)
            {
                //ボタンの位置設定
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //インスタンス化
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //親の設定
                obj.transform.parent = owner._buttonParent.transform;
                //テキストの設定
                var text = obj.GetComponentInChildren<Text>();
                //text.text = data.weaponName(owner.productionWeapon * 100 + i);
                //ボタンが押されたときの設定
                var button = obj.GetComponent<Button>();
                //ボタンが押されたときの処理
               // button.onClick.AddListener(/*boxHolderに武器追加*/);
                owner._buttons.Add(obj);
            }
        }
        public override void OnUpdate(Blacksmith owner)
        {
            if (_confirmation)
            {
                //選択できるようにしておく
                float v = owner._inputAction.ReadValue<Vector2>().x;
                if (v < 0) _currntButton = owner._proceedButton;
                if (v > 0) _currntButton = owner._backButton;
                _currntButton.Select();
            }
            else
            {
                //コントローラーで選択できるようにする
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
                    owner._infoText.text = "このクエストを受注しますか";
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

