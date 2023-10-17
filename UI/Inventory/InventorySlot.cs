using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class InventorySlot : ItemSlot
{
    public override void OnClickThisSlot()
    {
        CombinedInventoryUI.Instance.OnSelectSlot(this);
        
        base.OnClickThisSlot();



        //GetComponent<Image>().color = Color.yellow;
    }

}
