using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingDataList : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] SettingDataObject DictionaryData;
    [SerializeField] float _BGMVolume;
    [SerializeField] float _SEVolume;
    [SerializeField] float _UIVolume;
    [SerializeField] bool modifyValues;

    public float BGMVolume { get => _BGMVolume; set => _BGMVolume = value; }
    public float SEVolume { get => _SEVolume; set => _SEVolume = value; }
    public float UIVolume { get => _UIVolume; set => _UIVolume = value; }

    private void Awake()
    {
        _BGMVolume = DictionaryData.BGMVolume;
        _SEVolume = DictionaryData.SEVolume;
        _UIVolume = DictionaryData.UIVolume;
    }
    public void OnBeforeSerialize()
    {
        if (!modifyValues)
        {
            _BGMVolume = DictionaryData.BGMVolume;
            _SEVolume = DictionaryData.SEVolume;
            _UIVolume = DictionaryData.UIVolume;
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

        modifyValues = false;
    }

    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        Debug.Log(_BGMVolume);
        Debug.Log(_SEVolume);
        Debug.Log(_UIVolume);
    }
}
