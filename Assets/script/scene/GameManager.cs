using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] bool chenge;
    [SerializeField] scene scene;
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
    public void SceneChange(scene scene)
    {
        SceneManager.LoadSceneAsync((int)scene);
    }

}
public enum scene
{
    Base,
    Forest,
    Animal
}