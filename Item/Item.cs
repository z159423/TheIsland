using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class ItemStack
{
    public Item Item;
    public int Count;
}

[System.Serializable]
public class KillTarget
{
    public NPCScriptableObject npc;
    public int Count;
}

public enum ItemCategoryType
{
    NONE = 0,
    MATERIAL = 1,
    FOOD = 2
}

[CreateAssetMenu(fileName = "Item", menuName = "Game/Item")]
public class Item : VerificationObject
{
    public const int MaxStack = 9999999;

    [field: SerializeField] public int ID { get; private set; }
    [HorizontalGroup("AutoID")]
    [SerializeField] int startAutoID = 1;

    [field: SerializeField] public LocalizedString NameLocal { get; private set; }
    [field: SerializeField] public LocalizedString DescLocal { get; private set; }
    [field: SerializeField] public ItemCategoryType ItemType { get; private set; }
    [field: SerializeField] public bool Stackable { get; private set; } = true;

    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Mesh Mesh { get; private set; }
    [field: SerializeField] public Material[] Materials { get; private set; }



    [HorizontalGroup("AutoID")]
    [Button]
    void AutoID()
    {
#if UNITY_EDITOR
        var res = Resources.LoadAll<Item>("Item");

        if (res.Count(f => f.ID == ID) == 1)
        {
            Debug.Log($"{name} 는 이미 아이디 세팅이 완료되었습니다.");
            return;
        }

        for (int i = startAutoID; i < startAutoID + 100; i++)
        {
            if (res.FirstOrDefault(f => f.ID == i) == null)
            {
                ID = i;
                UnityEditor.EditorUtility.SetDirty(this);
                Debug.Log($"{name} 의 아이디가 {i} 로 자동 설정되었습니다.");
                break;
            }
        }
#endif
    }

    private void OnValidate()
    {
        Verification();
    }

    public virtual void OnUse(SaveInventoryData.Controller inventory, Player player, int count = 1, int slotNum = -1)
    {
        MonoBehaviour.print("아이템 사용 : " + NameLocal.GetLocalizedString() + " " + count + "개");

        if (slotNum == -1)
        {
            inventory.LossItem(ID, 1);
        }
        else
        {
            inventory.LossItem_InventoryIndex(slotNum, 1);
        }
    }

    public override void Verification()
    {
        var res = Resources.LoadAll<Item>("Item").ToList();
        var find = res.Find(f => f != this && f.ID == ID);
        if (find != null)
            Debug.LogError($"({ID}, {NameLocal.GetLocalizedString()}) is duplicated ({find.ID}, {find.NameLocal.GetLocalizedString()})");
    }

    public virtual int GetHealValue()
    {
        return 0;
    }
    public virtual int GetHungerValue()
    {
        return 0;
    }

}
