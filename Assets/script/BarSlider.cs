using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarSlider : MonoBehaviour
{
    [SerializeField] private Slider hpbar;
    private Status _status;

    // Start is called before the first frame update
    void Start()
    {

        _status = transform.root.gameObject.GetComponent<Status>();
        if (hpbar == null || _status == null) return;
        hpbar.maxValue = _status.HP;
        hpbar.minValue = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (hpbar == null || _status == null) return;
        hpbar.value = _status.HP;
    }
}
