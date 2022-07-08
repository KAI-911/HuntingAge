using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quest : MonoBehaviour
{
    [SerializeField] string _nowState;
    //クエストデータ
    [SerializeField] QuestData _questData;
    public QuestData QuestData { get => _questData; set => _questData = value; }


    //倒した敵の種類と数を記録
    private EnemyCount _killEnemyCount = new EnemyCount();
    //倒すべき敵の種類と数を記録
    private EnemyCount _questTargetCount = new EnemyCount();

    [SerializeField] List<STRINGINT> viewKillEnemy = new List<STRINGINT>();
    [SerializeField] List<STRINGINT> viewQuestTargetEnemy = new List<STRINGINT>();

    //全ての敵の情報を記録
    [SerializeField] List<Enemy> _enemyList = new List<Enemy>();

    public List<Enemy> EnemyList { get => _enemyList; }
    public EnemyCount KillEnemyCount { get => _killEnemyCount;}


    //シーン切り替えまでの時間
    [SerializeField] float _sceneChengeTime;
    //プレイヤーの情報
    private Player _player;
    //このクエストで何回死んだか
    private int _deathCount;

    private RunOnce _runOnce;

    private QuestState _currentState;

    private void Awake()
    {
        _enemyList = new List<Enemy>();
        _runOnce = new RunOnce();
        _currentState = new Standby();
        _currentState.OnEnter(this, null);
    }

    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }



    void Update()
    {
        _nowState = _currentState.GetType().ToString();
        viewKillEnemy.Clear();
        viewQuestTargetEnemy.Clear();
        foreach (var item in KillEnemyCount.EnemyCountList)
        {
            var tmp = new STRINGINT();
            tmp.name = item.Key;
            tmp.number = item.Value;
            viewKillEnemy.Add(tmp);
        }
        foreach (var item in _questTargetCount.EnemyCountList)
        {
            var tmp = new STRINGINT();
            tmp.name = item.Key;
            tmp.number = item.Value;
            viewQuestTargetEnemy.Add(tmp);
        }
        _currentState.OnUpdate(this);
    }

    public void GoToQuset()
    {
        GameManager.Instance.SceneChange(_questData.Field);
    }

    private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        _currentState.OnActiveSceneChanged(this);
    }
    void LoadEnemy()
    {
        //討伐対象
        foreach (var target in _questData.TargetName)
        {

            Debug.Log(target.name + "    " + target.number);
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
        //非討伐対象
        foreach (var target in _questData.OtherName)
        {
            Debug.Log(target.name + "    " + target.number);
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


    void ChangeState<T>() where T : QuestState, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
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


    abstract class QuestState
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
            owner._player = GameObject.FindWithTag("Player").GetComponent<Player>();
            owner.KillEnemyCount.EnemyCountList.Clear();
            owner._questTargetCount.EnemyCountList.Clear();
            owner._enemyList.Clear();
            owner._deathCount = 0;
            owner.LoadEnemy();
            switch (owner._questData.Clear)
            {
                case ClearConditions.TargetSubjugation:
                    owner.ChangeState<TargetSubjugation>();
                    break;
                case ClearConditions.Gathering:
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
            bool clear = true;
            foreach (var item in owner._questTargetCount.EnemyCountList)
            {
                if(!owner.KillEnemyCount.EnemyCountList.ContainsKey(item.Key))
                {
                    clear = false;
                    break;
                }
                //クエストの討伐数以上倒していなかったら
                if (owner.KillEnemyCount.EnemyCountList[item.Key] < item.Value)
                {
                    clear = false;
                    break;
                }
            }

            if (clear)
            {
                owner.KillEnemyCount.EnemyDataSet();

                owner.ChangeState<QuestClear>();
                return;
            }

            if (owner._player.Status.HP == 0)
            {
                foreach (var ene in owner._enemyList) ene.DiscoverFlg = false;

                _ = owner._runOnce.WaitForAsync(2,
                    () =>
                    {
                        owner._deathCount++;
                        if (owner._deathCount >= (int)owner.QuestData.Failure + 1)
                        {
                            owner.ChangeState<QuestFailure>();
                        }
                        else
                        {
                            owner._player.Revival();
                            foreach (var ene in owner._enemyList) ene.DiscoverFlg = false;
                            owner._runOnce.Flg = false;//もう一度行えるように
                        }
                    }
                );
            }
        }
        public override void OnExit(Quest owner, QuestState nextState)
        {
            owner._questTargetCount.EnemyCountList.Clear();
            owner._killEnemyCount.EnemyCountList.Clear();
            owner.EnemyList.Clear();

        }
    }
    class QuestClear : QuestState
    {
        float time;
        public override void OnEnter(Quest owner, QuestState prevState)
        {
            time = owner._sceneChengeTime;
        }

        public override void OnUpdate(Quest owner)
        {
            Debug.Log("クリアしました");
            owner._player.Status.InvincibleFlg = true;
            time -= Time.deltaTime;
            if (time < 0) GameManager.Instance.SceneChange(GameManager.Instance.VillageScene);


        }
        public override void OnActiveSceneChanged(Quest owner)
        {


            owner.ChangeState<Standby>();
        }
    }
    class QuestFailure : QuestState
    {
        float time;
        public override void OnEnter(Quest owner, QuestState prevState)
        {
            time = owner._sceneChengeTime / 2;
        }

        public override void OnUpdate(Quest owner)
        {
            Debug.Log("失敗しました");
            owner._player.Status.InvincibleFlg = true;
            time -= Time.deltaTime;
            if (time < 0) GameManager.Instance.SceneChange(GameManager.Instance.VillageScene);
        }
        public override void OnActiveSceneChanged(Quest owner)
        {
            owner.ChangeState<Standby>();
        }
    }
}
