using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] bool _chenge;
    [SerializeField] Scene _scene;
    [SerializeField] Quest _quest;
    [SerializeField] Scene _villageScene;
    [SerializeField] ItemHolder _itemBox;
    [SerializeField] ItemHolder _itemPoach;
    [SerializeField] QuestDataList _questDataList;
    [SerializeField] EnemyDataList _enemyDataList;
    [SerializeField] ItemDataList _itemDataList;
    [SerializeField] WeaponDataList _weaponDataList;

    public Scene VillageScene { get => _villageScene; }
    public Quest Quest { get => _quest; set => _quest = value; }
    public ItemHolder ItemBox { get => _itemBox; }
    public ItemHolder ItemPoach { get => _itemPoach; }
    public QuestDataList QuestDataList { get => _questDataList; }
    public EnemyDataList EnemyDataList { get => _enemyDataList; }
    public ItemDataList ItemDataList { get => _itemDataList; }
    public WeaponDataList WeaponDataList { get => _weaponDataList; }

    void Start()
    {
        _chenge = false;
    }

    void Update()
    {
        if (_chenge)
        {
            _chenge = false;
            SceneManager.LoadSceneAsync((int)_scene);
        }
    }

    public void SceneChange(Scene scene)
    {
        SceneManager.LoadSceneAsync((int)scene);
    }

    //[ContextMenu("SetItem_Box")]
    //private void SetItem_Box()
    //{
    //    _itemBox.ItemList.Clear();
    //    foreach (var item in SaveData.Keys())
    //    {
    //        if (item.StartsWith("Item") || item.StartsWith("Material"))
    //        {
    //            var data = SaveData.GetClass(item, new MaterialData());
    //            _itemBox.ItemList.Add(item, data.BoxHoldNumber);
    //        }
    //    }
    //    foreach (var item in _itemBox.ItemList)
    //    {
    //        Debug.Log(item);
    //    }
    //}



}
public enum Scene
{
    Base,
    Forest,
    Animal,
    Sato
}