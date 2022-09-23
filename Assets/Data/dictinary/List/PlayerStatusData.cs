using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class PlayerStatusData : MonoBehaviour
{
    [SerializeField] PlayerSaveData _playerSaveData;
    public PlayerSaveData PlayerSaveData { get => _playerSaveData; set => _playerSaveData = value; }
}
