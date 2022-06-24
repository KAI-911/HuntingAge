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
            part.EnduranceValue = part.MaxEnduranceValue;
        }
    }

    public bool PartEnduranceDamage(PartType partType, int damage)
    {
        var part = GetPartData(partType);
        if (part == null)
        {
            Debug.Log("部位が見つかりません");
            return false;
        }
        return part.Damage(damage,_status);
        

    }

    private PartDate GetPartData(PartType partType)
    {
        foreach (var part in _partDatas)
        {
            if (part.PartType == PartType.notSet) continue;
            if (part.PartType == partType) return part;
        }
        return null;
    }

    [Serializable]
    class PartDate
    {
        // 部位
        [SerializeField] private PartType _partType;
        public PartType PartType { get => _partType; set => _partType = value; }

        // 怯み値
        [SerializeField] private int _maxEnduranceValue;
        public int MaxEnduranceValue { get => _maxEnduranceValue; set => _maxEnduranceValue = value; }

        // 現在の怯み値
        private int _enduranceValue;
        public int EnduranceValue { get => _enduranceValue; set => _enduranceValue = value; }

        public bool Damage(int _damage, Status _status)
        {
            _enduranceValue -= _damage;

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
