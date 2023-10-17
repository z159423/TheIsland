using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinedInventoryUIBase : UIBase
{
    public override void Show()
    {
        //throw new System.NotImplementedException();
    }

    public override void Hide()
    {
        //throw new System.NotImplementedException();

        HideAnimation(() => Destroy(gameObject));
        //Destroy(gameObject);
    }
}
