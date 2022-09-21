using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingDataStorage", menuName = "SettingDataObject")]
public class SettingDataObject : ScriptableObject
{
    public float BGMVolume;
    public float SEVolume;
    public float UIVolume;
    public float CameraVolume;
    public float CameraMaxVolume;
    public float CameraMinVolume;

}
