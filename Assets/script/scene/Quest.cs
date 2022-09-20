using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Data;
public class Quest : MonoBehaviour
{
    //�N�G�X�g�f�[�^
    [SerializeField] QuestData _questData;
    //�N�G�X�g��
    [SerializeField] bool _isQuest;
    //�N�G�X�g�o�ߎ���(s)
    [SerializeField] float _questTime;
    //�|�����G�̎�ނƐ����L�^
    private EnemyCount _killEnemyCount = new EnemyCount();
    //�|���ׂ��G�̎�ނƐ����L�^
    private EnemyCount _questTargetCount = new EnemyCount();
    //�̎悵���A�C�e�����L�^
    private GatheringCount _gatheringCount = new GatheringCount();
    //�S�Ă̓G�̏����L�^
    [SerializeField] List<Enemy> _enemyList = new List<Enemy>();
    [SerializeField] float imageAlpahSpeed;
    //UI�@�̗�
    private GameObject _HPBar;
    //UI�@�X�^�~�i
    private GameObject _SPBar;
    //UI�@�N�G�X�g���U���g
    private GameObject _result;

    public QuestData QuestData { get => _questData; set => _questData = value; }
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
            //�����Ώ�
            foreach (var target in _questData.TargetName)
            {

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
            //UI�̃Z�b�g
            owner._HPBar = Instantiate(Resources.Load("UI/Bar")) as GameObject;
            owner._SPBar = Instantiate(Resources.Load("UI/Bar")) as GameObject;
            owner._HPBar.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            owner._SPBar.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            //�̗̓o�[
            var hp = owner._HPBar.GetComponent<BarScript>();
            hp.SetMaxSlider(owner._player.Status.MaxHP);
            hp.SetSlisder(owner._player.Status.MaxHP);
            hp.SetFillColor(new Color(0, 1, 0, 1));
            hp.SetRectTransform(new Vector2(-Screen.width / 2 + Data.SCR.Padding, Screen.height / 2 - Data.SCR.Padding));
            //�X�^�~�i�o�[
            var sp = owner._SPBar.GetComponent<BarScript>();
            sp.SetMaxSlider(owner._player.Status.MaxSP);
            sp.SetSlisder(owner._player.Status.MaxSP);
            sp.SetFillColor(new Color(1, 1, 0, 1));
            sp.SetRectTransform(new Vector2(-Screen.width / 2 + Data.SCR.Padding, hp.GetRectTransform().y - hp.GetRectTransformSize().y - Data.SCR.Padding));
            //�v���C���[�̃N�G�X�g�J�n��
            GameManager.Instance.Player.Revival();
            //�A�C�e���̎g�p�󋵂̃��Z�b�g
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
            owner._questTime += Time.deltaTime;

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
            imageRect.sizeDelta = new Vector2(300, 50);
            textRect.sizeDelta = new Vector2(300, 50);
            var textText = text.GetComponent<Text>();
            textText.text = "����" + time + "�b�ő��ɖ߂�܂�";
            imageRect.anchoredPosition = new Vector2(-150, 25);
            textRect.anchoredPosition = new Vector2(-150, 25);

            once_1 = new RunOnce();
            once_2 = new RunOnce();

            //�N�G�X�g�N���A�t���O�𗧂Ă�
            var index = GameManager.Instance.QuestDataList.Keys.IndexOf(owner.QuestData.ID);
            var data = GameManager.Instance.QuestDataList.Values[index];
            data.ClearedFlg = true;
            GameManager.Instance.QuestDataList.Values[index] = data;
            GameManager.Instance.QuestDataList.DesrializeDictionary();

            //�L�[�N�G�X�g��S�ăN���A�����瑺���x�����グ��
            int level = GameManager.Instance.VillageData.VillageLevel;
            List<QuestData> questDatas = new List<QuestData>();
            bool levelup = true;
            var holdData = GameManager.Instance.QuestHolderData;
            for (int i = 0; i < holdData.Values[level - 1].Quests.Count; i++)
            {
                var tmpQuestData = GameManager.Instance.QuestDataList.Dictionary[holdData.Values[level - 1].Quests[i]];
                if (tmpQuestData.KeyQuestFlg && !tmpQuestData.ClearedFlg)
                {
                    levelup = false;
                    break;
                }
            }
            if (levelup && GameManager.Instance.VillageData.VillageLevel < 6)
            {
                GameManager.Instance.VillageData.VillageLevel++;
                GameManager.Instance.VillageData.KitchenLevel = GameManager.Instance.VillageData.VillageLevel;
                GameManager.Instance.VillageData.BlacksmithLevel = GameManager.Instance.VillageData.VillageLevel;
                GameManager.Instance.VillageData.DesrializeDictionary();
                Debug.Log("���x�����オ��܂���");
                GameManager.Instance.LevelUp = true;

            }

        }
        public override void OnExit(Quest owner, QuestState nextState)
        {
            Destroy(clearimage);
        }

