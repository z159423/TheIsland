using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSkinUIBase : UIBase
{

    [SerializeField] Text nameText;

    private GameObject dummy;

    public override void Show()
    {

    }

    public override void Hide()
    {
        HideAnimation(() => { Destroy(gameObject); Destroy(dummy); });
    }

    public void InitDummy(string guid)
    {
        dummy = Instantiate(ResourceManager.Instance.GetResource<GameObject>("Animation/Models/New Skin Dummy"));

        var dummyScripts = dummy.GetComponentInChildren<NewSkinPlayerDummy>();

        nameText.text = LocalizationManager.Instance.GetLocalizedString("Skin", ResourceManager.Instance.GetPlayerSkin(guid).skinNameKey);

        dummyScripts.Init(guid);
    }
}
