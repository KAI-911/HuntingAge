using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class SettingDataList : MonoBehaviour
{
    public SettingSaveData _saveData;

    public float BGMVolume { get => _saveData.BGMVolume; set => _saveData.BGMVolume = value; }
    public float SEVolume { get => _saveData.SEVolume; set => _saveData.SEVolume = value; }
    public float UIVolume { get => _saveData.UIVolume; set => _saveData.UIVolume = value; }
    public float CameraVolume { get => _saveData.CameraVolume; set => _saveData.CameraVolume = value; }
    public float CameraMaxVolume { get => _saveData.CameraMaxVolume; set => _saveData.CameraMaxVolume = value; }
    public float CameraMinVolume { get => _saveData.CameraMinVolume; set => _saveData.CameraMinVolume = value; }


    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        Debug.Log(BGMVolume);
        Debug.Log(SEVolume);
        Debug.Log(UIVolume);
        Debug.Log(CameraVolume);
        Debug.Log(CameraMaxVolume);
        Debug.Log(CameraMinVolume);
    }
}
