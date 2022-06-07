using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PartCheckerReceiver : MonoBehaviour
{
    [SerializeField] PartDate[] _partDatas;
    private Status _status;

    private void Start()
    {
        _status = GetComponent<Status>();
        foreach (var part in _partDatas)
        {
            part.EnduranceValue = part._maxEnduranceValue;
        }
    }

    public bool PartEnduranceDamage(PartType partType, int damage)
    {
        Debug.Log("部位ダメージ計算開始");
        var part = GetPartData(partType);
        if (part == null)
        {
            Debug.Log("部位が見つかりません");
            return false;
        }
        Debug.Log("部位  "+ part._partType);
        return part.Damage(damage,_status);
        

    }

    private PartDate GetPartData(PartType partType)
    {
        foreach (var part in _partDatas)
        {
            if (part._partType == PartType.notSet) continue;
            if (part._partType == partType) return part;
        }
        return null;
    }

    [Serializable]
    class PartDate
    {
        // 部位
        public PartType _partType;

        // 怯み値
        public int _maxEnduranceValue;

        // 現在の怯み値
        public int _enduranceValue;
        public int EnduranceValue { get => _enduranceValue; set => _enduranceValue = value; }

        public bool Damage(int _damage, Status _status)
        {
            _enduranceValue -= _damage;
            Debug.Log("部位ダメージ計算終了");

            if (_enduranceValue <= 0)
            {
                _enduranceValue = _maxEnduranceValue;
                if (_status.DownFlg) return false;//ダウン中に怯み値が0になってもダウンしない
                return true;
            }
            return false;
        }
    }


}
