using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Data;
public class Quest : MonoBehaviour
{
    //�N�G�X�g�f�[�^
    [SerializeField] QuestData _questData;
    public QuestData QuestData { get => _questData; set => _questData = value; }

    [SerializeField] bool _isQuest;

    //�|�����G�̎�ނƐ����L�^
    private EnemyCount _killEnemyCount = new EnemyCount();
    //�|���ׂ��G�̎�ނƐ����L�^
    private EnemyCount _questTargetCount = new EnemyCount();
    //�̎悵���A�C�e�����L�^
    private GatheringCount _gatheringCount = new GatheringCount();
    //�S�Ă̓G�̏����L�^
    [SerializeField] List<Enemy> _enemyList = new List<Enemy>();

    //UI
    private GameObject _HPBar;
    private GameObject _SPBar;

    public List<Enemy> EnemyList { get => _enemyList; }
    public EnemyCount KillEnemyCount { get => _killEnemyCount; }
    public bool IsQuest { get => _isQuest; }
    public GatheringCount GatheringCount { get => _gatheringCount; }



    //�V�[���؂�ւ��܂ł̎���
    [SerializeField] float _sceneChengeTime;
    //�v���C���[�̏��
    private Player _player;
    //���̃N�G�X�g�ŉ��񎀂񂾂�
    [SerializeField] private int _deathCount;
    public int DeathCount { get => _deathCount; set => _deathCount = value; }

    [SerializeField] float _delayTime;
    private float _delay;

    private QuestState _currentState;

    private void Awake()
    {
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
        if(_HPBar!=null&&_SPBar!=null)
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
            //�����Ώ�
            foreach (var target in _questData.TargetName)
            {

                Debug.Log(target.name + "    " + target.number);
                _questTargetCount.Entry(target.name, target.number);
                for (int i = 0; i < target.number; i++)
                {
                    var data = GameManager.Instance.EnemyDataList.Dictionary[target.name];
                    Vector3 popPos = Vector3.zero;
                    //�V�[�����Ƃ̍ŏ��̈ʒu��ݒ�
                    popPos = data.EnemyPosition(_questData.Field).pos[Random.Range(0, data.EnemyPosition(_questData.Field).pos.Count)];
                    var obj = Instantiate(Resources.Load(data.InstanceName), popPos, Quaternion.identity) as GameObject;
                    var ene = obj.GetComponent<Enemy>();
                    ene.WaningPos = data.EnemyPosition(_questData.Field).pos;
                }
            }
        }

