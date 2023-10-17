using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] EquipmentType equipmentType;

    public void OnClickEquipmentSlot()
    {
        // EquipmentUI.Instance.OnClickEquipmentSlot(equipmentType);
        CombinedInventoryUI.Instance.OnClickEquipmentSlot(equipmentType);

    }
}
