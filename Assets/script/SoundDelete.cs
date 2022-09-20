using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundDelete : MonoBehaviour
{
    [SerializeField] AudioSource _audio;
    [SerializeField] Sound _sound;
    [SerializeField]bool _remainSound;

    void Start()
    {
        switch (_sound)
        {
            case Sound.SE:
                _audio.volume = GameManager.Instance.SettingDataList.SEVolume;
               
                if (_remainSound == true)
                {
                    DontDestroyOnLoad(this);
                }
                break;
            case Sound.BGM:
                _audio.volume = GameManager.Instance.SettingDataList.BGMVolume;
                break;
            case Sound.UI:
                _audio.volume = GameManager.Instance.SettingDataList.UIVolume;
                break;

        }
        if (!_audio.loop) Destroy(gameObject, _audio.clip.length);
    }
    private void Update()
    {
        switch (_sound)
        {
            case Sound.SE:
                _audio.volume = GameManager.Instance.SettingDataList.SEVolume;
                break;
            case Sound.BGM:
                _audio.volume = GameManager.Instance.SettingDataList.BGMVolume;
                break;
            case Sound.UI:
                _audio.volume = GameManager.Instance.SettingDataList.UIVolume;
                break;

        }

    }
    enum Sound
    {
        SE,
        BGM,
        UI
    }

}
