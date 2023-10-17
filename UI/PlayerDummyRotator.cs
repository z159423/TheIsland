using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PlayerDummyRotator : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    Vector2 mPrevPos;

    public void OnBeginDrag(PointerEventData data)
    {
        mPrevPos = data.position;
        // print(data.position);
    }

    public void OnDrag(PointerEventData data)
    {
        var mDelta = (mPrevPos - data.position);

        PlayerManager.Instance.RotateDummy(mDelta);

        mPrevPos = data.position;

        // print(mDelta);
    }
}
