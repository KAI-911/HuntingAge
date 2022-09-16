using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] Scene _villageScene;
    [SerializeField] Scene _nowScene;

    private Quest _quest;
    private ItemCanvas _itemCanvas;
    private MaterialDataList _MaterialDataList;
    private ItemDataList _ItemDataList;
    private UIPoach _UIPoachList;
    private WeaponDataList _weaponDataList;
    private EnemyDataList _enemyDataList;
    private QuestDataList _questDataList;
    private QuestHolder _questHolderData;
    private SettingDataList _settingDataList;
    private VillageData _villageData;
    private FadeManager _fadeManager;
    private UIItemView_new _uiItemView;
    private Player _player;
    public Scene VillageScene { get => _villageScene; }
    public Scene NowScene { get => _nowScene; set => _nowScene = value; }
    public Quest Quest { get => _quest; set => _quest = value; }
    public EnemyDataList EnemyDataList { get => _enemyDataList; }
    public MaterialDataList MaterialDataList { get => _MaterialDataList; }
    public ItemDataList ItemDataList { get => _ItemDataList; }
    public WeaponDataList WeaponDataList { get => _weaponDataList; }
    public VillageData VillageData { get => _villageData; }
    public ItemCanvas ItemCanvas { get => _itemCanvas; }
    public UIPoach UIPoachList { get => _UIPoachList; }
    public UIItemView_new UIItemView { get => _uiItemView; }
    public Player Player { get => _player; }
    public FadeManager FadeManager { get => _fadeManager; }
    public SettingDataList SettingDataList { get => _settingDataList; }
    public QuestDataList QuestDataList { get => _questDataList; }
    public QuestHolder QuestHolderData { get => _questHolderData; }

    protected override void Awake()
    {
        _quest = GetComponent<Quest>();
        _itemCanvas = GetComponent<ItemCanvas>();
        _MaterialDataList = GetComponent<MaterialDataList>();
        _ItemDataList = GetComponent<ItemDataList>();
        _UIPoachList = GetComponentInChildren<UIPoach>();
        _weaponDataList = GetComponent<WeaponDataList>();
        _enemyDataList = GetComponent<EnemyDataList>();
        _questDataList = GetComponent<QuestDataList>();
        _questHolderData = GetComponent<QuestHolder>();
        _settingDataList = GetComponent<SettingDataList>();
        _villageData = GetComponent<VillageData>();
        _fadeManager = GetComponentInChildren<FadeManager>();
        _uiItemView = GetComponentInChildren<UIItemView_new>();
        base.Awake();
    }
    public void GoToVillage()
    {
        if (_nowScene == Scene.Title)
        {
            _nowScene = VillageScene;
            UISoundManager.Instance.PlayQuestSE();
            _fadeManager.FadeOutStart(() =>
            {
                _fadeManager.FadeInStart();
                SceneManager.LoadScene((int)VillageScene);
                _quest.QuestReset();
                _player.ChangeState<VillageState>();
            });

        }
    }

    void Start()
    {
        _itemCanvas = GetComponent<ItemCanvas>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }


    public void SceneChange(Scene scene)
    {
        _nowScene = scene;
        _fadeManager.FadeOutStart(() => SceneManager.LoadScene((int)scene));
    }



}
public enum Scene
{
    Title,
    Forest,
    Animal,
    Sato,
    Hama,
    Shimi,
    Base
}
