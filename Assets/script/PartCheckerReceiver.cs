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
        Debug.Log("���ʃ_���[�W�v�Z�J�n");
        var part = GetPartData(partType);
        if (part == null)
        {
            Debug.Log("���ʂ�������܂���");
            return false;
        }
        Debug.Log("����  "+ part._partType);
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
        // ����
        public PartType _partType;

        // ���ݒl
        public int _maxEnduranceValue;

        // ���݂̋��ݒl
        public int _enduranceValue;
        public int EnduranceValue { get => _enduranceValue; set => _enduranceValue = value; }

        public bool Damage(int _damage, Status _status)
        {
            _enduranceValue -= _damage;
            Debug.Log("���ʃ_���[�W�v�Z�I��");

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
