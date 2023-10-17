using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelUI : SingletonInstance<LevelUI>
{
    [SerializeField] private Image expFill;
    [SerializeField] private Text levelText;
    [SerializeField] private Text expText;
    [SerializeField] private CanvasGroup levelupEffectCanvasGroup;

    Sequence expBarSeq;


    // Start is called before the first frame update
    void Start()
    {
        expBarSeq = DOTween.Sequence();

        this.SetListener(GameObserverType.Player.ON_PLAYER_EXP_CHANGE, OnChangeExp);
        this.SetListener(GameObserverType.Player.ON_PLAYER_LEVEL_CHANGE, OnLevelChange);

        expText.text = SaveManager.Instance.PlayerController.GetPlayerExp() + " / " + PlayerSaveData.requiredEXP[SaveManager.Instance.PlayerController.GetPlayerLevel()];
        expFill.fillAmount = (float)SaveManager.Instance.PlayerController.GetPlayerExp() / (float)PlayerSaveData.requiredEXP[SaveManager.Instance.PlayerController.GetPlayerLevel()];
        levelText.text = SaveManager.Instance.PlayerController.GetPlayerLevel().ToString();

    }

    void OnChangeExp()
    {
        // if (expBarSeq.IsPlaying())
        //     expBarSeq.Kill();

        // // if (expBarSeq != null)
        // expBarSeq = DOTween.Sequence();

        expBarSeq.Append(expFill.DOFillAmount((float)SaveManager.Instance.PlayerController.GetPlayerExp() / (float)PlayerSaveData.requiredEXP[SaveManager.Instance.PlayerController.GetPlayerLevel()], 0.5f).OnPlay(() => expText.text = SaveManager.Instance.PlayerController.GetPlayerExp() + " / " + PlayerSaveData.requiredEXP[SaveManager.Instance.PlayerController.GetPlayerLevel()]));
        // .SetAutoKill(false);
    }

    void OnLevelChange()
    {
        if (expBarSeq != null)
            expBarSeq.Kill();

        // if (expBarSeq != null)
        expBarSeq = DOTween.Sequence();

        expBarSeq.Append(expFill.DOFillAmount(1, 1f).OnPlay(() => expText.text = PlayerSaveData.requiredEXP[SaveManager.Instance.PlayerController.GetPlayerLevel() - 1] + " / " + PlayerSaveData.requiredEXP[SaveManager.Instance.PlayerController.GetPlayerLevel() - 1]).OnStart(() => levelupEffectCanvasGroup.DOFade(1f, 0.5f)));
        // .SetAutoKill(false);
        expBarSeq.Append(transform.DOScale(Vector3.one * 1.15f, 0.6f).OnComplete(() => levelText.text = SaveManager.Instance.PlayerController.GetPlayerLevel().ToString()));
        expBarSeq.Append(transform.DOScale(Vector3.one, 0.4f));
        expBarSeq.Append(expFill.DOFillAmount(0, 0f));
        expBarSeq.Append(expFill.DOFillAmount((float)SaveManager.Instance.PlayerController.GetPlayerExp() / (float)PlayerSaveData.requiredEXP[SaveManager.Instance.PlayerController.GetPlayerLevel()], 0f)
        .OnComplete(() => { expText.text = SaveManager.Instance.PlayerController.GetPlayerExp() + " / " + PlayerSaveData.requiredEXP[SaveManager.Instance.PlayerController.GetPlayerLevel()]; levelupEffectCanvasGroup.DOFade(0f, 0.5f); }));
    }
}
