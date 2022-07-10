using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] Quest _quest;
    [SerializeField] Scene _villageScene;
    [SerializeField] Scene _nowScene;
    private ItemCanvas _itemCanvas;



    [SerializeField] ItemDataList _itemDataList;
    [SerializeField] UIPoach _UIPoachList;

    [SerializeField] WeaponDataList _weaponDataList;


    [SerializeField] EnemyDataList _enemyDataList;

    [SerializeField] VillageData _villageData;

    [SerializeField] FadeManager _fadeManager;

    public Scene VillageScene { get => _villageScene; }
    public Scene NowScene { get => _nowScene; set => _nowScene = value; }
    public Quest Quest { get => _quest; set => _quest = value; }
    public EnemyDataList EnemyDataList { get => _enemyDataList; }
    public ItemDataList ItemDataList { get => _itemDataList; }
    public WeaponDataList WeaponDataList { get => _weaponDataList; }
    public VillageData VillageData { get => _villageData; }
    public ItemCanvas ItemCanvas { get => _itemCanvas; }
    public UIPoach UIPoachList { get => _UIPoachList; set => _UIPoachList = value; }

    void Start()
    {
        _itemCanvas = GetComponent<ItemCanvas>();
        SceneManager.sceneLoaded += OnSceneLoaded;


    }


    void Update()
    {

    }

    public void SceneChange(Scene scene)
    {
        _nowScene = scene;
        _fadeManager.FadeOutStart(() => SceneManager.LoadScene((int)scene));
    }
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
    }



}
public enum Scene
{
    Base,
    Forest,
    Animal,
    Sato
}
