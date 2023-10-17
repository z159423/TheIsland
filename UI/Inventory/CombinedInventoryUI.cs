using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UI;

public class CombinedInventoryUI : SingletonInstance<CombinedInventoryUI>
{
    [BoxGroup("Inventory")][SerializeField] private Transform layoutParent;
    [BoxGroup("Inventory")][SerializeField] private Image selectedItemImage;
    [BoxGroup("Inventory")][SerializeField] private Text selectedItemName;
    [BoxGroup("Inventory")][SerializeField] private Text selectedItemAmount;


    [BoxGroup("Equipment")][SerializeField] Image currentEquipemnt_swordImage;
    [BoxGroup("Equipment")][SerializeField] Image currentEquipemnt_axeImage;
    [BoxGroup("Equipment")][SerializeField] Image currentEquipemnt_pickaxeImage;
    [BoxGroup("Equipment")][SerializeField] Image currentEquipemnt_chestIamge;

    [Space]

    [BoxGroup("Equipment")][SerializeField] Text sword_Lv;
    [BoxGroup("Equipment")][SerializeField] Text axe_Lv;
    [BoxGroup("Equipment")][SerializeField] Text pickaxe_Lv;
    [BoxGroup("Equipment")][SerializeField] Text chest_Lv;

    [Space]

    [BoxGroup("Equipment")][SerializeField] Text hp_text;
    [BoxGroup("Equipment")][SerializeField] Text damage_text;

    [Space]

    [BoxGroup("Equipment")][SerializeField] Text stat_sword;
    [BoxGroup("Equipment")][SerializeField] Text stat_axe;
    [BoxGroup("Equipment")][SerializeField] Text stat_pickaxe;

    [Space]

    [BoxGroup("Skin")][SerializeField] Transform skinLayoutParent;
    [BoxGroup("Skin")][SerializeField] UIDynamicPanel skinDynamic;
    [BoxGroup("Skin")][SerializeField] Text skinNameText;


    private bool skinLayout = false;




    // Start is called before the first frame update
    private void OnEnable()
    {
        UpdateInventoryDisplay(immediateColl: true);
        UpdateUI();

        GenerateSkinLoayout();
    }

    public void UpdateUI()
    {
        var currentSword = PlayerSaveData.GetEquipmentData(SaveManager.Instance.PlayerController.GetSwordId());
        var currentAxe = PlayerSaveData.GetEquipmentData(SaveManager.Instance.PlayerController.GetAxeId());
        var currentPickaxe = PlayerSaveData.GetEquipmentData(SaveManager.Instance.PlayerController.GetPickaxeId());
        var currentChest = PlayerSaveData.GetEquipmentData(SaveManager.Instance.PlayerController.GetChestArmorId());

        currentEquipemnt_swordImage.sprite = currentSword.Icon;
        currentEquipemnt_pickaxeImage.sprite = currentPickaxe.Icon;
        currentEquipemnt_axeImage.sprite = currentAxe.Icon;
        currentEquipemnt_chestIamge.sprite = currentChest == null ? ResourceManager.Instance.GetTrnasparentSprite() : currentChest?.Icon;

        pickaxe_Lv.text = currentPickaxe.NextEquipment ? "Lv. " + currentPickaxe.Tier.ToString() : "Max";
        axe_Lv.text = currentAxe.NextEquipment ? "Lv. " + currentAxe.Tier.ToString() : "Max";
        sword_Lv.text = currentSword.NextEquipment ? "Lv. " + currentSword.Tier.ToString() : "Max";
        chest_Lv.text = currentChest == null ? "Lv. 0" : (currentChest?.NextEquipment ? "Lv. " + currentChest?.Tier.ToString() : "Max");

        hp_text.text = SaveManager.Instance.PlayerController.GetPlayerMaxHp().ToString();
        // damage_text.text = currentChest.Armor.ToString();
        damage_text.text = currentSword.Damage.ToString();

        // stat_sword.text = currentSword.Tier.ToString();
        // stat_axe.text = currentAxe.Tier.ToString();
        // stat_pickaxe.text = currentPickaxe.Tier.ToString();

        PlayerManager.Instance.ChangeDummyPlayerEquipment(EquipmentType.CHEST, currentChest.mesh);
        PlayerManager.Instance.ChangeDummySkin(SaveManager.Instance.PlayerController.GetPlayerCurrentSkin());
    }

