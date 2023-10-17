using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryCaller : UIBase
{
    [SerializeField] Image invImage;

    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite fullSprite;

    Sequence fullSeq;

    public override void Show()
    {
        this.SetListener(GameObserverType.Inventory.ON_INVENTORY_FULL, InventoryFull);

        invImage.sprite = defaultSprite;
    }

    public override void Hide()
    {
        HideAnimation(() =>
        {
            //ObjectPool.Instance.AddPoolChild(contentRoot);
            Destroy(gameObject);
        });
    }

    public void InvnetoryActive()
    {
        UIManager.Instance.ShowScreen("UI/CombinedInventory");
        SoundManager.Instance.PlaySFX("InventoryOpen", 0.4f, Random.Range(0.8f, 1.2f));
        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "Inventory");
    }

    void InventoryFull()
    {
        if (fullSeq != null)
            fullSeq.Kill();

        invImage.sprite = fullSprite;
        invImage.transform.localScale = Vector3.one;

        fullSeq = DOTween.Sequence();
        fullSeq.Append(invImage.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 6));
        fullSeq.AppendInterval(2f);
        fullSeq.AppendCallback(() => invImage.sprite = defaultSprite);
    }
}
