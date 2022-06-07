using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] bool chenge;
    // Start is called before the first frame update
    void Start()
    {
        chenge = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (chenge) SceneManager.LoadSceneAsync((int)scene.Animal);
    }
    enum scene
    {
        Base,
        Forest,
        Animal
    }

}
