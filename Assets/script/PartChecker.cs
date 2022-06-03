using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartChecker : MonoBehaviour
{
    [SerializeField] private PartType _partType;
    public PartType PartType { get => _partType; }


    [SerializeField] private int _partMaxEnduranceValue;
    public int PartMaxEnduranceValue { get => _partMaxEnduranceValue; set => _partMaxEnduranceValue = value; }

    private int _partEnduranceValue;
    public int PartEnduranceValue { get => _partEnduranceValue; set => _partEnduranceValue = value; }

    private Status _status;

    private void Start()
    {
        PartEnduranceValue = PartMaxEnduranceValue;
        _status = this.gameObject.transform.root.gameObject.GetComponent<Status>();
    }
    //���ݒl��0�ɂȂ�����true��Ԃ�
    public bool PartEnduranceDamage(int _damage)
    {
        
        _partEnduranceValue -= _damage;
        if (_partEnduranceValue <= 0)
        {
            _partEnduranceValue = _partMaxEnduranceValue;
            if (_status.DownFlg) return false;//�_�E�����ɋ��ݒl��0�ɂȂ��Ă��_�E�����Ȃ�
            return true;
        }
        return false;
    }
}
