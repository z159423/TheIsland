using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerUIBase : UIBase
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