        //�񓢔��Ώ�
        foreach (var target in _questData.OtherName)
        {
            for (int i = 0; i < target.number; i++)
            {
                var data = GameManager.Instance.EnemyDataList.Dictionary[target.name];
                Vector3 popPos = Vector3.zero;
                //�V�[�����Ƃ̍ŏ��̈ʒu��ݒ�
                popPos = data.EnemyPosition(_questData.Field).pos[Random.Range(0, data.EnemyPosition(_questData.Field).pos.Count)];
                var obj = Instantiate(Resources.Load(data.InstanceName), popPos, Quaternion.identity) as GameObject;
                var ene = obj.GetComponent<Enemy>();
                ene.WaningPos = data.EnemyPosition(_questData.Field).pos;
            }
        }

    }
    /// <summary>
    /// �o�������G���Ǘ����邽�߂Ƀ��X�g�ɉ�����
    /// </summary>
    /// <param name="enemy"></param>
    public void AddEnemy(Enemy enemy)
    {
        //�����G���ǉ�����Ȃ��悤�ɂ���
        if (_enemyList.Find(n => n == enemy)) return;
        _enemyList.Add(enemy);
    }
    void ChangeState<T>() where T : QuestState, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
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
            //owner._player = GameObject.FindWithTag("Player").GetComponent<Player>();
            owner.KillEnemyCount.EnemyCountList.Clear();
            owner._questTargetCount.EnemyCountList.Clear();
            owner._enemyList.Clear();
            owner._deathCount = 0;
            owner.LoadEnemy();
            //UI�̃Z�b�g
            owner._HPBar = Instantiate(Resources.Load("UI/Bar")) as GameObject;
            owner._SPBar = Instantiate(Resources.Load("UI/Bar")) as GameObject;
            owner._HPBar.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            owner._SPBar.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            var hp = owner._HPBar.GetComponent<BarScript>();
            hp.SetMaxSlider(owner._player.Status.MaxHP);
            hp.SetSlisder(owner._player.Status.MaxHP);
            hp.SetFillColor(new Color(0, 1, 0, 1));
            hp.SetRectTransform(new Vector2(-Data.SCR.Width / 2 + Data.SCR.Padding, Data.SCR.Height/2 - Data.SCR.Padding));
            var sp = owner._SPBar.GetComponent<BarScript>();
            sp.SetMaxSlider(owner._player.Status.MaxSP);
            sp.SetSlisder(owner._player.Status.MaxSP);
            sp.SetFillColor(new Color(1, 1, 0, 1));
            sp.SetRectTransform(new Vector2(-Data.SCR.Width / 2 + Data.SCR.Padding, hp.GetRectTransform().y - hp.GetRectTransformSize().y - Data.SCR.Padding));
            GameManager.Instance.Player.Revival();
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

            if (owner.CheckPlayerDown()) return;


            foreach (var item in owner._questTargetCount.EnemyCountList)
            {
                if (!owner.KillEnemyCount.EnemyCountList.ContainsKey(item.Key)) return;
                //�N�G�X�g�̓������ȏ�|���Ă��Ȃ�������
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

            if (owner.CheckPlayerDown()) return;

            foreach (var item in owner._questData.TargetName)
            {
                if (!owner._gatheringCount.CountList.ContainsKey(item.name)) return;
                //�N�G�X�g�̍̎搔�ȏ����Ă��Ȃ�������
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
        float time;
        public override void OnEnter(Quest owner, QuestState prevState)
        {
            time = owner._sceneChengeTime;
            owner._player.Status.InvincibleFlg = true;
            owner._isQuest = false;
        }

        public override void OnUpdate(Quest owner)
        {
            Debug.Log("�N���A���܂���");

            time -= Time.deltaTime;
            if (time < 0)
            {

                GameManager.Instance.SceneChange(GameManager.Instance.VillageScene);
            }

        }
        public override void OnActiveSceneChanged(Quest owner)
        {
            Destroy(owner._HPBar);
            Destroy(owner._SPBar);
            owner.ChangeState<Standby>();
        }
    }
    class QuestFailure : QuestState
    {
        float time;
        public override void OnEnter(Quest owner, QuestState prevState)
        {
            time = owner._sceneChengeTime / 2;
            owner._isQuest = false;
            owner._player.Status.InvincibleFlg = true;
        }

        public override void OnUpdate(Quest owner)
        {
            Debug.Log("���s���܂���");

            time -= Time.deltaTime;
            if (time < 0)
            {

                GameManager.Instance.SceneChange(GameManager.Instance.VillageScene);
            }
        }
        public override void OnActiveSceneChanged(Quest owner)
        {
            Destroy(owner._HPBar);
            Destroy(owner._SPBar);
            owner.ChangeState<Standby>();
        }
    }


    private bool CheckPlayerDown()
    {
        Debug.Log("C1");
        if (_player.Status.HP <= 0)
        {
            foreach (var ene in _enemyList) ene.DiscoverFlg = false;
            Debug.Log("C2");
            _delay -= Time.deltaTime;
            if(_delay<0)
            {
                Debug.Log("C3");
                if (_deathCount >= (int)QuestData.Failure + 1)
                {
                    Debug.Log("C4");
                    _delay = _delayTime;
                    ChangeState<QuestFailure>();
                    return true;
                }
                else
                {
                    Debug.Log("C5");
                    _delay = _delayTime;
                    _player.Revival();
                    foreach (var ene in _enemyList) ene.DiscoverFlg = false;
                }

            }
        }
        return false;
    }
}
