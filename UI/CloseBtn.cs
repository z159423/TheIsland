using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseBtn : MonoBehaviour
{
    public void Close()
    {
        if (GetComponentInParent<UIBase>() != null)
            GetComponentInParent<UIBase>().Hide();
    }
}
