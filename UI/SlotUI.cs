using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public enum ItemSlotType
{
    INVENTORY,
    CHEST,
    LOCK,
    RVLock,
    SurveyLock,
    HIDE,
    SKIN
}

public class SlotUI : MonoBehaviour
{
    public ItemSlotType slotType;

    [field: SerializeField] public MultipleTargetButton slotBtn { get; private set; }
    [field: SerializeField] public Image itemImage { get; private set; }
    [field: SerializeField] public Text itemAmountText { get; private set; }

    [field: SerializeField] public int itemID { get; private set; }

    [field: SerializeField] public Transform lockImageTrans { get; private set; }
    [field: SerializeField] public Transform rvImageTrans { get; private set; }
    [field: SerializeField] public Transform surveyImageTrans { get; private set; }
    [field: SerializeField] public Transform hideImageTrans { get; private set; }


    public void InitSlot(ItemSlotType type, UnityAction onClickBtnAction = null, int itemID = 0, int itemAmount = 0)
    {
        this.itemID = itemID;
        this.slotType = type;

        slotBtn.onClick.RemoveAllListeners();
        if (onClickBtnAction != null)
            slotBtn.onClick.AddListener(onClickBtnAction);

        if (itemID != 0)
        {
            var scriptableObjects = Resources.LoadAll<Item>("Item").ToList();
            var find = scriptableObjects.Find(f => f.ID == itemID);
            itemImage.sprite = find?.Icon;
        }

        if (itemAmount != 0)
        {
            itemAmountText.text = Util.GetKMBNumber(itemAmount);
        }

        if (lockImageTrans != null)
            lockImageTrans.gameObject.SetActive(type == ItemSlotType.LOCK);

        if (rvImageTrans != null)
            rvImageTrans?.gameObject.SetActive(type == ItemSlotType.RVLock);

        if (surveyImageTrans != null)
            surveyImageTrans?.gameObject.SetActive(type == ItemSlotType.SurveyLock);

        if (hideImageTrans != null)
            hideImageTrans?.gameObject.SetActive(type == ItemSlotType.HIDE);
    }

    public void InitSlot(bool haveRecipe, UnityAction onClickBtnAction = null, int itemID = 0)
    {
        this.itemID = itemID;

        if (haveRecipe)
        {
            if (itemID != 0)
            {
                itemImage.GetComponent<Shadow>().enabled = true;
                itemImage.GetComponent<Outline>().enabled = true;
                var scriptableObjects = Resources.LoadAll<Item>("Item").ToList();
                var find = scriptableObjects.Find(f => f.ID == itemID);
                itemImage.sprite = find?.Icon;
            }
        }
        else
        {
            itemImage.sprite = Resources.Load<Sprite>("Sprite/question mark");
            itemImage.GetComponent<Shadow>().enabled = false;
            itemImage.GetComponent<Outline>().enabled = false;
        }
    }

    public void InitSlot(ItemSlotType type, UnityAction onClickBtnAction = null, string skinGUID = "")
    {
        this.slotType = type;

        slotBtn.onClick.RemoveAllListeners();
        if (onClickBtnAction != null)
            slotBtn.onClick.AddListener(onClickBtnAction);

        // var skin = ResourceManager.Instance.GetPlayerSkin(skinGUID);

        // itemImage.sprite = skin.skinIcon;

        // slotBtn.onClick.AddListener(() => { PlayerManager.Instance.player.GetComponentInChildren<PlayerSkin>().ChangePlayerSkin(skinGUID); });

        if (lockImageTrans != null)
            lockImageTrans.gameObject.SetActive(type == ItemSlotType.LOCK);

        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "Click Skin Slot - " + skinGUID);

    }

    public void ClearSlot()
    {
        itemImage.sprite = Resources.Load<Sprite>("Sprite/transparent");
        itemAmountText.text = "";
    }
}
