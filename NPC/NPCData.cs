using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData : ScriptableObject
{
    [SerializeField] public int ID { get; private set; }
    [SerializeField] public int MaxHp { get; private set; } = 10;
    [SerializeField] public int MoveSpeed { get; private set; }
    [SerializeField] public GameObject Prefab { get; private set; }
}
