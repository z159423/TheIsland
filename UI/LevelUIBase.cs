using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUIBase : UIBase
{
    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