    public void UpdateInventoryDisplay(ItemCategoryType type = ItemCategoryType.NONE, bool collapse = true, bool immediateColl = false)
    {
        CleanInventoryDisplay(collapse: collapse, immediateColl: immediateColl);

        // UpdateTab(type);
        // currentCategoryType = type;

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
            ObjectPool.Instance.GetPool("UI/Empty Slot", layoutParent);
        }

        //RV로 잠금해제 가능한 인벤토리 슬롯
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

        foreach (var slot in layoutParent.GetComponentsInChildren<EmptySlot>())
        {
            ObjectPool.Instance.AddPool(slot.gameObject);
        }

        // if (collapse)
        //     UnSelectSlot(immediateColl);
    }

    public void OnSelectSlot(InventorySlot slot)
    {
        if (slot == null)
            return;

        var itemResource = Resources.LoadAll<Item>("Item").ToList();
        var find = itemResource.Find(f => f.ID == slot.item.ID);

        selectedItemName.LocalizedString(find.NameLocal);
        selectedItemImage.sprite = find.Icon;

        selectedItemAmount.text = SaveManager.Instance.InventoryController.GetItemCount(find.ID).ToString("#,###");

        layoutParent.GetComponentsInChildren<InventorySlot>().ForEach(f => { f.UnSelectedSlot(); });
    }

    public void OnClickEquipmentSlot(EquipmentType type)
    {
        var equipment = SaveManager.Instance.PlayerController.GetCurrentEquipmentData(type);

        PlayerManager.Instance.ChangeDummyPlayerEquipment(equipment.Type, equipment.mesh);
    }

    public void OnSelectSlot(SkinSlot slot)
    {
        if (slot == null || !SaveManager.Instance.PlayerController.CheckOwnedSkin(slot.skinGuid))
            return;

        print(SaveManager.Instance.PlayerController.CheckOwnedSkin(slot.skinGuid));

        print(slot.skinGuid);

        PlayerManager.Instance.player.GetComponentInChildren<PlayerSkin>().ChangePlayerSkin(slot.skinGuid);

        skinNameText.text = LocalizationManager.Instance.GetLocalizedString("Skin", ResourceManager.Instance.GetPlayerSkin(slot.skinGuid).skinNameKey);

        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "Change Skin On UI - " + slot.skinGuid);

        foreach(var skinSlot in skinLayoutParent.GetComponentsInChildren<SkinSlot>())
        {
            skinSlot.UnSelectedSlot();
        }
    }

    public void OnClickSkinBtn()
    {
        if (!skinLayout)
        {
            skinDynamic.Expand();
            skinLayout = true;

            AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "Click Skin Btn - On");

        }
        else
        {
            skinDynamic.Collapse();
            skinLayout = false;

            AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "Click Skin Btn - Off");

        }
    }

    void GenerateSkinLoayout()
    {
        foreach (var skin in ResourceManager.Instance.GetplayerSkines())
        {
            var slot = ObjectPool.Instance.GetPool("UI/Skin Slot", skinLayoutParent);
            if (SaveManager.Instance.PlayerController.CheckOwnedSkin(skin.Guid))
            {
                slot.GetComponent<SkinSlot>().InitSkinSlot(ItemSlotType.SKIN, skin.Guid);

                // if (skin.Guid == SaveManager.Instance.PlayerController.GetPlayerCurrentSkin())

            }
            else
            {
                slot.GetComponent<SkinSlot>().InitSkinSlot(ItemSlotType.LOCK, skin.Guid);

            }
        }
    }
}
