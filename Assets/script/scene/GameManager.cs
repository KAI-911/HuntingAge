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
    private ItemCanvas _itemCanvas;



    [SerializeField] ItemHolder _itemBox;
    [SerializeField] ItemHolder _itemPoach;
    [SerializeField] ItemDataList _itemDataList;

    [SerializeField] QuestHolder _questHolder;
    [SerializeField] QuestDataList _questDataList;

    [SerializeField] EnemyDataList _enemyDataList;

    [SerializeField] VillageData _villageData;

    [SerializeField] FadeManager _fadeManager;

    public Scene VillageScene { get => _villageScene; }
    public Quest Quest { get => _quest; set => _quest = value; }
    public ItemHolder ItemBox { get => _itemBox; }
    public ItemHolder ItemPoach { get => _itemPoach; }
    public QuestDataList QuestDataList { get => _questDataList; }
    public EnemyDataList EnemyDataList { get => _enemyDataList; }
    public ItemDataList ItemDataList { get => _itemDataList; }
    public QuestHolder QuestHolder { get => _questHolder; }
    public VillageData VillageData { get => _villageData; }
    public ItemCanvas ItemCanvas { get => _itemCanvas; }



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
        //SceneManager.LoadScene((int)scene);
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
