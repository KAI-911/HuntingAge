using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemBoxUI : MonoBehaviour
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
        foreach (var item in GameManager.Instance.ItemBox.Dictionary)
        {
            
        }
    }

}
