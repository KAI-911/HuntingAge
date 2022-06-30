using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
public class QuestReception : MonoBehaviour
{
    //キャンバス
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _buttonParent;
    ///表示するテキスト
    [SerializeField] Text _questName;
    [SerializeField] Text _questContents;

    //プレイヤーが近くまで来たか判断
    [SerializeField] TargetChecker _questbordChecker;
    [SerializeField] TargetChecker _gateChecker;

    //確認用
    [SerializeField] Confirmation _confirmation;


    //ボタンの配列
    //縦に並ぶ感じ
    [SerializeField] List<GameObject> _buttons;
    //選択中のボタン番号
    [SerializeField] int _currentButtonNumber;

    //レベル別クエストのまとまり
    [SerializeField] List<QuestDataReception> _questList;
    private int _currentQuestLevelNumber;

    //インプットシステム
    [SerializeField] private InputControls _input;
    private InputAction _inputAction;
    public InputAction InputAction { get => _inputAction; }

    [SerializeField] private RunOnce _buttonRunOnce;
    [SerializeField] private RunOnce _serectRunOnce;

    [SerializeField] private Vector3 firstButtonPos;
    [SerializeField] private float buttonBetween;

    [SerializeField] private bool buttonSetting;
    private UIState _currentState;

    private Player _player;
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _input = new InputControls();
        _buttonRunOnce = new RunOnce();
        _serectRunOnce = new RunOnce();
        _currentState = new CanvasClose();
        _currentState.OnEnter(this, null);

    }
    void Start()
    {
        _currentQuestLevelNumber = 0;
        _canvas.enabled = false;
        foreach (var item in GameManager.Instance.QuestHolder.Dictionary)
        {
            QuestDataReception tmp = new QuestDataReception();
            tmp.key = item.Key;
            tmp.questDatas = new List<QuestData>();
            foreach (var quests in item.Value.Quests)
            {
                tmp.questDatas.Add(GameManager.Instance.QuestDataList.Dictionary[quests]);
            }
            _questList.Add(tmp);
        }
    }

    private void OnEnable()
    {
        _inputAction = _input.UI.Selection;
        _input.UI.Proceed.started += Proceed;
        _input.UI.Proceed.canceled += Unlock;
        _input.UI.Back.started += Back;
        _input.UI.Back.canceled += Unlock;
        _input.UI.Enable();
        _currentQuestLevelNumber = 0;
    }



    private void OnDisable()
    {
        _input.UI.Proceed.started -= Proceed;
        _input.UI.Proceed.canceled -= Unlock;
        _input.UI.Back.started -= Back;
        _input.UI.Back.canceled -= Unlock;

        _input.UI.Disable();
    }


    void Update()
    {
        if (buttonSetting)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                Vector3 vector3 = firstButtonPos;
                vector3.y -= buttonBetween * i;
                _buttons[i].transform.position = vector3;
            }
        }

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


                str += "クリア条件: ";
                foreach (var item in data.TargetName)
                {
                    var tmp = GameManager.Instance.EnemyDataList.Dictionary[item.name];

                    str += tmp.DisplayName + "を" + item.number + "体討伐する\n";
                }
                break;
            case ClearConditions.Gathering:
                break;
            default:
                break;
        }
        str += "失敗条件: " + (int)(data.Failure + 1) + "回力尽きる\n";
        switch (data.Field)
        {
            case Scene.Forest:
                str += "狩場: 森林";
                break;
            case Scene.Animal:
                str += "狩場: 実験用";
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
            }
            else
            {
                _currentButtonNumber--;
            }
            _serectRunOnce.Flg = true;
            _currentButtonNumber = Math.Clamp(_currentButtonNumber, 0, _buttons.Count - 1);
            _buttons[_currentButtonNumber].GetComponent<Button>().Select();

        }
        else
        {
            _serectRunOnce.Flg = false;
        }



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
    private void Unlock(InputAction.CallbackContext obj)
    {
        Debug.Log("Unlock");
        _buttonRunOnce.Flg = false;
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
            owner._player.IsAction = true;
        }
        public override void OnUpdate(QuestReception owner)
        {
        }
        public override void OnExit(QuestReception owner, UIState nextState)
        {
        }
        public override void OnProceed(QuestReception owner)
        {
            //近くに来ている
            if (owner._questbordChecker.TriggerHit)
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
            owner._player.IsAction = false;

            owner._canvas.enabled = true;
            //レベル選択画面
            owner.ButtonDelete();
            owner._currentButtonNumber = owner._currentQuestLevelNumber;
            if (owner._currentButtonNumber < 0) owner._currentButtonNumber = 0;
            owner._questName.text = "";
            owner._questContents.text = "";
            owner._canvas.enabled = true;
            //ボタンの追加
            var villageData = GameManager.Instance.VillageData;
            for (int i = 0; i < Math.Min(villageData.VillageLevel, GameManager.Instance.QuestHolder.Dictionary.Count); i++)
            {
                //ボタンの位置設定
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //インスタンス化
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //テキストの設定
                var text = obj.GetComponentInChildren<Text>();
                text.text = "level" + (i + 1);
                //親の設定
                obj.transform.parent = owner._buttonParent.transform;
                //ボタンが押されたときの設定
                var button = obj.GetComponent<Button>();
                int num = i + 1;
                string questHolder = "QuestHolder";
                questHolder += String.Format("{0:D3}", num);
                //ボタンが押されたときの処理
                //_questHolderに情報を入れてQuestSerectに移動
                int level = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    owner._currentQuestLevelNumber = level;
                });
                owner._buttons.Add(obj);
            }
            owner._buttons[owner._currentButtonNumber].GetComponent<Button>().Select();

        }
        public override void OnUpdate(QuestReception owner)
        {
            //コントローラーで選択できるようにする
            owner.Serect();

        }
        public override void OnExit(QuestReception owner, UIState nextState)
        {
            owner.ButtonDelete();
        }
        public override void OnProceed(QuestReception owner)
        {
            owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
            owner.ChangeState<QuestSerect>();
        }
        public override void OnBack(QuestReception owner)
        {
            owner.ChangeState<CanvasClose>();
        }
    }
    public class QuestSerect : UIState
    {
        public override void OnEnter(QuestReception owner, UIState prevState)
        {
            owner._player.IsAction = false;

            owner.ButtonDelete();
            for (int i = 0; i < owner._questList[owner._currentQuestLevelNumber].questDatas.Count; i++)
            {
                //ボタンの位置設定
                Vector3 pos = owner.firstButtonPos;
                pos.y -= i * owner.buttonBetween;
                //インスタンス化
                QuestData questData = owner._questList[owner._currentQuestLevelNumber].questDatas[i];
                var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
                //親の設定
                obj.transform.parent = owner._buttonParent.transform;
                //テキストの設定
                var text = obj.GetComponentInChildren<Text>();
                text.text = questData.Name;
                //ボタンが押されたときの設定
                var button = obj.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    owner._confirmation.gameObject.SetActive(true);
                });
                owner._buttons.Add(obj);
            }


            //最初のクエストの内容を表示させるため
            if (owner._buttons.Count >= 1)
            {
                owner._currentButtonNumber = 0;
                var data = owner._questList[owner._currentQuestLevelNumber].questDatas[owner._currentButtonNumber];
                owner.SelectQuest_Rec(data.ID);
                owner._buttons[0].GetComponent<Button>().Select();
            }

            //確認用のボタンの関数の設定
            owner._confirmation.SetProceedButton(() =>
            {
                GameManager.Instance.Quest.AcceptingQuest = true;
                owner.ChangeState<QuestDecision>();
                owner._confirmation.gameObject.SetActive(false);
            });
            owner._confirmation.SetBackButton(() =>
            {
                GameManager.Instance.Quest.AcceptingQuest = false;
            });
            owner._confirmation.SetText("このクエストを受けますか");
        }
        public override void OnUpdate(QuestReception owner)
        {
            if (owner._confirmation.gameObject.activeSelf)
            {
                //コントローラーで選択できるようにする
                owner._confirmation.Selecte();

                //クエストの情報を見せる
                var data = owner._questList[owner._currentQuestLevelNumber].questDatas[owner._currentButtonNumber];
                owner.SelectQuest_Rec(data.ID);
            }
            else
            {
                //コントローラーで選択できるようにする
                owner.Serect();

                //クエストの情報を見せる
                var data = owner._questList[owner._currentQuestLevelNumber].questDatas[owner._currentButtonNumber];
                owner.SelectQuest_Rec(data.ID);
            }

        }
        public override void OnExit(QuestReception owner, UIState nextState)
        {
            owner.ButtonDelete();
        }

        public override void OnProceed(QuestReception owner)
        {
            if (owner._confirmation.gameObject.activeSelf == false)
            {
                owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
                owner._confirmation.gameObject.SetActive(true);
            }
        }

        public override void OnBack(QuestReception owner)
        {
            Debug.Log(owner._currentState.GetType());
            if (owner._confirmation.gameObject.activeSelf == false)
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
            owner._player.IsAction = true;

            owner.ButtonDelete();
            owner._canvas.enabled = false;
            owner._confirmation.gameObject.SetActive(false);

            owner._confirmation.SetProceedButton(() =>
            {
                if (owner._questbordChecker.TriggerHit)
                {
                    owner.ChangeState<CanvasClose>();
                    owner._confirmation.gameObject.SetActive(false);
                    GameManager.Instance.Quest.AcceptingQuest = false;
                    owner._player.IsAction = true;

                }
                else if (owner._gateChecker.TriggerHit)
                {
                    owner.GoToQuest_Rec();
                    owner._player.IsAction = true;
                }
            });
            owner._confirmation.SetBackButton(() =>
            {
                if (owner._questbordChecker.TriggerHit)
                {
                    owner._confirmation.gameObject.SetActive(false);
                    owner._player.IsAction = true;
                }
                else if (owner._gateChecker.TriggerHit)
                {
                    owner._confirmation.gameObject.SetActive(false);
                    owner._player.IsAction = true;

                }
            });

        }
        public override void OnUpdate(QuestReception owner)
        {
            if (owner._confirmation.gameObject.activeSelf)
            {
                owner._confirmation.Selecte();
            }
        }
        public override void OnExit(QuestReception owner, UIState nextState)
        {
            owner._confirmation.gameObject.SetActive(false);
        }
        public override void OnProceed(QuestReception owner)
        {
            if (owner._questbordChecker.TriggerHit)
            {
                owner._player.IsAction = false;

                owner._confirmation.SetText("このクエストを破棄しますか");
                owner._confirmation.gameObject.SetActive(true);
            }


            if (owner._gateChecker.TriggerHit)
            {
                owner._player.IsAction = false;

                owner._confirmation.SetText("クエストを出発しますか");
                owner._confirmation.gameObject.SetActive(true);
            }
        }
        public override void OnBack(QuestReception owner)
        {

        }

    }

    [Serializable]
    struct QuestDataReception
    {
        public string key;
        public List<QuestData> questDatas;
    }
}
