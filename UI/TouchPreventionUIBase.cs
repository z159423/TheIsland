using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPreventionUIBase : UIBase
{
    public override void Hide()
    {
        Destroy(gameObject);
    }

    public override void Show()
    {

    }
}
