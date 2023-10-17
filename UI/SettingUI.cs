using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingUI : UIBase
{
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Text soundOnOffText;

    [SerializeField] private Slider hapticSlider;
    [SerializeField] private Text hapticOnOffText;

    [SerializeField] RectTransform panelTransform;

    [TitleGroup("Continue")]
    [SerializeField] GameObject continueObject;
    [SerializeField] CanvasGroup continueGroup;
    [SerializeField] GameObject continueGoogle;
    [SerializeField] GameObject continueApple;

    [TitleGroup("Login")]
    [SerializeField] CanvasGroup loginGroup;

    [TitleGroup("Delete")]
    [SerializeField] GameObject deletePopup;
    [SerializeField] Image deleteBack;
    [SerializeField] GameObject deletePanel;
    [SerializeField] GameObject deleteRestartPanel;
    [SerializeField] Button deleteButton;

    public override void Show()
    {
        soundSlider.value = SaveManager.Instance.OptionController.GetSounds() ? 0 : 1;
        hapticSlider.value = SaveManager.Instance.OptionController.GetHaptics() ? 0 : 1;

        OnChangeSoundSetting();
        OnChangeHapticSetting();

        continueObject.SetActive(SaveManager.Instance.DatabaseController.GetLoginType() == SaveDatabase.LoginType.GUEST);

        continueGroup.interactable = true;

#if UNITY_ANDROID
        continueGoogle.SetActive(true);
        continueApple.SetActive(false);
#endif
#if UNITY_IOS
        continueGoogle.SetActive(true);
        continueApple.SetActive(true);
#endif

        LayoutRebuilder.ForceRebuildLayoutImmediate(panelTransform);
    }

    public override void Hide()
    {
        HideAnimation(() => Destroy(gameObject));
    }

    public void OnChangeSoundSetting()
    {
        if (soundSlider.value == 0)
            soundOnOffText.text = LocalizationManager.Instance.GetLocalizedString("UI", "SETTING_TOGGLE_ON");
        else
            soundOnOffText.text = LocalizationManager.Instance.GetLocalizedString("UI", "SETTING_TOGGLE_OFF");
    }

    public void OnChangeHapticSetting()
    {
        if (hapticSlider.value == 0)
            hapticOnOffText.text = LocalizationManager.Instance.GetLocalizedString("UI", "SETTING_TOGGLE_ON");
        else
            hapticOnOffText.text = LocalizationManager.Instance.GetLocalizedString("UI", "SETTING_TOGGLE_OFF");
    }

    public void OnChangeSetting()
    {
        SaveManager.Instance.OptionController.SetSounds(soundSlider.value == 0 ? true : false);
        SaveManager.Instance.OptionController.SetHaptics(hapticSlider.value == 0 ? true : false);
    }

    public void OnClickSoundSlider()
    {
        soundSlider.value = soundSlider.value == 0 ? 1 : 0;

        OnChangeSoundSetting();
    }

    public void OnClickHapticSlider()
    {
        hapticSlider.value = hapticSlider.value == 0 ? 1 : 0;

        OnChangeHapticSetting();
    }

    public void OnClickContinueGoogle()
    {
        continueGroup.interactable = false;
        DatabaseManager.Instance.LinkGoogle(ContinueCancel, ContinueLink);
    }

    public void OnClickContinueApple()
    {
        continueGroup.interactable = false;
        DatabaseManager.Instance.LinkApple(ContinueCancel, ContinueLink);
    }

    public void OnClickLoginGoogle()
    {
        loginGroup.interactable = false;
        DatabaseManager.Instance.LoginGoogle(false);
    }

    public void OnClickLoginApple()
    {
        loginGroup.interactable = false;
        DatabaseManager.Instance.LoginApple(false);
    }

    public void OnClickDeleteAccountHidePopup()
    {
        deleteBack.DOFade(0f, 0.4f);
        deletePanel.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() => deletePopup.gameObject.SetActive(false));
    }

    public void OnClickDeleteAccountShowPopup()
    {
        deleteRestartPanel.SetActive(false);

        deletePopup.gameObject.SetActive(true);

        deleteBack.GetComponent<Button>().interactable = true;

        deleteBack.color = Color.clear;
        deleteBack.DOFade(0.5f, 0.4f);

        deletePanel.SetActive(true);
        deletePanel.transform.localScale = Vector3.zero;
        deletePanel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }

    public void OnClickDeleteAccount()
    {
        deleteButton.interactable = false;
        deleteBack.GetComponent<Button>().interactable = false;
        DatabaseManager.Instance.DeleteAccount(() =>
        {
            deleteRestartPanel.SetActive(true);
            deletePanel.SetActive(false);
            deleteRestartPanel.transform.localScale = Vector3.zero;
            deleteRestartPanel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        });
    }

    public void OnClickRestore()
    {
        MondayOFF.IAPManager.RestorePurchase();
    }

    public void OnClickRestart()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void ContinueLink()
    {
        continueObject.gameObject.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelTransform);
    }

    void ContinueCancel()
    {
        continueGroup.interactable = true;
    }
}
