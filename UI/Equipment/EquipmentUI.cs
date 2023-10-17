using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class EquipmentUI : SingletonInstance<EquipmentUI>
{

    [BoxGroup("Upgrade Menu")][SerializeField] Image NextUpgradeEquipment_Image;
    [BoxGroup("Upgrade Menu")][SerializeField] Text NextUpgradeEquipment_Name;
    [BoxGroup("Upgrade Menu")][SerializeField] Text NextUpgradeEquipment_Description;
    [BoxGroup("Upgrade Menu")][SerializeField] UpgradeIngredients[] NextUpgradeEquipment_Ingredients;
    [BoxGroup("Upgrade Menu")][SerializeField] Button upgradeBtn;

    [Space]

    [BoxGroup("Upgrade Menu")][SerializeField] Button infoItemPopup;
    [BoxGroup("Upgrade Menu")][SerializeField] Image infoItemPopupIcon;
    [BoxGroup("Upgrade Menu")][SerializeField] Text infoItemPopupName;
    // [BoxGroup("Upgrade Menu")][SerializeField] Button upgradeBtn_pickaxe;
    // [BoxGroup("Upgrade Menu")][SerializeField] Button upgradeBtn_axe;
    // [BoxGroup("Upgrade Menu")][SerializeField] Button upgradeBtn_sword;
    // [BoxGroup("Upgrade Menu")][SerializeField] Sprite upgradeBtn_pickaxe_act;
    // [BoxGroup("Upgrade Menu")][SerializeField] Sprite upgradeBtn_pickaxe_deact;
    // [BoxGroup("Upgrade Menu")][SerializeField] Sprite upgradeBtn_axe_act;
    // [BoxGroup("Upgrade Menu")][SerializeField] Sprite upgradeBtn_axe_deact;
    // [BoxGroup("Upgrade Menu")][SerializeField] Sprite upgradeBtn_sword_act;
    // [BoxGroup("Upgrade Menu")][SerializeField] Sprite upgradeBtn_sword_deact;

    [BoxGroup("Upgrade Menu")][SerializeField] Text sword_Lv;
    [BoxGroup("Upgrade Menu")][SerializeField] Text axe_Lv;
    [BoxGroup("Upgrade Menu")][SerializeField] Text pickaxe_Lv;
    [BoxGroup("Upgrade Menu")][SerializeField] Text chest_Lv;


    [BoxGroup("Upgrade Menu")][SerializeField] Button RvButton;

    Tween infoItemTween;

    [Space]

    [BoxGroup("Current Equipment")][SerializeField] Image currentEquipemnt_pickaxeImage;
    [BoxGroup("Current Equipment")][SerializeField] Image currentEquipemnt_axeImage;
    [BoxGroup("Current Equipment")][SerializeField] Image currentEquipemnt_swordImage;
    [BoxGroup("Current Equipment")][SerializeField] Image currentEquipemnt_chestIamge;

    [Space]

    [BoxGroup("Player Stat")][SerializeField] Text stat_pickaxe;
    [BoxGroup("Player Stat")][SerializeField] Text stat_axe;
    [BoxGroup("Player Stat")][SerializeField] Text stat_sword;
    [BoxGroup("Player Stat")][SerializeField] Text stat_hunger;
    [BoxGroup("Player Stat")][SerializeField] Text stat_hp;

    [Space]

    [SerializeField] UIDynamicPanel dynamic;

    [SerializeField] Equipment selectedEquipment;

    [System.Serializable]
    public class UpgradeIngredients
    {
        public Button ingredientButton;
        public Image ingredientIcon;
        public Text ingredientNeedCount;

        public void InitIngredient(int id, int needCount)
        {
            ingredientButton.gameObject.SetActive(true);

            var itemInfo = ResourceManager.Instance.GetAllResource<Item>("Item").FirstOrDefault(f => f.ID == id);

            ingredientIcon.sprite = itemInfo.Icon;

            int containCount = SaveManager.Instance.InventoryController.GetItemCount(itemInfo.ID);

            ingredientNeedCount.text = containCount + "/" + needCount;
            ingredientButton.onClick.RemoveAllListeners();
            ingredientButton.onClick.AddListener(() =>
            {
                EquipmentUI.Instance.ShowItemInfo(itemInfo, ingredientButton.transform.position.AddY(180f * (Screen.height / 2688f)));
            });

            ingredientNeedCount.GetComponentInChildren<CircleOutline>().effectColor = containCount >= needCount ? Color.black : Color.red;
        }

        public void Clear()
        {
            ingredientButton.gameObject.SetActive(false);
            //ingredientIcon.sprite = ResourceManager.Instance.GetResource<Sprite>("Sprite/transparent");
            //ingredientNeedCount.text = "";
        }
    }

    private void OnEnable()
    {
        dynamic.Collapse(true);
        infoItemPopup.onClick.AddListener(CloseItemInfo);
        UpdateUI();
        PlayerManager.Instance.DummyInit();
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

        stat_sword.text = currentSword.Tier.ToString();
        stat_axe.text = currentAxe.Tier.ToString();
        stat_pickaxe.text = currentPickaxe.Tier.ToString();
        stat_hunger.text = SaveManager.Instance.PlayerController.GetPlayerMaxHunger().ToString();
        stat_hp.text = SaveManager.Instance.PlayerController.GetPlayerMaxHp().ToString();

        PlayerManager.Instance.ChangeDummyPlayerEquipment(EquipmentType.CHEST, currentChest.mesh);
    }

    void ShowItemInfo(Item item, Vector3 pos)
    {
        if (infoItemTween != null)
            infoItemTween.Kill();

        infoItemPopup.gameObject.SetActive(true);

        infoItemPopup.transform.position = pos;
        infoItemPopup.transform.localScale = Vector3.zero;
        infoItemPopupName.text = item.NameLocal.GetLocalizedString();
        infoItemPopupIcon.sprite = item.Icon;
        infoItemTween = infoItemPopup.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)infoItemPopup.transform);
    }

    void CloseItemInfo()
    {
        if (infoItemTween != null)
            infoItemTween.Kill();

        infoItemTween = infoItemPopup.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => infoItemPopup.gameObject.SetActive(false));
    }

    ///<summary>
    ///장비 아이템 클릭시
    ///</summary>
    public void OnClickEquipmentSlot(EquipmentType type)
    {
        var equipment = SaveManager.Instance.PlayerController.GetCurrentEquipmentData(type);

        // //만약 현제 장착중인 장비가 null이면 가장 낮은 티어의 장비 가져오기
        // if (equipment == null)
        // {
        //     equipment = Resources.LoadAll<Equipment>("Equipment").OrderBy(f => f.Tier).Where(f => f.Type == type).First();
        // }

        if (equipment == null || equipment.NextEquipment == null)
            return;

        InitUpgradeUI(equipment.NextEquipment, equipment);

        void InitUpgradeUI(Equipment nextUpgrade, Equipment currentEquipment)
        {
            infoItemPopup.gameObject.SetActive(false);

            selectedEquipment = currentEquipment;

            NextUpgradeEquipment_Image.sprite = nextUpgrade.Icon;
            NextUpgradeEquipment_Description.LocalizedString(nextUpgrade.DescLocal);
            NextUpgradeEquipment_Name.LocalizedString(nextUpgrade.NameLocal);

            ClearIngredientSlots();

            for (int i = 0; i < nextUpgrade.Recipe.Length; i++)
            {
                NextUpgradeEquipment_Ingredients[i].Clear();
                NextUpgradeEquipment_Ingredients[i].InitIngredient(nextUpgrade.Recipe[i].Item.ID, nextUpgrade.Recipe[i].Count);
            }

            if (SaveManager.Instance.PlayerController.CheckPossibleUpgradeNextEquipment(selectedEquipment.Type))
                upgradeBtn.interactable = true;
            else
                upgradeBtn.interactable = false;


            if (nextUpgrade != null)
            {
                print(SaveManager.Instance.PlayerController.GetPlayerLevel() + " / " + nextUpgrade.RV_RequireLVL);

                if (SaveManager.Instance.PlayerController.GetPlayerLevel() >= nextUpgrade.RV_RequireLVL)
                    RvButton.gameObject.SetActive(true);
                else
                    RvButton.gameObject.SetActive(false);
            }
            else
            {
                RvButton.gameObject.SetActive(false);
                dynamic.Collapse();
            }

            PlayerManager.Instance.ChangeDummyPlayerEquipment(selectedEquipment.Type, selectedEquipment.mesh);

            dynamic.Expand();
        }
    }

    ///<summary>
    ///업그레이드 버튼 클릭시
    ///</summary>
    public void OnClickUpgradeBtn()
    {
        //업그레이드 성공시
        if (SaveManager.Instance.PlayerController.CheckPossibleUpgradeNextEquipment(selectedEquipment.Type))
        {

            foreach (var needyItem in selectedEquipment.NextEquipment.Recipe)
            {
                SaveManager.Instance.InventoryController.LossItem(needyItem.Item.ID, needyItem.Count);
            }

            ClearIngredientSlots();

            UpgradeSelectedWeapon();

            UpdateUI();

            selectedEquipment = selectedEquipment.NextEquipment;
            if (selectedEquipment.NextEquipment == null)
                dynamic.Collapse();
            else
                OnClickEquipmentSlot(selectedEquipment.Type);
        }
        else
        {
            Debug.LogError("업그레이드 실패");
        }
    }
    public void OnClickRV()
    {
        MondayOFF.AdsManager.ShowRewarded(() =>
        {
            AnalyticsManager.Instance.RewardVideoEvent($"Equipment {selectedEquipment.NextEquipment.ID}");
            UpgradeSelectedWeapon();

            UpdateUI();

            selectedEquipment = selectedEquipment.NextEquipment;
            if (selectedEquipment.NextEquipment == null)
                dynamic.Collapse();
            else
                OnClickEquipmentSlot(selectedEquipment.Type);
        });
    }
    void UpgradeSelectedWeapon()
    {
        PlayerManager.Instance.player.GetComponent<PlayerAttack>().GenearteEquipment(selectedEquipment.NextEquipment.Type, selectedEquipment.NextEquipment.ID);
        AnalyticsManager.Instance.GameEvent(AnalyticsGameEventType.EQUIPMENT, selectedEquipment.NextEquipment.ID.ToString());
        if (selectedEquipment.NextEquipment != null)
            PlayerManager.Instance.ChangeDummyPlayerEquipment(selectedEquipment.Type, selectedEquipment.NextEquipment.mesh);
        else
            PlayerManager.Instance.ChangeDummyPlayerEquipment(selectedEquipment.Type, selectedEquipment.mesh);

        GameObserver.Instance.Call(GameObserverType.Player.ON_PLAYER_EQUIPMENT_CHANGE);
    }

    public void ClearIngredientSlots()
    {
        foreach (var ingredient in NextUpgradeEquipment_Ingredients)
        {
            ingredient.Clear();
        }
    }
}
