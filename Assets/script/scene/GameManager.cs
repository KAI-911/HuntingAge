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
    private PlayerStatusData _playerStatusData;
    private QuestDataList _questDataList;
    private QuestHolder _questHolderData;
    private SettingDataList _settingDataList;
    private VillageData _villageData;
    private FadeManager _fadeManager;
    private UIItemView_new _uiItemView;
    private Player _player;
    [SerializeField] private LookAtCamera _lookAtCamera;
    private bool _levelUp;
    private Report _report;
    private InitData _initData;
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
    public PlayerStatusData StatusData { get => _playerStatusData; }

    public bool LevelUp { get => _levelUp; set => _levelUp = value; }
    public Report Report { get => _report; }
    public LookAtCamera LookAtCamera { get => _lookAtCamera; }
    public string VillageDataPath { get => _villageDataPath; }
    public string SettingDataPath { get => _settingDataPath; }
    public string MaterialDataPath { get => _materialDataPath; }
    public string ItemDataPath { get => _itemDataPath; }
    public string QuestDataPath { get => _questDataPath; }
    public string WeponDataPath { get => _weponDataPath; }
    public string PlayerDataPath { get => _playerDataPath; }

    private string _villageDataPath;
    private string _settingDataPath;
    private string _materialDataPath;
    private string _itemDataPath;
    private string _questDataPath;
    private string _weponDataPath;
    private string _playerDataPath;
    protected override void Awake()
    {
        _villageDataPath = Application.persistentDataPath + "/villageData.json";
        _settingDataPath = Application.persistentDataPath + "/settingData.json";
        _materialDataPath = Application.persistentDataPath + "/materialData.json";
        _itemDataPath = Application.persistentDataPath + "/itemData.json";
        _questDataPath = Application.persistentDataPath + "/questData.json";
        _weponDataPath = Application.persistentDataPath + "/weponData.json";
        _playerDataPath = Application.persistentDataPath + "/playerData.json";
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
        _playerStatusData = GetComponent<PlayerStatusData>();
        _villageData = GetComponent<VillageData>();
        _fadeManager = GetComponentInChildren<FadeManager>();
        _uiItemView = GetComponentInChildren<UIItemView_new>();
        _report = GetComponentInChildren<Report>();
        _initData = GetComponent<InitData>();
        SceneManager.sceneLoaded += OnSceneLoaded;


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        base.Awake();
        _initData.InitSaveDataLoad();
        Load();

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

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
        Debug.Log("player"+Player.WeaponID);
        Debug.Log("saveplyer"+StatusData.PlayerSaveData.Wepon);
        Player.WeaponID = StatusData.PlayerSaveData.Wepon;

    }


    public void SceneChange(Scene scene)
    {
        _nowScene = scene;
        _fadeManager.FadeOutStart(() => SceneManager.LoadScene((int)scene));
    }
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        var obj = GameObject.FindGameObjectWithTag("LookTarget");
        if (obj != null)
        {
            LookAtCamera.LookAtTarget(obj.transform.position);
        }
    }
    public void Save()
    {
        JsonDataManager.Save(_villageData._saveData, _villageDataPath);

        JsonDataManager.Save(_settingDataList._saveData, _settingDataPath);

        JsonDataManager.Save(_playerStatusData.PlayerSaveData, _playerDataPath);

        _MaterialDataList._materialSaveData.SaveBefore();
        JsonDataManager.Save(_MaterialDataList._materialSaveData, _materialDataPath);

        ItemDataList._itemSaveData.SaveBefore();
        JsonDataManager.Save(ItemDataList._itemSaveData, _itemDataPath);

        QuestDataList._saveData.SaveBefore();
        JsonDataManager.Save(QuestDataList._saveData, _questDataPath);

        _weaponDataList._weponSaveData.SaveBefore();
        JsonDataManager.Save(_weaponDataList._weponSaveData, _weponDataPath);

    }
    public void Load()
    {
        Debug.Log(StatusData.PlayerSaveData.Wepon);

        //データ読み込み
        _villageData._saveData = JsonDataManager.Load<VillageSaveData>(_villageDataPath);

        _settingDataList._saveData = JsonDataManager.Load<SettingSaveData>(_settingDataPath);

        _playerStatusData.PlayerSaveData = JsonDataManager.Load<PlayerSaveData>(_playerDataPath);

        _MaterialDataList._materialSaveData = JsonDataManager.Load<MaterialSaveData>(_materialDataPath);
        _MaterialDataList._materialSaveData.LoadAfter();

        ItemDataList._itemSaveData = JsonDataManager.Load<ItemSaveData>(_itemDataPath);
        ItemDataList._itemSaveData.LoadAfter();

        QuestDataList._saveData = JsonDataManager.Load<QuestSaveData>(_questDataPath);
        QuestDataList._saveData.LoadAfter();

        _weaponDataList._weponSaveData = JsonDataManager.Load<WeponSaveData>(_weponDataPath);
        _weaponDataList._weponSaveData.LoadAfter();
        Debug.Log(StatusData.PlayerSaveData.Wepon);

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
