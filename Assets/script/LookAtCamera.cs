using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class LookAtCamera : MonoBehaviour
{
    CinemachineFreeLook _freeLookCamera;
    bool _isAutoLookAt;
    float _autoLookAtAngle;
    float _autoLookAtAngleProgress;
    float _autoLookAtCurrentVelocity;
    void Start()
    {
        _freeLookCamera = GetComponent<CinemachineFreeLook>();
    }
    private void Update()
    {
        if (_isAutoLookAt)
        {
            float angle = Mathf.SmoothDamp(
                _autoLookAtAngleProgress, _autoLookAtAngle, ref _autoLookAtCurrentVelocity, 0.2f);

            // �O��Ƃ̍�����ݒ�.
            _freeLookCamera.m_XAxis.Value = angle - _autoLookAtAngleProgress;
            _autoLookAtAngleProgress = angle;

            // ���������ڕW�p�x�ɂȂ�����I��.
            if (Mathf.Abs((int)_autoLookAtAngleProgress) >= Mathf.Abs((int)_autoLookAtAngle)) _isAutoLookAt = false;
        }

    }
    public void LookAtTarget(Vector3 target)
    {
        Debug.Log("dfhashgfldsadjfpgshjg;asfasgsagdsfgfxdhbg");
        // ���ꂼ��̍��W���J�����̍����ɍ��킹��.
        float cameraHeight = _freeLookCamera.transform.position.y;
        Vector3 followPosition =
            new Vector3(_freeLookCamera.Follow.position.x, cameraHeight, _freeLookCamera.Follow.position.z);
        Vector3 targetPosition =
            new Vector3(target.x, cameraHeight, target.z);

        // ���ꂼ��̃x�N�g�����v�Z.
        Vector3 followToTarget = targetPosition - followPosition;
        Vector3 followToTargetReverse = Vector3.Scale(followToTarget, new Vector3(-1, 1, -1));
        Vector3 followToCamera = _freeLookCamera.transform.position - followPosition;

        // �J������]�̊p�x�ƕ������v�Z.
        Vector3 axis = Vector3.Cross(followToCamera, followToTargetReverse);
        float direction = axis.y < 0 ? -1 : 1;
        float angle = Vector3.Angle(followToCamera, followToTargetReverse);

        _autoLookAtAngle = angle * direction;
        _isAutoLookAt = true;
        _autoLookAtCurrentVelocity = 0;
        _autoLookAtAngleProgress = 0;
    }
    public void SetSensitivity(float sensitivity)
    {
        var tmp = _freeLookCamera.m_XAxis;
        tmp.m_MaxSpeed = sensitivity;
        _freeLookCamera.m_XAxis = tmp;
    }
}