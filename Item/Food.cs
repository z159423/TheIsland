using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Game/Food")]
public class Food : Item
{
    [Space]

    [SerializeField] private int HealValue = 0;
    [SerializeField] private int HungerValue = 0;


    public override void OnUse(SaveInventoryData.Controller inventory, Player player = null, int count = 1, int slotNum = -1)
    {
        base.OnUse(inventory, player, count, slotNum);

        if (player != null)
        {
            for (int i = 0; i < count; i++)
            {
                if (HealValue >= 0)
                    player.Heal(HealValue);

                if (HungerValue >= 0)
                    player.Eat(HungerValue);
            }
        }
    }

    public override int GetHealValue()
    {
        return HealValue;
    }

    public override int GetHungerValue()
    {
        return HungerValue;
    }
}
