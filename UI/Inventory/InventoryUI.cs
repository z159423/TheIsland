using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Sirenix.OdinInspector;


public class InventoryUI : SingletonInstance<InventoryUI>
{
    [SerializeField] private Transform layoutParent;

    [Space]



    [Space]

    [SerializeField] RectTransform detailInfoLayout;

    [SerializeField] private GameObject hungerParent;
    [SerializeField] private Text hungerText;

    [SerializeField] private GameObject healParent;
    [SerializeField] private Text healText;

    [SerializeField] private GameObject timeParent;
    [SerializeField] private Text timeText;

    [SerializeField] private GameObject bakeParent;
    [SerializeField] private Text bakeText;

    [Space]

    [SerializeField] private GameObject itemDropBtn;
    [SerializeField] private GameObject itemUseBtn;

    [SerializeField] private Text itemName;
    [SerializeField] private Text itemDescription;
    [SerializeField] private Image itemImage;
    [SerializeField] private UIDynamicPanel dynamicPanel;

    [Space]

    [SerializeField] private InventorySlot SelectedItemSlot = null;

    [FoldoutGroup("Drop Item Menu")][SerializeField] private GameObject dropPanel;
    [FoldoutGroup("Drop Item Menu")][SerializeField] private Slider dropItemCountSlider;
    [FoldoutGroup("Drop Item Menu")][SerializeField] private Text dropItemCountText;

    [Space]

    [SerializeField] private Button tabButtonAll;
    [SerializeField] private Button tabButtonMaterial;
    [SerializeField] private Button tabButtonFood;
    [SerializeField] private Sprite activeButtonSprite;
    [SerializeField] private Sprite deactiveButtonSprite;


    private ItemCategoryType currentCategoryType;


    private void OnEnable()
    {
        UpdateInventoryDisplay(immediateColl: true);
    }

    void UpdateTab(ItemCategoryType newType)
    {
        void ChangeColor(ItemCategoryType type, Button button)
        {
            // var colors = tabButtonAll.colors;
            // colors.normalColor = type == newType ? Util.ToColor("#C0C0C0") : Color.white;
            // colors.highlightedColor = type == newType ? Util.ToColor("#B5B5B5") : Util.ToColor("#F5F5F5");
            // colors.pressedColor = type == newType ? Util.ToColor("#888888") : Util.ToColor("#C8C8C8");
            // colors.selectedColor = type == newType ? Util.ToColor("#B5B5B5") : Util.ToColor("#F5F5F5");
            // button.colors = colors;

            button.GetComponent<Image>().sprite = type == newType ? activeButtonSprite : deactiveButtonSprite;
            button.GetComponent<InventorySortBtn>().line.enabled = type == newType ? true : false;
        }

        ChangeColor(ItemCategoryType.NONE, tabButtonAll);
        ChangeColor(ItemCategoryType.MATERIAL, tabButtonMaterial);
        ChangeColor(ItemCategoryType.FOOD, tabButtonFood);
    }

