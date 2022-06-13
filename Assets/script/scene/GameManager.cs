using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] bool chenge;
    [SerializeField] Scene scene;
    void Start()
    {
        chenge = false;
    }

    void Update()
    {
        if (chenge)
        {
            chenge = false;
            SceneManager.LoadSceneAsync((int)scene);
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
    Animal
}