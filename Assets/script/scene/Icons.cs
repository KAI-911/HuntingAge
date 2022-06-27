using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Icons : MonoBehaviour
{
    [SerializeField] List<ItemButton> _ItemBoxButtons;
    [SerializeField] List<ItemButton> _ItemPoachButtons;
    [SerializeField] GameObject ItemBoxObject;
    [SerializeField] GameObject ItemPoachObject;




    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("Set")]
    private void Set()
    {
        foreach (var button in _ItemBoxButtons)
        {
            button.clear();
        }
        foreach (var button in _ItemPoachButtons)
        {
            button.clear();
        }

        foreach (var item in GameManager.Instance.ItemBox.Dictionary)
        {
            var data = GameManager.Instance.ItemDataList.Dictionary[item.Key];
            Debug.Log(data.Name + "  BoxUINumber  " + data.BoxUINumber + "  PoachUINumber  " + data.PoachUINumber);
            if (data.BoxUINumber >= 0)
            {
                Debug.Log("dsafadsgojslkhoisdnvfgsemtfcjse;c");
                _ItemBoxButtons[data.BoxUINumber].SetID(data.ID);
            }
            if (data.PoachUINumber >= 0)
            {
                Debug.Log("dsafadsgojslkhoisdnvfgsemtfcjse;c");
                _ItemPoachButtons[data.PoachUINumber].SetID(data.ID);
            }
        }
    }

}
