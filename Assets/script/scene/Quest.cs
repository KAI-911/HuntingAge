using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Data;
public class Quest : MonoBehaviour
{
    //クエストデータ
    [SerializeField] QuestData _questData;
    //クエスト中
    [SerializeField] bool _isQuest;
    //クエスト経過時間(s)
    [SerializeField] float _questTime;
    //倒した敵の種類と数を記録
    private EnemyCount _killEnemyCount = new EnemyCount();
    //倒すべき敵の種類と数を記録
    private EnemyCount _questTargetCount = new EnemyCount();
    //採取したアイテムを記録
    private GatheringCount _gatheringCount = new GatheringCount();
    //全ての敵の情報を記録
    [SerializeField] List<Enemy> _enemyList = new List<Enemy>();
    [SerializeField] float imageAlpahSpeed;
    //UI　体力
    private GameObject _HPBar;
    //UI　スタミナ
    private GameObject _SPBar;
    //UI　クエストリザルト
    private GameObject _result;

    public QuestData QuestData { get => _questData; set => _questData = value; }
    public List<Enemy> EnemyList { get => _enemyList; }
    public EnemyCount KillEnemyCount { get => _killEnemyCount; }
    public bool IsQuest { get => _isQuest; }
    public GatheringCount GatheringCount { get => _gatheringCount; }



    //シーン切り替えまでの時間
    [SerializeField] float _sceneChengeTime;
    //プレイヤーの情報
    private Player _player;
    //このクエストで何回死んだか
    [SerializeField] private int _deathCount;
    public int DeathCount { get => _deathCount; set => _deathCount = value; }
    public float QuestTime { get => _questTime; }

    [SerializeField] float _delayTime;
    private float _delay;

    private QuestState _currentState;
    public QuestState CurrentState { get => _currentState; }

