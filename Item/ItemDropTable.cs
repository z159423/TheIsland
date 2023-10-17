using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ItemDropTable
{
    [field: SerializeField] public Item Item { get; private set; }
    [field: SerializeField][Range(0f, 100f)] public float dropPercent;
    [field: SerializeField][Range(0, 99)] public int minDropAmount;
    [field: SerializeField][Range(0, 99)] public int maxDropAmount;

    public bool GetDropSuccess()
    {
        if (dropPercent > UnityEngine.Random.Range(0f, 100f))
            return true;
        else
            return false;
    }

    public int GetDropAmount()
    {
        return UnityEngine.Random.Range(minDropAmount, maxDropAmount + 1);
    }
}
