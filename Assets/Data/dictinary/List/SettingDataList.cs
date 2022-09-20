using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingDataList : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] SettingDataObject DictionaryData;
    [SerializeField] float _BGMVolume;
    [SerializeField] float _SEVolume;
    [SerializeField] float _UIVolume;
    [SerializeField] float _CameraVolume;
    [SerializeField] float _CameraMaxVolume;
    [SerializeField] float _CameraMinVolume;
    [SerializeField] bool modifyValues;

    public float BGMVolume { get => _BGMVolume; set => _BGMVolume = value; }
    public float SEVolume { get => _SEVolume; set => _SEVolume = value; }
    public float UIVolume { get => _UIVolume; set => _UIVolume = value; }
    public float CameraVolume { get => _CameraVolume; set => _CameraVolume = value; }
    public float CameraMaxVolume { get => _CameraMaxVolume; set => _CameraMaxVolume = value; }
    public float CameraMinVolume { get => _CameraMinVolume; set => _CameraMinVolume = value; }

    private void Awake()
    {
        _BGMVolume = DictionaryData.BGMVolume;
        _SEVolume = DictionaryData.SEVolume;
        _UIVolume = DictionaryData.UIVolume;
        _CameraVolume = DictionaryData.CameraVolume;
        _CameraMaxVolume = DictionaryData.CameraMaxVolume;
        _CameraMinVolume = DictionaryData.CameraMinVolume;
    }
    public void OnBeforeSerialize()
    {
        if (!modifyValues)
        {
            _BGMVolume = DictionaryData.BGMVolume;
            _SEVolume = DictionaryData.SEVolume;
            _UIVolume = DictionaryData.UIVolume;
            _CameraVolume = DictionaryData.CameraVolume;
            _CameraMaxVolume = DictionaryData.CameraMaxVolume;
            _CameraMinVolume = DictionaryData.CameraMinVolume;

        }
    }

    public void OnAfterDeserialize()
    {

    }
    public void DesrializeDictionary()
    {
        Debug.Log("DesrializeDictionary");
        DictionaryData.BGMVolume = _BGMVolume;
        DictionaryData.SEVolume = _SEVolume;
        DictionaryData.UIVolume = _UIVolume;
        DictionaryData.CameraVolume = _CameraVolume;
        DictionaryData.CameraMaxVolume = _CameraMaxVolume;
        DictionaryData.CameraMinVolume = _CameraMinVolume;

        modifyValues = false;
    }

    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        Debug.Log(_BGMVolume);
        Debug.Log(_SEVolume);
        Debug.Log(_UIVolume);
        Debug.Log(_CameraVolume);
        Debug.Log(_CameraMaxVolume);
        Debug.Log(_CameraMinVolume);
    }
}
