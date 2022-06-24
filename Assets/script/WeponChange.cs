using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeponChange : MonoBehaviour
{
    [SerializeField] GameObject _parentObject;
    [SerializeField] GameObject _wepon = null;
    [SerializeField] HitReceiver _hitReceiver;
    [SerializeField] WeponType _weponType;

    public enum WeponType
    {
        Axe,
        Spear
    }
    private string[] WeponName = new string[]
    {
        "Wepon/Axe",
        "Wepon/Spear"
    };

    private void Start()
    {
        _weponType = WeponType.Axe;
    }

    public void Change(WeponType wepon)
    {
        //���퐔�ȏ�̐��l�����������̃G���[����

        //���ɕ���������Ă����炻��ƍ폜
        if (_wepon != null)
        {
            Destroy(_wepon);
            _wepon = null;
            Resources.UnloadUnusedAssets();
        }
        //�C���X�^���X��
        _wepon = Instantiate(Resources.Load(WeponName[(int)wepon]), _parentObject.transform.position, _parentObject.transform.rotation) as GameObject;
        _wepon.transform.parent = _parentObject.transform;
        _wepon.transform.localScale = new Vector3(1, 1, 1);
        _weponType = wepon;
    }

    public PartType GetPartType()
    {
        switch (_weponType)
        {
            case WeponType.Axe:
                return PartType.axe;

            case WeponType.Spear:
                return PartType.spear;

            default:
                return PartType.axe;
        }
    }
    public void C_Axe()
    {
        Change(WeponType.Axe);
    }
    public void C_Spear()
    {
        Change(WeponType.Spear);
    }
}
