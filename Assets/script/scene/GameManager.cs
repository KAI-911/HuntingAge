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
    public Scene VillageScene { get => _villageScene; }
    public Quest Quest { get => _quest; set => _quest = value; }
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


}
public enum Scene
{
    Base,
    Forest,
    Animal,
    Sato
}