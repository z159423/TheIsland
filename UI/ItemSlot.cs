using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class ItemSlot : MonoBehaviour
{
    [field: SerializeField] public ItemData item;
    [field: SerializeField] public int slotNum { get; set; }

    [Space]

    [SerializeField] private bool selected = false;
    [SerializeField] protected Image selectedCover;


    //[SerializeField] private GameObject slotUIPrefab;

    [SerializeField] protected SlotUI slotUI;


    public void InitSlot(ItemData item, ItemSlotType type)
    {
        //slotUI = Instantiate(slotUIPrefab, transform).GetComponent<SlotUI>();

        slotUI.InitSlot(type, OnClickThisSlot, item.ID, item.Count);

        this.item = item;

        //var scriptableObjects = Resources.LoadAll<Item>("Item").ToList();
        //var find = scriptableObjects.Find(f => f.ID == item.ID);
        //itemImage.sprite = find?.Icon;

        //amountText.text = item.Count.ToString();
    }

    public virtual void OnClickThisSlot()
    {
        if (item == null || item.Count == 0 || item.ID == 0)
            return;

        //GetComponent<Image>().color = Color.yellow;

        selectedCover.gameObject.SetActive(true);
    }

    public virtual void OnClickSkinSlot()
    {
        selectedCover.gameObject.SetActive(true);

        print("");
    }

    public void OnClickRvSlot()
    {
        MondayOFF.AdsManager.ShowRewarded(() =>
            {
                AnalyticsManager.Instance.RewardVideoEvent("InventorySlotUnlock_4");
                //SaveManager.Instance.IAPController.RV_UnlockInventorySlot();

                //Hide();
            });
    }

    public void OnClickSurveySlot()
    {
        if (!SaveManager.Instance.OptionController.GetSurvey())
        {
            // InventoryUI.Instance.InventoryHide();
            var ui = UIManager.Instance.ShowScreen("UI/Survey");

            AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "OnClickInventorySurveyBtn");

            // ui.Show();
        }
    }

    public void UnSelectedSlot()
    {
        this.selected = false;

        selectedCover.gameObject.SetActive(false);

        //GetComponent<Image>().color = Color.white;
        //UpdateThisSlot();
    }

    public void UpdateThisSlot()
    {
        if (item == null)
        {
            ClearUI();
        }
        else
        {
            if (item.Count <= 0)
                ClearUI();
        }
    }

    public void ClearUI()
    {
        slotUI.ClearSlot();
        //DestroyImmediate(slotUI.gameObject);
    }

    public void EmptySlot()
    {

    }


}