    private void Awake()
    {
        _questTime = 0;
        _enemyList = new List<Enemy>();
        _currentState = new Standby();
        _currentState.OnEnter(this, null);
        _isQuest = false;
        _delay = _delayTime;
    }

    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

    }
    void Update()
    {
        _currentState.OnUpdate(this);
        if (_HPBar != null && _SPBar != null)
        {
            var hp = _HPBar.GetComponent<BarScript>();
            hp.SetSlisder(_player.Status.HP);
            var sp = _SPBar.GetComponent<BarScript>();
            sp.SetSlisder(_player.Status.SP);
        }
    }
    public void GoToQuset()
    {
        _isQuest = true;
        GameManager.Instance.SceneChange(_questData.Field);
    }
    private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        _currentState.OnActiveSceneChanged(this);
    }
    void LoadEnemy()
    {
        if (_questData.Clear == ClearConditions.TargetSubjugation)
        {
            //討伐対象
            foreach (var target in _questData.TargetName)
            {

                _questTargetCount.Entry(target.name, target.number);
                for (int i = 0; i < target.number; i++)
                {
                    var data = GameManager.Instance.EnemyDataList.Dictionary[target.name];
                    Vector3 popPos = Vector3.zero;
                    //シーンごとの最初の位置を設定
                    popPos = data.EnemyPosition(_questData.Field).pos[Random.Range(0, data.EnemyPosition(_questData.Field).pos.Count)];
                    var obj = Instantiate(Resources.Load(data.InstanceName), popPos, Quaternion.identity) as GameObject;
                    var ene = obj.GetComponent<Enemy>();
                    ene.WaningPos = data.EnemyPosition(_questData.Field).pos;
                }
            }
        }

        //非討伐対象
        foreach (var target in _questData.OtherName)
        {
            for (int i = 0; i < target.number; i++)
            {
                var data = GameManager.Instance.EnemyDataList.Dictionary[target.name];
                Vector3 popPos = Vector3.zero;
                //シーンごとの最初の位置を設定
                popPos = data.EnemyPosition(_questData.Field).pos[Random.Range(0, data.EnemyPosition(_questData.Field).pos.Count)];
                var obj = Instantiate(Resources.Load(data.InstanceName), popPos, Quaternion.identity) as GameObject;
                var ene = obj.GetComponent<Enemy>();
                ene.WaningPos = data.EnemyPosition(_questData.Field).pos;
            }
        }

    }
    /// <summary>
    /// 出現した敵を管理するためにリストに加える
    /// </summary>
    /// <param name="enemy"></param>
    public void AddEnemy(Enemy enemy)
    {
        //同じ敵が追加されないようにする
        if (_enemyList.Find(n => n == enemy)) return;
        _enemyList.Add(enemy);
    }
    public void QuestReset()
    {
        ChangeState<Standby>();
        if (_HPBar != null) Destroy(_HPBar);
        if (_SPBar != null) Destroy(_SPBar);

    }
    void ChangeState<T>() where T : QuestState, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }
    public class QuestState
    {
        public virtual void OnEnter(Quest owner, QuestState prevState) { }
        public virtual void OnUpdate(Quest owner) { }
        public virtual void OnExit(Quest owner, QuestState nextState) { }
        public virtual void OnActiveSceneChanged(Quest owner) { }
    }
    class Standby : QuestState
    {
        public override void OnActiveSceneChanged(Quest owner)
        {
            owner._questTime = 0;
            owner.KillEnemyCount.EnemyCountList.Clear();
            owner._questTargetCount.EnemyCountList.Clear();
            owner._enemyList.Clear();
            owner._deathCount = 0;
            owner.LoadEnemy();
            //UIのセット
            owner._HPBar = Instantiate(Resources.Load("UI/Bar")) as GameObject;
            owner._SPBar = Instantiate(Resources.Load("UI/Bar")) as GameObject;
            owner._HPBar.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            owner._SPBar.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            //体力バー
            var hp = owner._HPBar.GetComponent<BarScript>();
            hp.SetMaxSlider(owner._player.Status.MaxHP);
            hp.SetSlisder(owner._player.Status.MaxHP);
            hp.SetFillColor(new Color(0, 1, 0, 1));
            hp.SetRectTransform(new Vector2(-Data.SCR.Width / 2 + Data.SCR.Padding, Data.SCR.Height / 2 - Data.SCR.Padding));
            //スタミナバー
            var sp = owner._SPBar.GetComponent<BarScript>();
            sp.SetMaxSlider(owner._player.Status.MaxSP);
            sp.SetSlisder(owner._player.Status.MaxSP);
            sp.SetFillColor(new Color(1, 1, 0, 1));
            sp.SetRectTransform(new Vector2(-Data.SCR.Width / 2 + Data.SCR.Padding, hp.GetRectTransform().y - hp.GetRectTransformSize().y - Data.SCR.Padding));
            //プレイヤーのクエスト開始時
            GameManager.Instance.Player.Revival();
            //アイテムの使用状況のリセット
            var itemDataList = GameManager.Instance.ItemDataList;
            for (int i = 0; i < itemDataList.Values.Count; i++)
            {
                var data = itemDataList.Values[i];
                data.Use = false;
                itemDataList.Values[i] = data;
            }
            itemDataList.DesrializeDictionary();
            switch (owner._questData.Clear)
            {
                case ClearConditions.TargetSubjugation:
                    owner.ChangeState<TargetSubjugation>();
                    break;
                case ClearConditions.Gathering:
                    owner.ChangeState<Gathering>();
                    break;
                default:
                    break;
            }
        }
    }
    class TargetSubjugation : QuestState
    {
        public override void OnUpdate(Quest owner)
        {
            owner._questTime += Time.deltaTime;
            if (owner.CheckPlayerDown()) return;


            foreach (var item in owner._questTargetCount.EnemyCountList)
            {
                if (!owner.KillEnemyCount.EnemyCountList.ContainsKey(item.Key)) return;
                //クエストの討伐数以上倒していなかったら
                if (owner.KillEnemyCount.EnemyCountList[item.Key] < item.Value) return;
            }

            owner.KillEnemyCount.EnemyDataSet();
            owner.ChangeState<QuestClear>();

        }
        public override void OnExit(Quest owner, QuestState nextState)
        {
            owner._questTargetCount.EnemyCountList.Clear();
            owner._killEnemyCount.EnemyCountList.Clear();
            owner._gatheringCount.CountList.Clear();
            owner.EnemyList.Clear();
        }
    }
    class Gathering : QuestState
    {
        public override void OnUpdate(Quest owner)
        {
            owner._questTime += Time.deltaTime;

            if (owner.CheckPlayerDown()) return;

            foreach (var item in owner._questData.TargetName)
            {
                if (!owner._gatheringCount.CountList.ContainsKey(item.name)) return;
                //クエストの採取数以上取っていなかったら
                if (owner._gatheringCount.CountList[item.name] < item.number) return;
            }
            owner.ChangeState<QuestClear>();

        }
        public override void OnExit(Quest owner, QuestState nextState)
        {
            owner._questTargetCount.EnemyCountList.Clear();
            owner._killEnemyCount.EnemyCountList.Clear();
            owner._gatheringCount.CountList.Clear();
            owner.EnemyList.Clear();
        }
    }
    class QuestClear : QuestState
    {
        GameObject backimage;
        GameObject text;
        RunOnce once_1;
        RunOnce once_2;
        GameObject clearimage;
        float time;
        public override void OnEnter(Quest owner, QuestState prevState)
        {
            time = owner._sceneChengeTime;
            owner._player.Status.InvincibleFlg = true;
            owner._isQuest = false;

            backimage = Instantiate(Resources.Load("UI/Image")) as GameObject;
            text = Instantiate(Resources.Load("UI/Text")) as GameObject;
            backimage.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            text.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            var imageRect = backimage.GetComponent<RectTransform>();
            var textRect = text.GetComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(300, 100);
            textRect.sizeDelta = new Vector2(300, 100);
            var textText = text.GetComponent<Text>();
            textText.text = "あと" + time + "秒で村に戻ります";
            imageRect.anchoredPosition = new Vector2(-150, -50);
            textRect.anchoredPosition = new Vector2(-150, -50);

            once_1 = new RunOnce();
            once_2 = new RunOnce();
        }
        public override void OnExit(Quest owner, QuestState nextState)
        {
            Destroy(clearimage);
        }

        public override void OnUpdate(Quest owner)
        {
            Debug.Log("クリアしました");

            time -= Time.deltaTime;
            //何秒で戻るを消す
            if (time < owner._sceneChengeTime - 2)
            {
                once_1.Run(() =>
                {
                    Destroy(backimage);
                    Destroy(text);
                });
            }

            //quest終了時にマークの表示
            if (time < 3)
            {
                once_2.Run(() =>
                {
                    clearimage = Instantiate(Resources.Load("UI/Image3")) as GameObject;
                    clearimage.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
                    var s = clearimage.GetComponent<Image>();
                    s.sprite = Resources.Load<Sprite>("Icon/questclear");
                    var color = s.color;
                    color.a = 0;
                    s.color = color;

                    var clearRect = clearimage.GetComponent<RectTransform>();
                    clearRect.sizeDelta = new Vector2(600,250);
                    clearRect.anchoredPosition = new Vector2(-clearRect.sizeDelta.x / 2, clearRect.sizeDelta.y / 2);

                });
                //徐々に見えるようにする
                var s = clearimage.GetComponent<Image>();
                var color = s.color;
                color.a += owner.imageAlpahSpeed;
                color.a = Mathf.Clamp(color.a, 0, 1);
                s.color = color;

            }
            if (time < 0)
            {
                GameManager.Instance.FadeManager.FadeOutStart(() =>
                {
                    owner.ChangeState<QuestResult>();
                    GameManager.Instance.FadeManager.FadeInStart();
                });
            }

        }
    }
    class QuestFailure : QuestState
    {
        GameObject backimage;
        GameObject text;
        RunOnce once_1;
        RunOnce once_2;
        GameObject failureimage;

        float time;
        public override void OnEnter(Quest owner, QuestState prevState)
        {
            time = owner._sceneChengeTime / 2;
            owner._isQuest = false;
            owner._player.Status.InvincibleFlg = true;

            backimage = Instantiate(Resources.Load("UI/Image")) as GameObject;
            text = Instantiate(Resources.Load("UI/Text")) as GameObject;
            backimage.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            text.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            var imageRect = backimage.GetComponent<RectTransform>();
            var textRect = text.GetComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(300, 100);
            textRect.sizeDelta = new Vector2(300, 100);
            var textText = text.GetComponent<Text>();
            textText.text = "あと" + time + "秒で村に戻ります";
            imageRect.anchoredPosition = new Vector2(-150, -50);
            textRect.anchoredPosition = new Vector2(-150, -50);

            once_1 = new RunOnce();
            once_2 = new RunOnce();

        }

        public override void OnUpdate(Quest owner)
        {
            Debug.Log("失敗しました");

            if (time < (owner._sceneChengeTime / 2) - 2)
            {
                once_1.Run(() =>
                {
                    Destroy(backimage);
                    Destroy(text);
                });
            }

            if (time < 3)
            {
                once_2.Run(() =>
                {
                    failureimage = Instantiate(Resources.Load("UI/Image3")) as GameObject;
                    failureimage.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
                    var s = failureimage.GetComponent<Image>();
                    s.sprite = Resources.Load<Sprite>("Icon/questfailure");
                    var color = s.color;
                    color.a = 0;
                    s.color = color;

                    var clearRect = failureimage.GetComponent<RectTransform>();
                    clearRect.sizeDelta = new Vector2(600, 250);
                    clearRect.anchoredPosition = new Vector2(-clearRect.sizeDelta.x / 2, clearRect.sizeDelta.y / 2);
                });
                //徐々に見えるようにする
                var s = failureimage.GetComponent<Image>();
                var color = s.color;
                color.a += owner.imageAlpahSpeed;
                color.a = Mathf.Clamp(color.a, 0, 1);
                s.color = color;

            }

            time -= Time.deltaTime;
            //何秒で戻るを消す
            if (time < owner._sceneChengeTime - 2)
            {
                once_1.Run(() =>
                {
                    Destroy(backimage);
                    Destroy(text);
                });
            }
            //quest終了時にマークの表示
            if (time < 3)
            {
                once_2.Run(() =>
                {
                    failureimage = Instantiate(Resources.Load("UI/Image")) as GameObject;
                    failureimage.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
                    var s = failureimage.GetComponent<Image>();
                    s.sprite = Resources.Load<Sprite>("Icon/questclear");
                    var color = s.color;
                    color.a = 0;
                    s.color = color;
                });
                //徐々に見えるようにする
                var s = failureimage.GetComponent<Image>();
                var color = s.color;
                color.a += owner.imageAlpahSpeed;
                color.a = Mathf.Clamp(color.a, 0, 1);
                s.color = color;

            }

            if (time < 0)
            {
                GameManager.Instance.FadeManager.FadeOutStart(() =>
                {
                    owner.ChangeState<QuestResult>();
                    GameManager.Instance.FadeManager.FadeInStart();
                });
            }
        }
    }
    class QuestResult : QuestState
    {
        UIQuestResult _questResult;
        public override void OnEnter(Quest owner, QuestState prevState)
        {
            if (owner._HPBar != null) Destroy(owner._HPBar);
            if (owner._SPBar != null) Destroy(owner._SPBar);
            GameManager.Instance.UIItemView.ChangeNotQuestState();

            //UIのセット
            owner._result = Instantiate(Resources.Load("UI/QuestResult")) as GameObject;
            _questResult = owner._result.GetComponent<UIQuestResult>();
        }
        public override void OnExit(Quest owner, QuestState nextState)
        {
            if (owner._result != null) Destroy(owner._result);
        }
        public override void OnActiveSceneChanged(Quest owner)
        {
            owner.ChangeState<Standby>();
        }
    }


    private bool CheckPlayerDown()
    {
        if (_player.Status.HP <= 0)
        {
            foreach (var ene in _enemyList) ene.DiscoverFlg = false;
            _delay -= Time.deltaTime;
            if (_delay < 0)
            {
                if (_deathCount >= (int)QuestData.Failure + 1)
                {
                    _delay = _delayTime;
                    ChangeState<QuestFailure>();
                    return true;
                }
                else
                {
                    _delay = _delayTime;
                    _player.Revival();
                    foreach (var ene in _enemyList) ene.DiscoverFlg = false;
                }

            }
        }
        return false;
    }
}