    public void UpdateInventoryDisplay(ItemCategoryType type = ItemCategoryType.NONE, bool collapse = true, bool immediateColl = false)
    {
        CleanInventoryDisplay(collapse: collapse, immediateColl: immediateColl);

        UpdateTab(type);

        currentCategoryType = type;

        int entireSlotCount = SaveManager.Instance.InventoryController.GetUseableInventorySlotCount();
        int usingSlotCount = 0;

        var data = SaveManager.Instance.InventoryController.GetInventoryData();

        for (int i = 0; i < data.Count; i++)
        {
            if (type == ItemCategoryType.NONE)
            {
                var slot = GenerateSlot(i);

                slot.GetComponent<InventorySlot>().slotNum = i;
                usingSlotCount++;
            }
            else
            {
                if (SaveManager.Instance.InventoryController.GetInventoryData()[i].GetItemCategoryType() == type)
                {
                    var slot = GenerateSlot(i);

                    slot.GetComponent<InventorySlot>().slotNum = i;
                    usingSlotCount++;
                }
            }
        }

        if (type != ItemCategoryType.NONE)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].GetItemCategoryType() != type)
                {
                    var slot = ObjectPool.Instance.GetPool("UI/InventorySlot", layoutParent);

                    slot.GetComponent<ItemSlot>().InitSlot(data[i], ItemSlotType.HIDE);

                    slot.GetComponent<InventorySlot>().slotNum = i;
                    usingSlotCount++;
                }
            }
        }

        //비어있는 인벤토리 슬롯
        int emptySlotCount = entireSlotCount - usingSlotCount;
        for (int i = 0; i < emptySlotCount; i++)
        {
            var slot = GenerateSlot(i, true);

            slot.GetComponent<InventorySlot>().slotNum = usingSlotCount + i;

            if (type != ItemCategoryType.NONE)
                slot.GetComponent<InventorySlot>().InitSlot(new ItemData(), ItemSlotType.HIDE);

        }

        ////RV로 잠금해제 가능한 인벤토리 슬롯
        //for (int i = 0; i < SaveManager.Instance.IAPController.RV_GetUnlockableSize(); i++)
        //{
        //    var slot = ObjectPool.Instance.GetPool("UI/InventorySlot", layoutParent);
        //    slot.GetComponent<ItemSlot>().InitSlot(new ItemData(), ItemSlotType.RVLock);
        //}

        ////Survey로 잠금해제 가능한 인벤토리 슬롯
        //for (int i = 0; i < SaveManager.Instance.IAPController.Survey_GetUnlockableSize(); i++)
        //{
        //    var slot = ObjectPool.Instance.GetPool("UI/InventorySlot", layoutParent);
        //    slot.GetComponent<ItemSlot>().InitSlot(new ItemData(), ItemSlotType.SurveyLock);
        //}

        ////잠겨있는 인벤토리 슬롯
        //for (int i = 0; i < SaveIAPData.UnlockableInventorySlotSize - SaveManager.Instance.IAPController.IAP_GetUnlockedSlotCount(); i++)
        //{
        //    var slot = ObjectPool.Instance.GetPool("UI/InventorySlot", layoutParent);
        //    slot.GetComponent<ItemSlot>().InitSlot(new ItemData(), ItemSlotType.LOCK);
        //}

        GameObject GenerateSlot(int i, bool emptySlot = false)
        {
            var slot = ObjectPool.Instance.GetPool("UI/InventorySlot", layoutParent);

            var data = SaveManager.Instance.InventoryController.GetInventoryData();

            if (!emptySlot)
                slot.GetComponent<ItemSlot>().InitSlot(data[i], ItemSlotType.INVENTORY);
            else
                slot.GetComponent<ItemSlot>().InitSlot(new ItemData(), ItemSlotType.INVENTORY);

            return slot;
        }
    }

    public void CleanInventoryDisplay(bool collapse = true, bool immediateColl = false)
    {
        foreach (var slot in layoutParent.GetComponentsInChildren<ItemSlot>())
        {
            slot.ClearUI();
            ObjectPool.Instance.AddPool(slot.gameObject);
        }

        if (collapse)
            UnSelectSlot(immediateColl);
    }

    public InventorySlot FindItemSlot(int id)
    {
        foreach (var slot in layoutParent.GetComponentsInChildren<InventorySlot>())
        {
            var s = slot.GetComponentInChildren<SlotUI>();
            if (s != null)
            {
                if (s.itemID == id)
                    return slot;
            }
        }
        return null;
    }

    public InventorySlot FindItemSlotAsItemType(ItemCategoryType type)
    {
        foreach (var slot in layoutParent.GetComponentsInChildren<InventorySlot>())
        {
            var s = slot.GetComponentInChildren<SlotUI>();
            if (s != null)
            {
                if (ResourceManager.Instance.GetItem(s.itemID).ItemType == type)
                    return slot;
            }
        }
        return null;
    }

    public void InventoryHide()
    {
        UIManager.Instance.HideScreen();
    }

    public void OnSelectSlot(InventorySlot slot)
    {
        if (slot == null)
            return;

        SelectedItemSlot = slot;

        var itemResource = Resources.LoadAll<Item>("Item").ToList();
        var find = itemResource.Find(f => f.ID == slot.item.ID);

        itemName.LocalizedString(find.NameLocal);
        itemDescription.LocalizedString(find.DescLocal);

        int detailInfoCount = 0;

        if (find.GetHealValue() > 0)
        {
            healParent.SetActive(true);
            healText.text = find.GetHealValue().ToString();
            detailInfoCount++;
            //itemDescription.text = itemDescription.text + "\nHeal +" + find.GetHealValue();
        }
        else
            healParent.SetActive(false);

        if (find.GetHungerValue() > 0)
        {
            hungerParent.SetActive(true);
            hungerText.text = find.GetHungerValue().ToString();
            detailInfoCount++;
            //itemDescription.text = itemDescription.text + "\nHunger +" + find.GetHungerValue();
        }
        else
            hungerParent.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(detailInfoLayout);

        itemImage.sprite = find.Icon;

        if (find.ItemType == ItemCategoryType.FOOD)
        {
            itemUseBtn.SetActive(true);
        }
        else
        {
            itemUseBtn.SetActive(false);
        }

        foreach (InventorySlot _slot in layoutParent.GetComponentsInChildren<InventorySlot>())
        {
            _slot.UnSelectedSlot();
        }

        itemDropBtn.SetActive(true);

        dynamicPanel.Expand();

        dropPanel.SetActive(false);
    }

    public void UnSelectSlot(bool immediate = false)
    {
        //SelectedItem = null;
        itemDropBtn.SetActive(false);
        itemUseBtn.SetActive(false);

        hungerParent.SetActive(false);
        healParent.SetActive(false);

        itemImage.sprite = ResourceManager.Instance.GetResource<Sprite>("Sprite/transparent");
        itemName.text = "";
        itemDescription.text = "";

        dynamicPanel.Collapse(immediate);
        if (dropPanel.activeSelf)
            ActiveDropPanel();
    }

    public void ActiveDropPanel()
    {
        dropPanel.SetActive(!dropPanel.activeSelf);
    }

    public void OnClickDropBtn()
    {
        if (SelectedItemSlot == null)
            return;

        ActiveDropPanel();

        if (SelectedItemSlot.item.Count > 1)
            dropItemCountSlider.gameObject.SetActive(true);
        else
            dropItemCountSlider.gameObject.SetActive(false);

        dropItemCountSlider.maxValue = SelectedItemSlot.item.Count;
        dropItemCountSlider.value = SelectedItemSlot.item.Count;

    }

    public void OnClickDropCancleBtn()
    {
        ActiveDropPanel();
    }

    public void DropItem()
    {
        var item = ItemManager.Instance.GenerateItemDrop(SelectedItemSlot.item.ID, PlayerManager.Instance.player.transform.position + PlayerManager.Instance.player.transform.forward + (Vector3.up * 1.5f));
        SaveManager.Instance.InventoryController.LossItem_InventoryIndex(SelectedItemSlot.slotNum, (int)dropItemCountSlider.value);

        ActiveDropPanel();

        //UpdateCurrentSlots();
        UpdateInventoryDisplay();

        item.GetComponent<Rigidbody>().AddForce((PlayerManager.Instance.player.transform.forward.normalized * 100f), ForceMode.Force);
    }

    public void OnClickUseBtn()
    {
        if (SelectedItemSlot == null)
            return;

        var itemResource = Resources.LoadAll<Item>("Item").ToList();
        var find = itemResource.Find(f => f.ID == SelectedItemSlot.item.ID);

        find.OnUse(SaveManager.Instance.InventoryController, PlayerManager.Instance.player, 1);

        if (SelectedItemSlot.item.Count <= 0)
            UpdateInventoryDisplay(currentCategoryType);
        else
            UpdateInventoryDisplay(currentCategoryType, collapse: false);

    }

    public void SetSliderText()
    {
        dropItemCountText.text = dropItemCountSlider.value.ToString();
    }

    public void OnClickDropCountPlusBtn()
    {
        dropItemCountSlider.value++;
        dropItemCountSlider.onValueChanged.Invoke(dropItemCountSlider.value);
    }

    public void OnClickDropCountMinusBtn()
    {
        dropItemCountSlider.value--;
        dropItemCountSlider.onValueChanged.Invoke(dropItemCountSlider.value);
    }
}
