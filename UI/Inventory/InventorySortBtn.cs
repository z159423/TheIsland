using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventorySortBtn : MonoBehaviour
{
    [SerializeField] private ItemCategoryType itemCategoryType;

    public Image line;

    public void OnClickSortBtn()
    {
        InventoryUI.Instance.UpdateInventoryDisplay(itemCategoryType);
    }
}
