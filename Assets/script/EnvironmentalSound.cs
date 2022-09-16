using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalSound : MonoBehaviour
{
    private AudioSource audio;

    private TargetChecker target;

    [SerializeField] float intervalTime;

    private float keepIntervalTime;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        target = GetComponent<TargetChecker>();
        keepIntervalTime = intervalTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (audio.isPlaying) return;
        
        intervalTime -= Time.deltaTime;

        if(intervalTime < 0 && target.TriggerHit)
        {
            audio.volume = GameManager.Instance.SettingDataList.BGMVolume;
            audio.Play();
            intervalTime = keepIntervalTime;
        }
    }
}
