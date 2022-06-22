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
            Debug.Log("���ʂ�������܂���");
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
        // ����
        [SerializeField] private PartType _partType;
        public PartType PartType { get => _partType; set => _partType = value; }

        // ���ݒl
        [SerializeField] private int _maxEnduranceValue;
        public int MaxEnduranceValue { get => _maxEnduranceValue; set => _maxEnduranceValue = value; }

        // ���݂̋��ݒl
        private int _enduranceValue;
        public int EnduranceValue { get => _enduranceValue; set => _enduranceValue = value; }

        public bool Damage(int _damage, Status _status)
        {
            _enduranceValue -= _damage;

            if (_enduranceValue <= 0)
            {
                _enduranceValue = _maxEnduranceValue;
                if (_status.DownFlg) return false;//�_�E�����ɋ��ݒl��0�ɂȂ��Ă��_�E�����Ȃ�
                return true;
            }
            return false;
        }
    }


}
