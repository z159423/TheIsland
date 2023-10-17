using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EquipmentUpgradeUI : MonoBehaviour
{
    [SerializeField] Image equipmentImage;
    [SerializeField] Text equipmentName;

    [SerializeField] UIDynamicPanel dynamic;


    public void Init(int id)
    {
        var equipment = ResourceManager.Instance.GetEquipment(id);

        if (equipment == null)
        {
            ObjectPool.Instance.AddPool(gameObject);
            return;
        }

        equipmentImage.sprite = equipment.Icon;
        equipmentName.text = equipment.NameLocal.GetLocalizedString();

        dynamic.Expand();

        this.TaskDelay(3.5f, () => { dynamic.Collapse(); this.TaskDelay(1f, () => ObjectPool.Instance.AddPool(gameObject)); });
    }
}
