using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeponChange : MonoBehaviour
{
    [SerializeField] GameObject _parentObject;
    [SerializeField] GameObject _wepon = null;
    [SerializeField] HitReceiver _hitReceiver;

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
    }

    public void Change(WeponType wepon)
    {
        //武器数以上の数値が入った時のエラー処理

        //既に武器を持っていたらそれと削除
        if (_wepon != null)
        {
            Destroy(_wepon);
            _wepon = null;
            Resources.UnloadUnusedAssets();
        }
        //インスタンス化
        _wepon = Instantiate(Resources.Load(WeponName[(int)wepon]), _parentObject.transform.position, _parentObject.transform.rotation) as GameObject;
        _wepon.transform.parent = _parentObject.transform;
        _wepon.transform.localScale = new Vector3(1, 1, 1);
    }


}
