using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

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
    private VillageData _villageData;
    private FadeManager _fadeManager;
    private UIItemView _iItemView;
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
    public UIItemView UIItemView { get => _iItemView; }
    public Player Player { get => _player; }
    public FadeManager FadeManager { get => _fadeManager;}

    protected override void Awake()
    {
        _quest = GetComponent<Quest>();
        _itemCanvas = GetComponent<ItemCanvas>();
        _MaterialDataList = GetComponent<MaterialDataList>();
        _ItemDataList = GetComponent<ItemDataList>();
        _UIPoachList = GetComponentInChildren<UIPoach>();
        _weaponDataList = GetComponent<WeaponDataList>();
        _enemyDataList = GetComponent<EnemyDataList>();
        _villageData = GetComponent<VillageData>();
        _fadeManager = GetComponentInChildren<FadeManager>();
        _iItemView = GetComponentInChildren<UIItemView>();
        base.Awake();
    }
    void Start()
    {
        _itemCanvas = GetComponent<ItemCanvas>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }


    void Update()
    {

    }

    public void SceneChange(Scene scene)
    {
        _nowScene = scene;
        _fadeManager.FadeOutStart(() => SceneManager.LoadScene((int)scene));
    }



}
public enum Scene
{
    Base,
    Forest,
    Animal,
    Sato
}
