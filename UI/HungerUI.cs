using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HungerUI : SingletonInstance<HungerUI>
{
    [SerializeField] private Slider slider;
    [SerializeField] Button rvButton;
    [SerializeField] Button hungerButton;
    [SerializeField] Text hungerText;


    public Gradient gradient;
    public Image fill;

    private void Start()
    {
        rvButton.onClick.AddListener(() =>
        {
            MondayOFF.AdsManager.ShowRewarded(() =>
            {
                PlayerManager.Instance.player.FullHunger();
                AnalyticsManager.Instance.RewardVideoEvent("Hunger");
            });
        });

        hungerButton.onClick.AddListener(() =>
        {
            OpenInventoryFoodTab();
        });

        rvButton.gameObject.SetActive(false);
    }

    public void SetMaxHunger(int value)
    {
        slider.maxValue = value;
        //slider.value = value;

        //fill.color = gradient.Evaluate(1f);
    }

    public void SetCurrentHunger(int value)
    {
        slider.value = value;
        hungerText.text = value.ToString();

        //fill.color = gradient.Evaluate(slider.normalizedValue);

        rvButton.gameObject.SetActive(value < SaveManager.Instance.PlayerController.GetPlayerMaxHunger() * 0.8f);
    }

    public void OpenInventoryFoodTab()
    {
        // var UI = UIManager.Instance.ShowScreen("UI/Inventory");

        UIManager.Instance.ShowScreen("UI/Inventory", (u) =>
            {
                var inv = u.GetComponent<InventoryUI>();
                InventorySlot find = null;

                find = inv.FindItemSlotAsItemType(ItemCategoryType.FOOD);

                if (find != null)
                    inv.OnSelectSlot(find);
            });

        SoundManager.Instance.PlaySFX("InventoryOpen", 0.4f, Random.Range(0.8f, 1.2f));
        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.UI, "HungerUI_InventoryFoodTab");

        InventoryUI.Instance.UpdateInventoryDisplay(ItemCategoryType.FOOD);


    }

    // public override void Show()
    // {
    //     gameObject.SetActive(true);
    // }

    // public override void Hide()
    // {
    //     gameObject.SetActive(false);
    // }
}