        public override void OnUpdate(Quest owner)
        {
            Debug.Log("�N���A���܂���");

            time -= Time.deltaTime;
            //���b�Ŗ߂������
            if (time < owner._sceneChengeTime - 2)
            {
                once_1.Run(() =>
                {
                    Destroy(backimage);
                    Destroy(text);
                });
            }

            //quest�I�����Ƀ}�[�N�̕\��
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
                    clearRect.sizeDelta = new Vector2(600, 250);
                    clearRect.pivot = new Vector2(0.5f, 0.5f);
                    clearRect.anchoredPosition = new Vector2(0, clearRect.sizeDelta.y / 2);
                    Data.Convert.Correction(clearRect);
                });
                //���X�Ɍ�����悤�ɂ���
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
        GameObject failureimage;
        float time;
        public override void OnEnter(Quest owner, QuestState prevState)
        {
            time = 3;
            owner._isQuest = false;
            owner._player.Status.InvincibleFlg = true;

            failureimage = Instantiate(Resources.Load("UI/Image3")) as GameObject;
            failureimage.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            var s = failureimage.GetComponent<Image>();
            s.sprite = Resources.Load<Sprite>("Icon/questfailure");
            var color = s.color;
            color.a = 0;
            s.color = color;

            var failureRect = failureimage.GetComponent<RectTransform>();
            failureRect.sizeDelta = new Vector2(600, 250);
            failureRect.pivot = new Vector2(0.5f, 0.5f);
            failureRect.anchoredPosition = new Vector2(0, failureRect.sizeDelta.y / 2);
            Data.Convert.Correction(failureRect);


        }
        public override void OnExit(Quest owner, QuestState nextState)
        {
            Destroy(failureimage);
        }

        public override void OnUpdate(Quest owner)
        {
            Debug.Log("���s���܂���");
            time -= Time.deltaTime;

            //���X�Ɍ�����悤�ɂ���
            var s = failureimage.GetComponent<Image>();
            var color = s.color;
            color.a += owner.imageAlpahSpeed;
            color.a = Mathf.Clamp(color.a, 0, 1);
            s.color = color;

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
        public override void OnEnter(Quest owner, QuestState prevState)
        {
            if (owner._HPBar != null) Destroy(owner._HPBar);
            if (owner._SPBar != null) Destroy(owner._SPBar);

            GameManager.Instance.UIItemView.ChangeNotQuestState();

            //UI�̃Z�b�g
            owner._result = Instantiate(Resources.Load("UI/QuestResult")) as GameObject;
            //quest�N���A���ɂ̂ݕ�V�����炦��
            if (prevState.GetType() == typeof(QuestClear))
            {
                owner._result.GetComponent<UIQuestResult>().SetReward();
            }

            var tmp = owner._questData;
            tmp.Field = Scene.Base;
            owner._questData = tmp;

            //�|�[�`�ɂ���f�ރA�C�e�����{�b�N�X�ɑ���
            var dataList = GameManager.Instance.MaterialDataList;
            for (int i = 0; i < dataList.Dictionary.Count; i++)
            {
                var d = dataList.Values[i];
                d.BoxHoldNumber += d.PoachHoldNumber;
                d.PoachHoldNumber = 0;
                d.BoxHoldNumber = Mathf.Clamp(d.BoxHoldNumber, 0, d.BoxStackNumber);
                dataList.Values[i] = d;
            }
            dataList.DesrializeDictionary();
        }
        public override void OnExit(Quest owner, QuestState nextState)
        {
            //�󂯂Ă��Ȃ�����ID���󔒂ɂ���
            var data = owner.QuestData;
            data.ID = "";
            owner.QuestData = data;

            if (owner._result != null) Destroy(owner._result);
        }
        public override void OnActiveSceneChanged(Quest owner)
        {
            owner.ChangeState<Standby>();
        }
    }
    public void QuestRetire()
    {
        ChangeState<QuestFailure>();
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
