using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkinSlot : ItemSlot
{
    [SerializeField] Text skinNameText;
    [SerializeField] Image skinImage;

    public string skinGuid;


    public override void OnClickSkinSlot()
    {
        PlayerManager.Instance.ChangeDummySkin(skinGuid);

        if (!SaveManager.Instance.PlayerController.CheckOwnedSkin(skinGuid))
            return;

        CombinedInventoryUI.Instance.OnSelectSlot(this);

        base.OnClickSkinSlot();
    }

    public void InitSkinSlot(ItemSlotType type, string guid)
    {
        skinGuid = guid;
        slotUI.InitSlot(type, OnClickSkinSlot, guid);

        if (SaveManager.Instance.PlayerController.GetPlayerCurrentSkin() == guid)
            selectedCover.gameObject.SetActive(true);

        var skin = ResourceManager.Instance.GetPlayerSkin(guid);

        skinNameText.text = LocalizationManager.Instance.GetLocalizedString("Skin", skin.skinNameKey);
        skinImage.sprite = skin.skinIcon;
    }
}
