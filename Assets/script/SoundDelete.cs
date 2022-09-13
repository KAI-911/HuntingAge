using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundDelete : MonoBehaviour
{
    [SerializeField] AudioSource _audio;
    [SerializeField] Sound _sound;
    void Start()
    {
        DontDestroyOnLoad(this);
        switch (_sound)
        {
            case Sound.SE:
                _audio.volume = UISoundManager.Instance.SEVolume;
                break;
            case Sound.BGM:
                _audio.volume = UISoundManager.Instance.BGMVolume;
                break;
        }
        
        Destroy(gameObject, _audio.clip.length);
    }
    private void Update()
    {
        
    }
    enum  Sound
    {
        SE,
        BGM
    }

}
