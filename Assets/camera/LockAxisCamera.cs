using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class LockAxisCamera : CinemachineExtension
{
    public bool x_islocked, y_islocked, z_islocked;
    public Vector3 lockPosition;
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var newPos = state.RawPosition;
            if (x_islocked) newPos.x = lockPosition.x;
            if (y_islocked) newPos.y = lockPosition.y;
            if (z_islocked) newPos.z = lockPosition.z;
            state.RawPosition = newPos;
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(LockAxisCamera))]
public class LockAxisCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var lockAxisCamera = target as LockAxisCamera;
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUIUtility.labelWidth = 10;
            EditorGUILayout.LabelField("å≈íËÇ∑ÇÈé≤");
            lockAxisCamera.x_islocked = EditorGUILayout.Toggle("X", lockAxisCamera.x_islocked);
            lockAxisCamera.y_islocked = EditorGUILayout.Toggle("Y", lockAxisCamera.y_islocked);
            lockAxisCamera.z_islocked = EditorGUILayout.Toggle("Z", lockAxisCamera.z_islocked);
        }
        EditorGUILayout.LabelField("å≈íËÇ∑ÇÈç¿ïW");
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUIUtility.labelWidth = 10;
            lockAxisCamera.lockPosition.x = EditorGUILayout.FloatField("X", lockAxisCamera.lockPosition.x);
            lockAxisCamera.lockPosition.y = EditorGUILayout.FloatField("Y", lockAxisCamera.lockPosition.y);
            lockAxisCamera.lockPosition.z = EditorGUILayout.FloatField("Z", lockAxisCamera.lockPosition.z);
        }
    }
}
#endif
