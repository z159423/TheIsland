using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;


public class UIFloatingCompanion : FloatingUI
{

    [SerializeField] UIDynamicPanel dynamicPanel;
    [SerializeField] GameObject panel;

    [SerializeField] GameObject trailBtn;
    [SerializeField] GameObject rvBtn;

    [SerializeField] GameObject detailRVVideoPanel;
    [SerializeField] GameObject detailRVTicketPanel;

    Tween toggleTween;

    private Companion companion;

    public void Init()
    {
        panel.transform.localScale = Vector3.zero;

        dynamicPanel.Collapse(true);

        companion = GetComponentInParent<Companion>();

        this.SetListener(GameObserverType.IAP.ON_CHANGE_RV_TOKEN, OnChangeRVToken);
    }

    public void InitBtn(bool trial)
    {
        if (trial)
            trailBtn.SetActive(true);
        else
            rvBtn.SetActive(true);
    }

    public override void Hide()
    {
        if (toggleTween != null)
            toggleTween.Kill();
        toggleTween = panel.transform.DOScale(Vector3.zero, 0.3f);
        dynamicPanel.Collapse();
    }

    public override void Show()
    {
        if (companion.GetHired)
            return;

        if (toggleTween != null)
            toggleTween.Kill();
        toggleTween = panel.transform.DOScale(Vector3.one, 0.3f);
        dynamicPanel.Expand();

        OnChangeRVToken();
    }

    public void TrailButtonClick()
    {
        companion.OnClickTrialBtn();

        Hide();
    }

    public void RVButtonClick()
    {
        companion.OnClickRvBtn();

        Hide();
    }

    void OnChangeRVToken()
    {
        detailRVVideoPanel.SetActive(SaveManager.Instance.IAPController.GetRVToken() == 0);
        detailRVTicketPanel.SetActive(SaveManager.Instance.IAPController.GetRVToken() > 0);
    }
}

