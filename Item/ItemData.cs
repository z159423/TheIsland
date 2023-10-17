using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Firebase.Database;

[System.Serializable]
public class ItemData : IDatabaseInit
{
    [SerializeField] public int Index;
    [SerializeField] public int ID;
    [SerializeField] public int Count;

    // public InventoryItem(int _id, int _stack)
    // {
    //     this.ID = _id;
    //     this.Stack = _stack;
    // }

    ///<summary>
    /// 이미 인벤토리에 존재하는 아이템일 경우 count만큼 개수를 추가하고 추가한 개수가 maxCount보다 커지면 남은 개수만큼 다시 반환함
    ///</summary>
    public int AddItem(int count, int maxCount)
    {
        int remain = 0;

        if (Count + count > maxCount)
        {
            //MonoBehaviour.print(Count + " " + count + " " + maxCount);
            remain = (Count + count) - maxCount;
        }

        Count += count;

        Count = Mathf.Clamp(Count, 0, maxCount);

        return remain;
    }

    public void LossItem(int count)
    {
        Count -= count;
    }

    public ItemCategoryType GetItemCategoryType()
    {
        var res = Resources.LoadAll<Item>("Item").ToList();
        var find = res.Find(f => f.ID == ID);

        return find.ItemType;
    }

    public void Init(DataSnapshot data)
    {
        ID = int.Parse(data.Child("ID").Value.ToString());
        Count = int.Parse(data.Child("Count").Value.ToString());
    }
}
