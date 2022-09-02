using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Delete : MonoBehaviour
{
    [SerializeField] float deleteTime;
    void Start()
    {
        DontDestroyOnLoad(this);
        Destroy(gameObject, deleteTime);
    }
}
