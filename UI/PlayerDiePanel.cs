using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerDiePanel : UIBase
{
    [field: SerializeField] public Text reviveCountText { get; private set; }

    float count = 0;

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ReviveIn(float count)
    {
        this.count = count;
        this.TaskWhile(1f, 0, () =>
        {
           reviveCountText.text = LocalizationManager.Instance.GetLocalizedStringValue("UI", "DEAD_REVIVE", count.ToString());
            count--;
        }, () => this.gameObject.activeSelf);
    }
}
